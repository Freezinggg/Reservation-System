using ReservationSystem.Domain.Exception;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReservationSystem.Domain.Entities
{
    public sealed class Event
    {
        public Guid Id { get; }
        public string Name { get; }
        public DateTimeOffset CreatedAt { get; private set; } //CreatedAt value is set by DB.

        public Event(string name)
        {
            if (string.IsNullOrWhiteSpace(name)) throw new InvariantViolationException("Event Name cannot be empty.");

            Id = Guid.NewGuid();
            Name = name;
        }
    }
}
