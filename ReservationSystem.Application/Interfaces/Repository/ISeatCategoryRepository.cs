using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReservationSystem.Application.Interfaces.Repository
{
    public interface ISeatCategoryRepository
    {
        Task<bool> TryAllocateSeatAsync(Guid seatCategoryId, int qty, CancellationToken ct);
        Task<bool> RestoreSeat(Dictionary<Guid, int> seatCategoryMap, CancellationToken ct);
        Task<int> GetRemainigSeatAsync(Guid seatCategoryId, CancellationToken ct);
    }
}
