using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReservationSystem.Domain.Enums
{
    public enum ReservationStatus : int
    {
        Active = 1,
        Confirmed,
        Expired,
        Cancelled
    }
}
