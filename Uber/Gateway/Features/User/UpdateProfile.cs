using Common.Models;
using Communication;
using Gateway.CQRS;
using Gateway.Validation;
using MediatR;
using Microsoft.ServiceFabric.Services.Client;
using Microsoft.ServiceFabric.Services.Remoting.Client;

namespace Gateway.Features.User
{
    public class UpdateProfile
    {
        public record UpdateProfileCommand(
            Guid Id, string UserName, string Email, string OldPassword, string NewPassword, string Name, string LastName,
            string Birthday, string Address, IFormFile Image) : ICommand;

        public class CommandHandler : ICommandHandler<UpdateProfileCommand>
        {
            private readonly IConfiguration _configuration;

            public CommandHandler(IConfiguration configuration)
            {
                _configuration = configuration;
            }

            public async Task<Unit> Handle(UpdateProfileCommand request, CancellationToken cancellationToken)
            {
                var proxy = ServiceProxy.Create<IUserStatefulCommunication>(
                    new Uri(_configuration.GetValue<string>("ProxyUrls:UserStateful")!), new ServicePartitionKey(1));

                UserModel existingUser = await proxy.GetUserById(request.Id);
                if (existingUser == null)
                {
                    throw new EntityNotFoundException();
                }

                var isMatch = BCrypt.Net.BCrypt.Verify(request.OldPassword == "/" ? "" : request.OldPassword
                                    , existingUser.Password);

                if (!isMatch)
                {
                    throw new BadPasswordException();
                }
                else
                {
                    existingUser.UserName = request.UserName;
                    existingUser.Email = request.Email;
                    existingUser.Password = request.NewPassword == "/" ? BCrypt.Net.BCrypt.HashPassword("")
                            : BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
                    existingUser.Name = request.Name;
                    existingUser.LastName = request.LastName;
                    existingUser.Birthday = request.Birthday;
                    existingUser.Address = request.Address;

                    var imageFileName = $"{request.Image.FileName}";
                    var imagePath = Path.Combine("wwwroot", "images", imageFileName);

                    using (var imageStream = new FileStream(imagePath, FileMode.Create))
                    {
                        await request.Image.CopyToAsync(imageStream);
                    }

                    existingUser.Image = imageFileName;

                    await proxy.UpdateUser(existingUser);
                }

                return Unit.Value;
            }
        }
    }
}
