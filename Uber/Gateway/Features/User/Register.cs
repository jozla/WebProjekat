using Common.Enums;
using Common.Models;
using Communication;
using Gateway.CQRS;
using MediatR;
using Microsoft.ServiceFabric.Services.Client;
using Microsoft.ServiceFabric.Services.Remoting.Client;
namespace Gateway.Features.User
{
    public class Register
    {
        public record RegisterCommand(
             string UserName, string Email, string Password, string Name, string LastName,
             DateOnly Birthday, string Address, UserRole Role, string Image) : ICommand;

        public class CommandHandler : ICommandHandler<RegisterCommand>
        {

            public async Task<Unit> Handle(RegisterCommand request, CancellationToken cancellationToken)
            {
                var newUser = new UserModel
                {
                    Id = Guid.NewGuid(),
                    UserName = request.UserName,
                    Email = request.Email,
                    Password = BCrypt.Net.BCrypt.HashPassword(request.Password),
                    Name = request.Name,
                    LastName = request.LastName,
                    Birthday = request.Birthday,
                    Address = request.Address,
                    Role = request.Role,
                    Image = request.Image
                };

                if (newUser.Role == UserRole.Driver)
                {
                    newUser.VerificationState = VerificationState.Processing;
                }

                var proxy = ServiceProxy.Create<IUserStatefullCommunication>(
                    new Uri("fabric:/Uber/UserStatefull"), new ServicePartitionKey(1));

                await proxy.Register(newUser);
                return Unit.Value;
            }
        }
    }
}
