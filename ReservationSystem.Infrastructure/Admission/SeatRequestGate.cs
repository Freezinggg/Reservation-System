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

            if (cachedSeats > 10) return true;

            if (cachedSeats > 0)
            {
                var probability = cachedSeats.Value / 10.0;
                return _random.NextDouble() < probability;
            }

            return _random.NextDouble() < 0.05;
        }
    }
}
