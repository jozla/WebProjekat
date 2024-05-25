using Gateway.Features.Rating;
using MediatR;
using Microsoft.AspNetCore.Authorization;
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
        [Authorize]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> GetRatingForUser(Guid userId)
        {
            var response = await _mediator.Send(new GetRatingForUser.GetRatingForUserQuery(userId));
            return Ok(response);
        }

        [HttpPost]
        [Authorize]
        [Authorize(Roles = "User")]
        public async Task<ActionResult> AddRating(AddRating.AddRatingCommand request)
        {
            await _mediator.Send(request);
            return Ok();
        }
    }
}
