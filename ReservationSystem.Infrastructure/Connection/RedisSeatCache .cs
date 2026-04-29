using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Logging;
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
        private readonly ILogger<RedisSeatCache> _logger;
        private readonly IDatabase _db;
        private static readonly TimeSpan SeatCacheTtl = TimeSpan.FromSeconds(10);

        public RedisSeatCache(IConnectionMultiplexer redis, ILogger<RedisSeatCache> logger)
        {
            _db = redis.GetDatabase();
            _logger = logger;
        }

        private static string GetSeatKey(Guid seatCategoryId) => $"seat:{seatCategoryId}:remaining";

        public async Task DecrementAsync(Guid seatCategoryId, int quantity)
        {
            try
            {
                var key = GetSeatKey(seatCategoryId);
                var exists = await _db.KeyExistsAsync(key);
                if (!exists) return;


                await _db.StringDecrementAsync(key, quantity);
                await _db.KeyExpireAsync(key, SeatCacheTtl);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to decrement cache seat for {SeatCategoryId}", seatCategoryId);
            }
        }

        public async Task<int?> GetRemainingAsync(Guid seatCategoryId)
        {
            try
            {
                var key = GetSeatKey(seatCategoryId);
                var value = await _db.StringGetAsync(key);

                return value.HasValue ? (int)value : null;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex,
                    "Failed to get cache seat for {SeatCategoryId}",
                    seatCategoryId);
                return null;
            }
        }

        public async Task SetAsync(Guid seatCategoryId, int quantity)
        {
            try
            {
                var key = GetSeatKey(seatCategoryId);
                await _db.StringSetAsync(key, quantity, SeatCacheTtl); //added TTL for 10s so after 10s the key will expires
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex,
                    "Failed to set cache seat for {SeatCategoryId}",
                    seatCategoryId);
            }
        }

        public async Task InvalidateAsync(Guid seatCategoryId)
        {
            try
            {
                //Delete because seat no longer valid
                var key = GetSeatKey(seatCategoryId);
                await _db.KeyDeleteAsync(key);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to invalidate cache seat for {SeatCategoryId}", seatCategoryId);
            }
        }
    }
}
