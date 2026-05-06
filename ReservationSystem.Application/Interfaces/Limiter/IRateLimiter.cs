using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReservationSystem.Application.Interfaces.Limiter
{
    public interface IRateLimiter
    {
        bool TryAllow(DateTime requestTime);
    }
}
