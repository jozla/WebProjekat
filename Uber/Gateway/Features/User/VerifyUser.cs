using Common.Enums;
using Communication;
using FluentValidation;
using Gateway.CQRS;
using Gateway.Helpers.Mail;
using Gateway.Validation;
using MediatR;
using Microsoft.ServiceFabric.Services.Client;
using Microsoft.ServiceFabric.Services.Remoting.Client;

namespace Gateway.Features.User
{
    public class VerifyUser
    {
        public record VerifyUserCommand(Guid Id) : ICommand;
        public class Validator : AbstractValidator<VerifyUserCommand>
        {
            public Validator()
            {
                RuleFor(entity => entity.Id).NotEmpty().WithMessage("Id is required");
            }
        }
        public class CommandHandler : ICommandHandler<VerifyUserCommand>
        {
            private readonly IConfiguration _configuration;
            private readonly IEmailSender _emailSender;

            public CommandHandler(IConfiguration configuration, IEmailSender emailSender)
            {
                _configuration = configuration;
                _emailSender = emailSender;
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
                    _emailSender.SendEmail(existingUser.Email);
                }

                return Unit.Value;
            }
        }
    }
}
