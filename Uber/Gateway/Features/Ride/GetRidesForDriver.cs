using Common.DTOs;
using Communication;
using Gateway.CQRS;
using Microsoft.ServiceFabric.Services.Client;
using Microsoft.ServiceFabric.Services.Remoting.Client;

namespace Gateway.Features.Ride
{
    public class GetRidesForDriver
    {
        public record GetRidesForDriverQuery(Guid DriverId) : IQuery<GetRidesForDriverResponse>;
        public record GetRidesForDriverResponse(IEnumerable<GetRideDto> Rides);

        public class CommandHandler : IQueryHandler<GetRidesForDriverQuery, GetRidesForDriverResponse>
        {
            public async Task<GetRidesForDriverResponse> Handle(GetRidesForDriverQuery request, CancellationToken cancellationToken)
            {
                var proxy = ServiceProxy.Create<IRideStatefulCommunication>(
                    new Uri("fabric:/Uber/RideStateful"), new ServicePartitionKey(2));

                var rides = await proxy.GetRidesForDriver(request.DriverId);

                var ridesDto = rides.Select(ride => new GetRideDto
                {
                    Id = ride.Id,
                    StartingPoint = ride.StartingPoint,
                    EndingPoint = ride.EndingPoint,
                    Price = ride.Price,
                    DriverTimeInSeconds = ride.DriverTimeInSeconds,
                    PassengerId = ride.PassengerId,
                    DriverId = ride.DriverId,
                    ArrivalTimeInSeconds = ride.ArrivalTimeInSeconds,
                    Status = ride.Status
                });

                return new GetRidesForDriverResponse(ridesDto);
            }
        }
    }
}
