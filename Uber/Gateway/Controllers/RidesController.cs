using Gateway.Features.Ride;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Gateway.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RidesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public RidesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<ActionResult> AddRide(AddRide.AddRideCommand request)
        {
            await _mediator.Send(request);
            return Ok();
        }

        [HttpPut("confirm-ride")]
        public async Task<ActionResult> ConfirmRide(ConfirmRide.ConfirmRideCommand request)
        {
            await _mediator.Send(request);
            return Ok();
        }

        [HttpPut("finsih-ride")]
        public async Task<ActionResult> FinishRide(FinishRide.FinishRideCommand request)
        {
            await _mediator.Send(request);
            return Ok();
        }

        [HttpGet("{id:Guid}")]
        public async Task<ActionResult> GetRideById(Guid id)
        {
            var response = await _mediator.Send(new GetRideById.GetRideByIdQuery(id));
            return Ok(response);
        }

        [HttpGet]
        public async Task<ActionResult> GetAllRides()
        {
            var response = await _mediator.Send(new GetAllRides.GetAllRidesQuery());
            return Ok(response);
        }

        [HttpGet("new-rides")]
        public async Task<ActionResult> GetNewRides()
        {
            var response = await _mediator.Send(new GetNewRides.GetNewRidesQuery());
            return Ok(response);
        }

        [HttpGet("driver-rides/{driverId:Guid}")]
        public async Task<ActionResult> GetRidesForDriver(Guid driverId)
        {
            var response = await _mediator.Send(new GetRidesForDriver.GetRidesForDriverQuery(driverId));
            return Ok(response);
        }

        [HttpGet("user-rides/{userId:Guid}")]
        public async Task<ActionResult> GetRidesForUser(Guid userId)
        {
            var response = await _mediator.Send(new GetRidesForUser.GetRidesForUserQuery(userId));
            return Ok(response);
        }
    }
}
