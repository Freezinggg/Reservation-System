using ReservationSystem.Application.Interfaces.Metric;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReservationSystem.Infrastructure.Observability
{
    public class InMemoryReservationMetric : IReservationMetric
    {
        private int _attempts = 0;
        private int _cacheReject = 0;
        private int _rateLimitReject = 0;
        private int _dbAttempts = 0;
        private int _success = 0;

        public void IncreaseAttempt() => _attempts++;

        public void IncreaseCacheReject() => _cacheReject++;
        public void IncreaseRateLimitReject() => _rateLimitReject++;

        public void IncreaseDbAttempt() => _dbAttempts++;

        public void IncreaseSuccess() => _success++;

        public Snapshot SnapshotAndReset()
        {
            Snapshot snap = new(_attempts, _cacheReject, _rateLimitReject, _dbAttempts, _success);

            _attempts = 0;
            _cacheReject = 0;
            _rateLimitReject = 0;
            _dbAttempts = 0;
            _success = 0;

            return snap;
        }
    }
}
