using ReservationSystem.Application.Interfaces.Admission;
using ReservationSystem.Application.Interfaces.Randomizer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReservationSystem.Infrastructure.Admission
{
    public class SeatRequestGate : ISeatRequestGate
    {
        private readonly IRandomizer _random;

        public SeatRequestGate(IRandomizer random) {
            _random = random;
        }

        public bool Allow(int? cachedSeats)
        {
            if (cachedSeats == null) return true;

            //Convert into 1.0, 0.5, 0.1 etc. lets say cachedSeats = 50, then 50 / 100 = 0.5 > 50%. so 50% probability
            var raw = cachedSeats.Value / 100.0;

            var probability = Math.Clamp(raw * 0.6, 0.05, 0.9); //only allows 60% of raw value. so if cachedseat = 100, then probability is 100% * 0.6 (around 60%) can go in. (balanced filtering, not too aggressive, not too loose)
            //var probability = Math.Clamp(raw * 0.3, 0.05, 0.9); //only allows 30% of raw value. aggressive filtering.

            return _random.NextDouble() < probability;
        }
    }
}
