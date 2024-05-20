using Gateway.Features.Rating;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Gateway.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RatingsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public RatingsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("{userId:Guid}")]
        public async Task<ActionResult> GetRatingForUser(Guid userId)
        {
            var response = await _mediator.Send(new GetRatingForUser.GetRatingForUserQuery(userId));
            return Ok(response);
        }

        [HttpPost]
        public async Task<ActionResult> AddRating(AddRating.AddRatingCommand request)
        {
            await _mediator.Send(request);
            return Ok();
        }
    }
}
