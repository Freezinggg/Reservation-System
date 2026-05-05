using ReservationSystem.Application.Interfaces.Metric;

namespace ReservationSystem.API.Services
{
    public class ReservationMetricSnapshotService : BackgroundService
    {
        private readonly IReservationMetric _metric;

        public ReservationMetricSnapshotService(IReservationMetric metric)
        {
            _metric = metric;
        }


        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);

                var snap = _metric.SnapshotAndReset();

                Console.WriteLine($@"
                [Snapshot]
                attempts={snap.Attempts}
                cache_reject={snap.CacheReject}
                db_attempts={snap.DbAttempts}
                success={snap.Success}
                conversion={(snap.DbAttempts == 0 ? 0 : (double)snap.Success / snap.DbAttempts):P}
                waste={(snap.DbAttempts == 0 ? 0 : (double)(snap.DbAttempts - snap.Success) / snap.DbAttempts):P}
                ");
            }
        }
    }
}
