using Microsoft.EntityFrameworkCore;
using ReservationSystem.Application.Interfaces.Repository;
using ReservationSystem.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReservationSystem.Infrastructure.Persistence.Repository
{
    public class SeatCategoryRepository : ISeatCategoryRepository
    {
        private readonly AppDbContext _db;

        public SeatCategoryRepository(AppDbContext db)
        {
            _db = db;
        }

        public async Task<bool> RestoreSeat(Dictionary<Guid, int> seatCategoryMap, CancellationToken ct)
        {
            foreach (var keyValue in seatCategoryMap)
            {
                var seatCategoryId = keyValue.Key;
                var quantity = keyValue.Value;

                var row = await _db.SeatCategories
                .Where(o => o.Id == seatCategoryId)
                .ExecuteUpdateAsync(setters => setters
                    .SetProperty(o => o.RemainingSeats, o => o.RemainingSeats + quantity)
                    , ct);

                if (row == 0) return false;
            }

            return true;
        }

        public async Task<bool> TryAllocateSeatAsync(Guid seatCategoryId, int qty, CancellationToken ct)
        {
            var rows = await _db.Database.ExecuteSqlInterpolatedAsync($@"
                UPDATE ""seat_category""
                SET ""RemainingSeats"" = ""RemainingSeats"" - {qty}
                WHERE ""Id"" = {seatCategoryId}
                  AND ""RemainingSeats"" >= {qty}", ct);

            return rows == 1;
        }
    }
}
