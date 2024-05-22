using Common.Enums;
using Communication;
using Gateway.CQRS;
using MediatR;
using Microsoft.ServiceFabric.Services.Client;
using Microsoft.ServiceFabric.Services.Remoting.Client;

namespace Gateway.Features.Ride
{
    public class ConfirmRide
    {
        public record ConfirmRideCommand(Guid DriverId, Guid RideId, int ArrivalTimeInSeconds) : ICommand;

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

                existingRide.DriverId = request.DriverId;
                existingRide.ArrivalTimeInSeconds = request.ArrivalTimeInSeconds;
                existingRide.Status = RideStatus.Confirmed;
                await proxy.UpdateRide(existingRide);

                return Unit.Value;
            }
        }
    }
}
