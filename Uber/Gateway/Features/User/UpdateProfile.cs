using Common.Models;
using Communication;
using Gateway.CQRS;
using MediatR;
using Microsoft.ServiceFabric.Services.Client;
using Microsoft.ServiceFabric.Services.Remoting.Client;

namespace Gateway.Features.User
{
    public class UpdateProfile
    {
        public record UpdateProfileCommand(
            Guid Id, string UserName, string Email, string OldPassword, string NewPassword, string Name, string LastName,
            string Birthday, string Address, string Image) : ICommand;

        public class CommandHandler : ICommandHandler<UpdateProfileCommand>
        {
            public async Task<Unit> Handle(UpdateProfileCommand request, CancellationToken cancellationToken)
            {
                var proxy = ServiceProxy.Create<IUserStatefulCommunication>(
                    new Uri("fabric:/Uber/UserStatefull"), new ServicePartitionKey(1));

                UserModel existingUser = await proxy.GetUserById(request.Id);
                var isMatch = BCrypt.Net.BCrypt.Verify(request.OldPassword, existingUser.Password);
                if (existingUser != null && isMatch)
                {
                    existingUser.UserName = request.UserName;
                    existingUser.Email = request.Email;
                    existingUser.Password = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
                    existingUser.Name = request.Name;
                    existingUser.LastName = request.LastName;
                    existingUser.Birthday = request.Birthday;
                    existingUser.Address = request.Address;
                    existingUser.Image = request.Image;

                    await proxy.UpdateUser(existingUser);
                }

                return Unit.Value;
            }
        }
    }
}
