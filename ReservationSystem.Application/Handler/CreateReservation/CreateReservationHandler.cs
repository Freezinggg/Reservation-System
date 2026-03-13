using MediatR;
using ReservationSystem.Application.Common;
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

        public CreateReservationHandler(IReservationRepository rsvRepo, ISeatCategoryRepository seatRepo, IUnitOfWork uow)
        {
            _rsvRepo = rsvRepo;
            _seatRepo = seatRepo;
            _uow = uow;
        }

        public async Task<Result<Guid>> Handle(CreateReservationCommand request, CancellationToken cancellationToken)
        {
            Result<Guid> result = new();
            if (request.Quantity <= 0) return Result<Guid>.Invalid("Quantity cannot be less than 1.");

            await _uow.BeginAsync(cancellationToken);
            try
            {
                //Try to allocate seat, if not available return fail and rollback immediately
                bool seatAvailable = await _seatRepo.TryAllocateSeatAsync(request.SeatCategoryId, request.Quantity, cancellationToken);
                if (!seatAvailable) {
                    await _uow.RollbackAsync(cancellationToken);
                    return Result<Guid>.Fail("No seat available.");
                }

                //Create reservation after allocate seat success/available
                Reservation reservation = new Reservation(request.SeatCategoryId, request.Quantity, DateTimeOffset.UtcNow.AddMinutes(3));
                await _rsvRepo.AddAsync(reservation, cancellationToken); //wheres my expiresat?

                await _uow.CommitAsync(cancellationToken);

                result = Result<Guid>.Success(reservation.Id);
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
