using Common.Enums;
using Communication;
using FluentValidation;
using Gateway.CQRS;
using Gateway.Validation;
using MediatR;
using Microsoft.ServiceFabric.Services.Client;
using Microsoft.ServiceFabric.Services.Remoting.Client;

namespace Gateway.Features.Ride
{
    public class FinishRide
    {
        public record FinishRideCommand(Guid RideId) : ICommand;
        public class Validator : AbstractValidator<FinishRideCommand>
        {
            public Validator()
            {
                RuleFor(entity => entity.RideId).NotEmpty().WithMessage("Ride id is required");
            }
        }
        public class CommandHandler : ICommandHandler<FinishRideCommand>
        {
            private readonly IConfiguration _configuration;

            public CommandHandler(IConfiguration configuration)
            {
                _configuration = configuration;
            }
            public async Task<Unit> Handle(FinishRideCommand request, CancellationToken cancellationToken)
            {
                var proxy = ServiceProxy.Create<IRideStatefulCommunication>(
                    new Uri(_configuration.GetValue<string>("ProxyUrls:RideStateful")!), new ServicePartitionKey(2));

                var existingRide = await proxy.GetRideById(request.RideId);

                if (existingRide == null)
                {
                    throw new EntityNotFoundException();
                }

                if (existingRide.Status == RideStatus.Pending)
                {
                    throw new RideNotConfirmedException();
                }

                existingRide.Status = RideStatus.Finished;
                await proxy.UpdateRide(existingRide);

                return Unit.Value;
            }
        }
    }
}
