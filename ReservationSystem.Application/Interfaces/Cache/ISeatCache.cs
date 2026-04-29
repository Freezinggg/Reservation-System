using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReservationSystem.Application.Interfaces.Cache
{
    public interface ISeatCache
    {
        Task SetAsync(Guid seatCategoryId, int quantity);
        Task<int?> GetRemainingAsync(Guid seatCategoryId);
        Task DecrementAsync(Guid seatCategoryId, int quantity);
        Task InvalidateAsync(Guid seatCategoryId);
    }
}
