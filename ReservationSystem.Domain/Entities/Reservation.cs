using ReservationSystem.Domain.Enums;
using ReservationSystem.Domain.Exception;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReservationSystem.Domain.Entities
{
    public sealed class Reservation
    {
        public Guid Id { get; }
        public Guid SeatCategoryId { get; }
        public int Quantity { get; }
        public ReservationStatus Status { get; private set; } //Allows only this object to mutates it
        public DateTimeOffset CreatedAt { get; private set; } //CreatedAt value is set by DB.
        public DateTimeOffset ExpiresAt { get; }

        public Reservation(Guid seatCategoryId, int quantity, DateTimeOffset expiresAt)
        {
            if (seatCategoryId == Guid.Empty) throw new InvariantViolationException("Seat cannot be empty.");
            if (quantity <= 0) throw new InvariantViolationException("Quantity have to be more than 0.");
            if (expiresAt <= DateTimeOffset.UtcNow) throw new InvariantViolationException("Expiration must be in the future.");

            Id = Guid.NewGuid();
            SeatCategoryId = seatCategoryId;
            Quantity = quantity;
            Status = ReservationStatus.Active; //Immediately active, so everytime Reservation created its always Active, and wont be defined by constructor.
            ExpiresAt = expiresAt;
        }

        public void Confirm()
        {
            EnsureNotTerminal();
            Status = ReservationStatus.Confirmed;
        }

        public void Cancel()
        {
            EnsureNotTerminal();
            Status = ReservationStatus.Cancelled;
        }
        
        public void EnsureNotTerminal()
        {
            if (Status == ReservationStatus.Confirmed || Status == ReservationStatus.Expired || Status == ReservationStatus.Cancelled)
                throw new InvalidStateTransitionException($"Reservation {Id} is in terminal state: {Status}");
        }
    }
}
