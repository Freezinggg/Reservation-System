using ReservationSystem.Application.Interfaces.Repository;
using ReservationSystem.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReservationSystem.Infrastructure.Persistence.Repository
{
    public class ReservationRepository : IReservationRepository
    {
        private readonly AppDbContext _db;

        public ReservationRepository(AppDbContext db)
        {
            _db = db;
        }

        public Task AddAsync(Reservation reservation, CancellationToken ct)
        {
            _db.Reservations.Add(reservation);
            return Task.CompletedTask;
        }
    }
}
