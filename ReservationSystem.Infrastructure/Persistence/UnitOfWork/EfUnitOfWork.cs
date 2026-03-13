using Microsoft.EntityFrameworkCore.Storage;
using ReservationSystem.Application.Interfaces;
using ReservationSystem.Application.Interfaces.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReservationSystem.Infrastructure.Persistence
{
    public sealed class EfUnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _db;
        private IDbContextTransaction? _tx;

        public EfUnitOfWork(AppDbContext db)
        {
            _db = db;
        }

        public async Task BeginAsync(CancellationToken ct)
        {
            _tx = await _db.Database.BeginTransactionAsync(ct);
        }

        public async Task CommitAsync(CancellationToken ct)
        {
            await _db.SaveChangesAsync(ct);
            await _tx!.CommitAsync(ct);
            await _tx!.DisposeAsync();
        }

        public async Task RollbackAsync(CancellationToken ct)
        {
            await _tx!.RollbackAsync(ct);
            await _tx!.DisposeAsync();
        }
    }
}
