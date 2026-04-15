using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReservationSystem.Application.Interfaces.Admission
{
    public interface ISeatRequestGate
    {
        bool Allow(int? cachedSeats);
    }
}
