using Microsoft.EntityFrameworkCore;
using ReservationSystem.Application.Interfaces.Repository;
using ReservationSystem.Application.Records;
using ReservationSystem.Domain.Entities;
using ReservationSystem.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReservationSystem.Infrastructure.Persistence.Repository
{
    public class ReservationRepository : IReservationRepository
    {
        private readonly AppDbContext _db;

        public ReservationRepository(AppDbContext db)
        {
            _db = db;
        }

        public Task AddAsync(Reservation reservation, CancellationToken ct)
        {
            _db.Reservations.Add(reservation);
            return Task.CompletedTask;
        }

        public async Task<List<ExpiredReservartionRecord>> TryExpireReservation(CancellationToken ct)
        {
            //Claim the reservation first, then expires it
            var claimedReservation = new List<ExpiredReservartionRecord>();
            claimedReservation = await _db.Reservations
                .FromSqlInterpolated($@"
                        SELECT ""Id"", ""SeatCategoryId"", ""Quantity""
                        FROM ""Reservation""
                        WHERE ""Status"" = {(int)ReservationStatus.Active}
                           AND ""ExpiresAt"" <= {DateTimeOffset.UtcNow}
                        FOR UPDATE SKIP LOCKED
                        LIMIT 100")
                .Select(o => new ExpiredReservartionRecord(o.Id, o.SeatCategoryId, o.Quantity))
                .ToListAsync(ct);

            if (claimedReservation.Count == 0) return new List<ExpiredReservartionRecord>();

            //Update claimed reservation to expired
            var reservationIds = claimedReservation.Select(r => r.Id).ToList();
            var rows = await _db.Reservations
                    .Where(r => reservationIds.Contains(r.Id) &&
                        r.Status == ReservationStatus.Active)
                .ExecuteUpdateAsync(setters => setters
                    .SetProperty(o => o.Status, ReservationStatus.Expired)
                    , ct);

            //Success
            if (rows == 0) return new List<ExpiredReservartionRecord>();
            return claimedReservation;
        }
    }
}
