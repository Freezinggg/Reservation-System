using ReservationSystem.Application.Records;
using ReservationSystem.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReservationSystem.Application.Interfaces.Repository
{
    public interface IReservationRepository
    {
        Task AddAsync(Reservation reservation, CancellationToken ct);
        Task<List<ExpiredReservartionRecord>> TryExpireReservation(CancellationToken ct);

    }
}
