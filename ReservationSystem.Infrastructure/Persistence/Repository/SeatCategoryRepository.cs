using Microsoft.EntityFrameworkCore;
using ReservationSystem.Application.Interfaces.Repository;
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
