using ReservationSystem.Application.Interfaces.Cache;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReservationSystem.Infrastructure.Connection
{
    public class RedisSeatCache : ISeatCache
    {
        private readonly IDatabase _db;

        public RedisSeatCache(IConnectionMultiplexer redis)
        {
            _db = redis.GetDatabase();
        }

        public async Task DecrementAsync(Guid seatCategoryId, int quantity)
        {
            var key = $"seat:{seatCategoryId}:remaining";

            await _db.StringDecrementAsync(key, quantity);
            await _db.KeyExpireAsync(key, TimeSpan.FromSeconds(20));
        }

        public async Task<int?> GetRemainingAsync(Guid seatCategoryId)
        {
            var key = $"seat:{seatCategoryId}:remaining";

            var value = await _db.StringGetAsync(key);

            return value.HasValue ? (int)value : null;
        }
    }
}
