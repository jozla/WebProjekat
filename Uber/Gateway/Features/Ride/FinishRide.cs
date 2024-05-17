using Common.Enums;
using Communication;
using Gateway.CQRS;
using MediatR;
using Microsoft.ServiceFabric.Services.Client;
using Microsoft.ServiceFabric.Services.Remoting.Client;

namespace Gateway.Features.Ride
{
    public class FinishRide
    {
        public record FinishRideCommand(Guid RideId) : ICommand;

        public class CommandHandler : ICommandHandler<FinishRideCommand>
        {
            public async Task<Unit> Handle(FinishRideCommand request, CancellationToken cancellationToken)
            {
                var proxy = ServiceProxy.Create<IRideStatefulCommunication>(
                    new Uri("fabric:/Uber/RideStateful"), new ServicePartitionKey(2));

                var existingRide = await proxy.GetRideById(request.RideId);

                existingRide.Status = RideStatus.Finished;
                await proxy.UpdateRide(existingRide);

                return Unit.Value;
            }
        }
    }
}
