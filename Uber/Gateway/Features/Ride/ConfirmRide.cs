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
    public class ConfirmRide
    {
        public record ConfirmRideCommand(Guid DriverId, Guid PassengerId, Guid RideId, int ArrivalTimeInSeconds) : ICommand;

        public class Validator : AbstractValidator<ConfirmRideCommand>
        {
            public Validator()
            {
                RuleFor(entity => entity.DriverId).NotEmpty().WithMessage("Driver id is required");
                RuleFor(entity => entity.PassengerId).NotEmpty().WithMessage("Driver id is required");
                RuleFor(entity => entity.RideId).NotEmpty().WithMessage("Ride id is required");
                RuleFor(entity => entity.ArrivalTimeInSeconds).NotEmpty().WithMessage("Arrival time is required");
            }
        }

        public class CommandHandler : ICommandHandler<ConfirmRideCommand>
        {
            private readonly IConfiguration _configuration;

            public CommandHandler(IConfiguration configuration)
            {
                _configuration = configuration;
            }
            public async Task<Unit> Handle(ConfirmRideCommand request, CancellationToken cancellationToken)
            {
                var proxy = ServiceProxy.Create<IRideStatefulCommunication>(
                    new Uri(_configuration.GetValue<string>("ProxyUrls:RideStateful")!), new ServicePartitionKey(2));

                var existingRide = await proxy.GetRideById(request.RideId);

                if (existingRide == null)
                {
                    throw new EntityNotFoundException();
                }

                if (existingRide.Status != RideStatus.Pending)
                {
                    throw new RideConfirmedException();
                }

                existingRide.DriverId = request.DriverId;
                existingRide.ArrivalTimeInSeconds = request.ArrivalTimeInSeconds;
                existingRide.Status = RideStatus.Confirmed;
                await proxy.UpdateRide(existingRide);

                return Unit.Value;
            }
        }
    }
}
