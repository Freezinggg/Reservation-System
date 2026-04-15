using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReservationSystem.Application.Interfaces.Randomizer
{
    public interface IRandomizer
    {
        double NextDouble(); //returns 0.0 - 1.0
    }
}
