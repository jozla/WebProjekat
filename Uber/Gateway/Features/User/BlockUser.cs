﻿using Common.Enums;
using Communication;
using FluentValidation;
using Gateway.CQRS;
using Gateway.Validation;
using MediatR;
using Microsoft.ServiceFabric.Services.Client;
using Microsoft.ServiceFabric.Services.Remoting.Client;

namespace Gateway.Features.User
{
    public class BlockUser
    {
        public record BlockUserCommand(Guid Id) : ICommand;

        public class Validator : AbstractValidator<BlockUserCommand>
        {
            public Validator()
            {
                RuleFor(entity => entity.Id).NotEmpty().WithMessage("Id is required");
            }
        }

        public class CommandHandler : ICommandHandler<BlockUserCommand>
        {
            private readonly IConfiguration _configuration;

            public CommandHandler(IConfiguration configuration)
            {
                _configuration = configuration;
            }
            public async Task<Unit> Handle(BlockUserCommand request, CancellationToken cancellationToken)
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
                    existingUser.VerificationState = VerificationState.Unverified;
                    await proxy.UpdateUser(existingUser);
                }

                return Unit.Value;
            }
        }
    }
}
