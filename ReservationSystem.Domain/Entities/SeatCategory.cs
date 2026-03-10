using ReservationSystem.Domain.Exception;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReservationSystem.Domain.Entities
{
    public sealed class SeatCategory
    {
        public Guid Id { get; }
        public Guid EventId { get; }
        public string Name { get; }
        public int Capacity { get; }
        public int RemainingSeats { get; private set; } //Set not allowed because mutation only happens in DB
        public DateTimeOffset CreatedAt { get; private set; } //CreatedAt value is set by DB.

        public SeatCategory(Guid eventId, string name, int capacity)
        {
            if(eventId == Guid.Empty) throw new InvariantViolationException("EventId cannot be empty.");
            if (string.IsNullOrWhiteSpace(name)) throw new InvariantViolationException("Seat Name cannot be empty.");
            if (capacity <= 0) throw new InvariantViolationException("Capacity have to be more than 0.");

            Id = Guid.NewGuid();
            EventId = eventId;
            Name = name;
            Capacity = capacity;
            RemainingSeats = capacity;
        }
    }
}
