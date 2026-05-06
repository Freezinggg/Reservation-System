using ReservationSystem.Application.Interfaces.Limiter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReservationSystem.Infrastructure.RateLimiter
{
    public class InMemoryRateLimiter : IRateLimiter
    {
        private readonly object _lock = new object();

        private readonly int requestLimit = 100;
        TimeSpan resetWindow = TimeSpan.FromSeconds(30);

        private int requestCount = 0;
        private DateTime currentTimeWindow = DateTime.UtcNow;
        
        public bool TryAllow(DateTime requestTime)
        {
            lock (_lock)
            {
                //If request time > 30s diff than time window, then reset
                if ((requestTime - currentTimeWindow) > resetWindow)
                {
                    currentTimeWindow = requestTime;
                    requestCount = 1;

                    return true;
                }

                //Check request limit
                if (requestCount >= requestLimit) return false;

                requestCount++;
                return true;
            }
        }
    }
}
