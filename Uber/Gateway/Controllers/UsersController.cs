using Gateway.Features.User;
using Gateway.Helpers.ChatHub;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace Gateway.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IHubContext<ChatHub, IChatHub> _chatHub;

        public UsersController(IMediator mediator, IHubContext<ChatHub, IChatHub> chatHub)
        {
            _mediator = mediator;
            _chatHub = chatHub;
        }

        [HttpPost]
        public async Task<ActionResult> Register([FromForm] Register.RegisterCommand request)
        {
            await _mediator.Send(request);
            return Ok();
        }

        [HttpPost("login")]
        public async Task<ActionResult> LogIn(LogIn.LogInCommand request)
        {
            var response = await _mediator.Send(request);
            return Ok(response);
        }

        [HttpPut]
        [Authorize]
        public async Task<ActionResult> UpdateProfile([FromForm] UpdateProfile.UpdateProfileCommand request)
        {
            await _mediator.Send(request);
            return Ok();
        }

        [HttpGet("get-drivers")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> GetAllDrivers()
        {
            var response = await _mediator.Send(new GetAllDrivers.GetAllDriversQuery());
            return Ok(response);
        }

        [HttpGet("{id:Guid}")]
        [Authorize]
        public async Task<ActionResult> GetUserById(Guid id)
        {
            var response = await _mediator.Send(new GetUserById.GetUserByIdQuery(id));
            return Ok(response);
        }

        [HttpPost("verify")]
        [Authorize]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> VerifyUser(VerifyUser.VerifyUserCommand request)
        {
            await _mediator.Send(request);
            return Ok();
        }

        [HttpPost("block")]
        [Authorize]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> BlockUser(BlockUser.BlockUserCommand request)
        {
            await _mediator.Send(request);
            return Ok();
        }

        [HttpPost("send-message")]
        [Authorize]
        [Authorize(Roles = "User,Driver")]
        public async Task<ActionResult> SendMessageToUser(SendMessageToUser.SendMessageToUserCommand request)
        {
            await _mediator.Send(request);
            await _chatHub.Clients.Group(request.UserId.ToString()).SendMessage(request.Message);
            return Ok();
        }
    }
}
