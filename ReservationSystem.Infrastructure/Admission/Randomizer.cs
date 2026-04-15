using ReservationSystem.Application.Interfaces.Randomizer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReservationSystem.Infrastructure.Admission
{
    public class Randomizer : IRandomizer
    {
        private readonly Random _random = new Random();

        public double NextDouble()
        {
            return _random.NextDouble();
        }
    }
}
