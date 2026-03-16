using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReservationSystem.Application.Records
{
    public sealed class ExpiredReservartionRecord
    {
        public Guid Id { get; }
        public Guid SeatCategoryId { get; }
        public int Quantity { get; }

        public ExpiredReservartionRecord(Guid id, Guid seatCategoryId, int quantity)
        {
            Id = id;
            SeatCategoryId = seatCategoryId;
            Quantity = quantity;
        }
    }
}
