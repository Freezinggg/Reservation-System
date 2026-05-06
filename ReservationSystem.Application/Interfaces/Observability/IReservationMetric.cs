using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReservationSystem.Application.Interfaces.Metric
{
    public interface IReservationMetric
    {
        void IncreaseAttempt();
        void IncreaseCacheReject();
        void IncreaseRateLimitReject();
        void IncreaseDbAttempt();
        void IncreaseSuccess();

        Snapshot SnapshotAndReset();
    }

    public record Snapshot(
        long Attempts,
        long CacheReject,
        long RateLimitReject,
        long DbAttempts,
        long Success
    );
}
