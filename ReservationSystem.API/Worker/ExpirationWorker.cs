using ReservationSystem.Application.Interfaces.Cache;
using ReservationSystem.Application.Interfaces.Repository;
using ReservationSystem.Application.Interfaces.UnitOfWork;

namespace ReservationSystem.API.Worker
{
    public sealed class ExpirationWorker : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IUnitOfWork _uow;
        private readonly ISeatCache _seatCache;

        public ExpirationWorker(IServiceScopeFactory scopeFactory, IUnitOfWork uow, ISeatCache seatCache)
        {
            _scopeFactory = scopeFactory;
            _uow = uow;
            _seatCache = seatCache;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using var scope = _scopeFactory.CreateScope();
                    var reservationRepo = scope.ServiceProvider.GetRequiredService<IReservationRepository>();
                    var seatCategoryRepo = scope.ServiceProvider.GetRequiredService<ISeatCategoryRepository>();

                    await _uow.BeginAsync(stoppingToken);

                    //Claim expired reservation first
                    var expiredReservation = await reservationRepo.TryExpireReservation(stoppingToken);
                    if(expiredReservation.Count == 0)
                    {
                        await _uow.CommitAsync(stoppingToken);
                    }
                    else
                    {
                        //Restore seats
                        Dictionary<Guid, int> seatCategoryRestoreMap = new Dictionary<Guid, int>();
                        foreach (var r in expiredReservation)
                        {
                            if (seatCategoryRestoreMap.ContainsKey(r.SeatCategoryId))
                                seatCategoryRestoreMap[r.SeatCategoryId] += r.Quantity;
                            else
                                seatCategoryRestoreMap[r.SeatCategoryId] = r.Quantity;
                        }

                        var restoreSeatResult = await seatCategoryRepo.RestoreSeat(seatCategoryRestoreMap, stoppingToken);
                        if (!restoreSeatResult) await _uow.RollbackAsync(stoppingToken);

                        await _uow.CommitAsync(stoppingToken);

                        //Invalidate cache here, so later app can re-write it
                        foreach (var seatCategoryId in seatCategoryRestoreMap.Keys)
                        {
                            await _seatCache.InvalidateAsync(seatCategoryId);
                        }
                    }
                }
                catch
                {
                    await _uow.RollbackAsync(stoppingToken);
                }

                await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
            }
        }
    }
}
