using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ReservationSystem.Application.Common;
using ReservationSystem.Application.Handler.CreateReservation;

namespace ReservationSystem.API.Controllers.Reservation
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReservationController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ReservationController(IMediator mediator)
        {
            _mediator = mediator;
        }


        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateReservationCommand command)
        {
            var result = await _mediator.Send(command);
            return result.Status switch
            {
                ResultStatus.Success => StatusCode(201, ApiResponse<Guid>.Ok(result.Data)),
                ResultStatus.Invalid => BadRequest(ApiResponse<Guid>.Fail(result.ErrorMessage)),
                ResultStatus.Fail => Conflict(ApiResponse<Guid>.Fail(result.ErrorMessage)),
                ResultStatus.Error => StatusCode(500, ApiResponse<Guid>.Fail(result.ErrorMessage)),
                _ => StatusCode(500, ApiResponse<Guid>.Fail("Unhandled result status")) //default value if ResultStatus is its new or default
            };
        }
    }
}
