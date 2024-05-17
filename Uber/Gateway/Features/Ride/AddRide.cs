using Common.Models;
using Communication;
using Gateway.CQRS;
using MediatR;
using Microsoft.ServiceFabric.Services.Client;
using Microsoft.ServiceFabric.Services.Remoting.Client;

namespace Gateway.Features.Ride
{
    public class AddRide
    {
        public record AddRideCommand(
                Guid Id, string StartingPoint, string EndingPoint, int Price, int DriverTimeInSeconds,
                Guid PassengerId) : ICommand;

        public class CommandHandler : ICommandHandler<AddRideCommand>
        {
            public async Task<Unit> Handle(AddRideCommand request, CancellationToken cancellationToken)
            {
                var proxy = ServiceProxy.Create<IRideStatefulCommunication>(
                    new Uri("fabric:/Uber/RideStateful"), new ServicePartitionKey(2));

                var newRide = new RideModel
                {
                    Id = Guid.NewGuid(),
                    StartingPoint = request.StartingPoint,
                    EndingPoint = request.EndingPoint,
                    Price = request.Price,
                    DriverTimeInSeconds = request.DriverTimeInSeconds,
                    PassengerId = request.PassengerId,
                    DriverId = Guid.Empty
                };

                await proxy.AddRide(newRide);

                return Unit.Value;
            }
        }
    }
}
