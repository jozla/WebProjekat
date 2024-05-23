using Common.Enums;
using Communication;
using Gateway.CQRS;
using Gateway.Validation;
using MediatR;
using Microsoft.ServiceFabric.Services.Client;
using Microsoft.ServiceFabric.Services.Remoting.Client;

namespace Gateway.Features.User
{
    public class VerifyUser
    {
        public record VerifyUserCommand(Guid Id) : ICommand;

        public class CommandHandler : ICommandHandler<VerifyUserCommand>
        {
            private readonly IConfiguration _configuration;

            public CommandHandler(IConfiguration configuration)
            {
                _configuration = configuration;
            }
            public async Task<Unit> Handle(VerifyUserCommand request, CancellationToken cancellationToken)
            {
                var proxy = ServiceProxy.Create<IUserStatefulCommunication>(
                    new Uri(_configuration.GetValue<string>("ProxyUrls:UserStateful")!), new ServicePartitionKey(1));

                var existingUser = await proxy.GetUserById(request.Id);

                if (existingUser == null)
                {
                    throw new EntityNotFoundException();
                }
                else
                {
                    existingUser.VerificationState = VerificationState.Verified;
                    await proxy.UpdateUser(existingUser);
                }

                return Unit.Value;
            }
        }
    }
}
