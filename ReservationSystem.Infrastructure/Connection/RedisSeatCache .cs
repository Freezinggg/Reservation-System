using Microsoft.EntityFrameworkCore.Metadata.Internal;
using ReservationSystem.Application.Interfaces.Cache;
using ReservationSystem.Domain.Entities;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReservationSystem.Infrastructure.Connection
{
    //We use trycatch because we dont want cache/redis to be blocking. So if redis/cache down, then its fine, program still works
    public class RedisSeatCache : ISeatCache
    {
        private readonly IDatabase _db;

        public RedisSeatCache(IConnectionMultiplexer redis)
        {
            _db = redis.GetDatabase();
        }

        public async Task DecrementAsync(Guid seatCategoryId, int quantity)
        {
            try
            {
                var key = $"seat:{seatCategoryId}:remaining";

                await _db.StringDecrementAsync(key, quantity);
                await _db.KeyExpireAsync(key, TimeSpan.FromSeconds(20));
            }
            catch(Exception) {//non-block
                              }
        }

        public async Task<int?> GetRemainingAsync(Guid seatCategoryId)
        {
            try
            {
                var key = $"seat:{seatCategoryId}:remaining";

                var value = await _db.StringGetAsync(key);

                return value.HasValue ? (int)value : null;
            }
            catch { return null; }
        }

        public async Task SetAsync(Guid seatCategoryId, int quantity)
        {
            try
            {
                var key = $"seat:{seatCategoryId}:remaining";
                await _db.StringSetAsync(key, quantity);
            }
            catch { }
        }

        public async Task SetZeroAsync(Guid seatCategoryId)
        {
            try
            {
                var key = $"seat:{seatCategoryId}:remaining";
                await _db.StringSetAsync(key, 0);
            }
            catch { }
            
        }
    }
}
