using Common.DTOs;
using Communication;
using Gateway.CQRS;
using Microsoft.ServiceFabric.Services.Client;
using Microsoft.ServiceFabric.Services.Remoting.Client;

namespace Gateway.Features.Ride
{
    public class GetAllRides
    {
        public record GetAllRidesQuery() : IQuery<GetAllRidesResponse>;
        public record GetAllRidesResponse(IEnumerable<GetRideDto> Rides);

        public class QueryHandler : IQueryHandler<GetAllRidesQuery, GetAllRidesResponse>
        {
            public async Task<GetAllRidesResponse> Handle(GetAllRidesQuery request, CancellationToken cancellationToken)
            {
                var proxy = ServiceProxy.Create<IRideStatefulCommunication>(
                     new Uri("fabric:/Uber/RideStateful"), new ServicePartitionKey(2));

                var rides = await proxy.GetAllRides();

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

                return new GetAllRidesResponse(ridesDto);
            }
        }
    }
}
