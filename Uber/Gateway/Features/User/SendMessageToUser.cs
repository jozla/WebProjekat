using Communication;
using FluentValidation;
using Gateway.CQRS;
using Gateway.Validation;
using MediatR;
using Microsoft.ServiceFabric.Services.Client;
using Microsoft.ServiceFabric.Services.Remoting.Client;

namespace Gateway.Features.User;

public class SendMessageToUser
{
    public record SendMessageToUserCommand(Guid UserId, string Message) : ICommand;

    public class Validator : AbstractValidator<SendMessageToUserCommand>
    {
        public Validator()
        {
            RuleFor(entity => entity.UserId).NotEmpty().WithMessage("Id is required");
            RuleFor(entity => entity.Message).NotEmpty().WithMessage("Message is required");
        }
    }

    public class CommandHandler : ICommandHandler<SendMessageToUserCommand>
    {
        private readonly IConfiguration _configuration;

        public CommandHandler(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<Unit> Handle(SendMessageToUserCommand request, CancellationToken cancellationToken)
        {
            var proxy = ServiceProxy.Create<IUserStatefulCommunication>(
                new Uri(_configuration.GetValue<string>("ProxyUrls:UserStateful")!), new ServicePartitionKey(1));

            var existingUser = await proxy.GetUserById(request.UserId);

            if (existingUser == null)
            {
                throw new EntityNotFoundException();
            }

            return Unit.Value;
        }
    }
}
