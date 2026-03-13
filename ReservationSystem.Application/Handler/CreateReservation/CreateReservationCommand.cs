using MediatR;
using ReservationSystem.Application.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReservationSystem.Application.Handler.CreateReservation
{
    public class CreateReservationCommand : IRequest<Result<Guid>>
    {
        public Guid SeatCategoryId { get; }
        public int Quantity { get; }

        public CreateReservationCommand(Guid seatCategoryId, int quantity)
        {
            SeatCategoryId = seatCategoryId;
            Quantity = quantity;
        }
    }
}
