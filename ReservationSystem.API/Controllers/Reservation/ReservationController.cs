using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ReservationSystem.Application.Common;
using ReservationSystem.Application.Handler.CreateReservation;
using ReservationSystem.Application.Interfaces.Limiter;
using ReservationSystem.Application.Interfaces.Metric;

namespace ReservationSystem.API.Controllers.Reservation
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReservationController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IReservationMetric _rsvMetric;
        private readonly IRateLimiter _rateLimiter;

        public ReservationController(IMediator mediator, IReservationMetric rsvMetric, IRateLimiter rateLimiter)
        {
            _mediator = mediator;
            _rsvMetric = rsvMetric;
            _rateLimiter = rateLimiter;
        }


        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateReservationCommand command)
        {
            if (!_rateLimiter.TryAllow(DateTime.UtcNow))
            {
                _rsvMetric.IncreaseRateLimitReject();
                return StatusCode(429);
            }
            

            var result = await _mediator.Send(command);
            return result.Status switch
            {
                ResultStatus.Success => StatusCode(201, ApiResponse<Guid>.Ok(result.Data)),
                ResultStatus.Invalid => BadRequest(ApiResponse<Guid>.Fail(result.ErrorMessage)),
                ResultStatus.Fail => Conflict(ApiResponse<Guid>.Fail(result.ErrorMessage)),
                ResultStatus.Error => StatusCode(500, ApiResponse<Guid>.Fail(result.ErrorMessage)),
                ResultStatus.Conflict => StatusCode(409, ApiResponse<Guid>.Fail(result.ErrorMessage)),
                ResultStatus.TooManyRequests => StatusCode(429, ApiResponse<Guid>.Fail(result.ErrorMessage)),
                _ => StatusCode(500, ApiResponse<Guid>.Fail("Unhandled result status")) //default value if ResultStatus is its new or default
            };
        }
    }
}
