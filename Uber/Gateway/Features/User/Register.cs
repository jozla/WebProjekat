using Common.Enums;
using Common.Models;
using Communication;
using FluentValidation;
using Gateway.CQRS;
using Gateway.Validation;
using MediatR;
using Microsoft.ServiceFabric.Services.Client;
using Microsoft.ServiceFabric.Services.Remoting.Client;
namespace Gateway.Features.User
{
    public class Register
    {
        public record RegisterCommand(
             string UserName, string Email, string? Password, string Name, string LastName,
             string? Birthday, string? Address, UserRole Role, IFormFile? Image) : ICommand;

        public class Validator : AbstractValidator<RegisterCommand>
        {
            public Validator()
            {
                RuleFor(entity => entity.UserName).NotEmpty().WithMessage("Username is required");
                RuleFor(entity => entity.Email).NotEmpty().WithMessage("Email is required");
                RuleFor(entity => entity.Name).NotEmpty().WithMessage("Name is required");
                RuleFor(entity => entity.LastName).NotEmpty().WithMessage("Last name is required");
            }
        }
        public class CommandHandler : ICommandHandler<RegisterCommand>
        {
            private readonly IConfiguration _configuration;

            public CommandHandler(IConfiguration configuration)
            {
                _configuration = configuration;
            }

            public async Task<Unit> Handle(RegisterCommand request, CancellationToken cancellationToken)
            {
                var proxy = ServiceProxy.Create<IUserStatefulCommunication>(
                    new Uri(_configuration.GetValue<string>("ProxyUrls:UserStateful")!), new ServicePartitionKey(1));

                var existingUser = await proxy.GetUserByEmail(request.Email);

                if (existingUser != null)
                {
                    throw new UserExistsException();
                }
                //else if (request.Role == UserRole.Admin)
                //{
                //    throw new AdminRegistrationException();
                //}
                else
                {
                    var newUser = new UserModel
                    {
                        Id = Guid.NewGuid(),
                        UserName = request.UserName,
                        Email = request.Email,
                        Password = request.Password == null ? BCrypt.Net.BCrypt.HashPassword("")
                            : BCrypt.Net.BCrypt.HashPassword(request.Password),
                        Name = request.Name,
                        LastName = request.LastName,
                        Birthday = request.Birthday == null ? "" : request.Birthday,
                        Address = request.Address == null ? "" : request.Address,
                        Role = request.Role,
                    };
                    if (request.Image != null)
                    {
                        var imageFileName = $"{request.Image.FileName}";
                        var imagePath = Path.Combine("wwwroot", "images", imageFileName);

                        using (var imageStream = new FileStream(imagePath, FileMode.Create))
                        {
                            await request.Image.CopyToAsync(imageStream);
                        }
                        newUser.Image = imageFileName;
                    }
                    else
                    {
                        newUser.Image = "";
                    }


                    if (newUser.Role == UserRole.Driver)
                    {
                        newUser.VerificationState = VerificationState.Processing;
                    }

                    await proxy.Register(newUser);
                }
                return Unit.Value;
            }
        }
    }
}
