using MediatR;
using ReservationSystem.Application.Common;
using ReservationSystem.Application.Interfaces.Admission;
using ReservationSystem.Application.Interfaces.Cache;
using ReservationSystem.Application.Interfaces.Metric;
using ReservationSystem.Application.Interfaces.Repository;
using ReservationSystem.Application.Interfaces.UnitOfWork;
using ReservationSystem.Domain.Entities;
using ReservationSystem.Domain.Exception;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ReservationSystem.Domain.Exception.DomainException;

namespace ReservationSystem.Application.Handler.CreateReservation
{
    public class CreateReservationHandler : IRequestHandler<CreateReservationCommand, Result<Guid>>
    {
        private readonly IReservationRepository _rsvRepo;
        private readonly ISeatCategoryRepository _seatRepo;
        private readonly IUnitOfWork _uow;
        private readonly ISeatCache _seatCache;
        private readonly ISeatRequestGate _seatRequestGate;
        private readonly IReservationMetric _rsvMetric;
        public CreateReservationHandler(IReservationRepository rsvRepo, ISeatCategoryRepository seatRepo, IUnitOfWork uow, ISeatCache seatCache, ISeatRequestGate seatRequestGate, IReservationMetric rsvMetric)
        {
            _rsvRepo = rsvRepo;
            _seatRepo = seatRepo;
            _uow = uow;
            _seatCache = seatCache;
            _seatRequestGate = seatRequestGate;
            _rsvMetric = rsvMetric;
        }

        public async Task<Result<Guid>> Handle(CreateReservationCommand request, CancellationToken cancellationToken)
        {
            _rsvMetric.IncreaseAttempt();

            Result<Guid> result = new();
            if (request.Quantity <= 0) return Result<Guid>.Invalid("Quantity cannot be less than 1.");

            //Read cached seat
            var cachedSeat = await _seatCache.GetRemainingAsync(request.SeatCategoryId);

            if (cachedSeat == null)
            {
                // fetch from DB
                var actualRemainingSeat = await _seatRepo.GetRemainigSeatAsync(request.SeatCategoryId, cancellationToken);

                // populate cache
                await _seatCache.SetAsync(request.SeatCategoryId, actualRemainingSeat);
                cachedSeat = actualRemainingSeat;
            }

            //Admission here to control how many request can comes in based on cachedseat
            if (!_seatRequestGate.Allow(cachedSeat))
            {
                _rsvMetric.IncreaseCacheReject();
                return Result<Guid>.TooManyRequest("Too many requests.");
            }

            await _uow.BeginAsync(cancellationToken);
            try
            {
                _rsvMetric.IncreaseDbAttempt();

                //Try to allocate seat, if not available return fail and rollback immediately
                bool seatAvailable = await _seatRepo.TryAllocateSeatAsync(request.SeatCategoryId, request.Quantity, cancellationToken);
                if (!seatAvailable)
                {
                    await _uow.RollbackAsync(cancellationToken);

                    //Set cachedseat to 0 to reduce incoming request later
                    await _seatCache.InvalidateAsync(request.SeatCategoryId);

                    return Result<Guid>.Conflict("No seat available.");
                }

                //Create reservation after allocate seat success/available
                //Reservation reservation = new Reservation(request.SeatCategoryId, request.Quantity, DateTimeOffset.UtcNow.AddMinutes(3));
                Reservation reservation = new Reservation(request.SeatCategoryId, request.Quantity, DateTimeOffset.UtcNow.AddSeconds(30)); //30s for testing purpose


                await _rsvRepo.AddAsync(reservation, cancellationToken); //wheres my expiresat?

                await _uow.CommitAsync(cancellationToken);

                _rsvMetric.IncreaseSuccess();
                result = Result<Guid>.Success(reservation.Id);
                
                //Decrease cachedseat
                await _seatCache.DecrementAsync(request.SeatCategoryId, request.Quantity);

            }
            catch (DomainException ex)
            {
                //This is domain exception, which is to check INVARIANT
                await _uow.RollbackAsync(cancellationToken);

                switch (ex.Category)
                {
                    case FailureCategory.Invariant:
                        result = Result<Guid>.Invalid(ex.Message);
                        break;
                    case FailureCategory.Policy or FailureCategory.State:
                        result = Result<Guid>.Fail(ex.Message);
                        break;
                    default:
                        result = Result<Guid>.Error("Unhandled domain exception.");
                        break;
                }
            }
            catch (Exception ex)
            {
                //Catch in general, system crash etc
                await _uow.RollbackAsync(cancellationToken);
                result = Result<Guid>.ServiceUnavailable("Service unavailable");
            }


            return result;
        }
    }
}
