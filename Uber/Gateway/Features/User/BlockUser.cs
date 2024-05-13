using Common.Enums;
using Communication;
using Gateway.CQRS;
using MediatR;
using Microsoft.ServiceFabric.Services.Client;
using Microsoft.ServiceFabric.Services.Remoting.Client;

namespace Gateway.Features.User
{
    public class BlockUser
    {
        public record BlockUserCommand(Guid Id) : ICommand;

        public class CommandHandler : ICommandHandler<BlockUserCommand>
        {

            public async Task<Unit> Handle(BlockUserCommand request, CancellationToken cancellationToken)
            {
                var proxy = ServiceProxy.Create<IUserStatefullCommunication>(
                    new Uri("fabric:/Uber/UserStatefull"), new ServicePartitionKey(1));

                var existingUser = await proxy.GetUserById(request.Id);

                if (existingUser != null)
                {
                    existingUser.VerificationState = VerificationState.Unverified;
                    await proxy.UpdateUser(existingUser);
                }

                return Unit.Value;
            }
        }
    }
}
