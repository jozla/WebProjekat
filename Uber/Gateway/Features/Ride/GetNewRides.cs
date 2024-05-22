using Common.DTOs;
using Communication;
using Gateway.CQRS;
using Microsoft.ServiceFabric.Services.Client;
using Microsoft.ServiceFabric.Services.Remoting.Client;

namespace Gateway.Features.Ride
{
    public class GetNewRides
    {
        public record GetNewRidesQuery() : IQuery<GetNewRidesResponse>;
        public record GetNewRidesResponse(IEnumerable<GetRideDto> Rides);

        public class QueryHandler : IQueryHandler<GetNewRidesQuery, GetNewRidesResponse>
        {
            private readonly IConfiguration _configuration;

            public QueryHandler(IConfiguration configuration)
            {
                _configuration = configuration;
            }
            public async Task<GetNewRidesResponse> Handle(GetNewRidesQuery request, CancellationToken cancellationToken)
            {
                var proxy = ServiceProxy.Create<IRideStatefulCommunication>(
                   new Uri(_configuration.GetValue<string>("ProxyUrls:RideStateful")!), new ServicePartitionKey(2));

                var newRides = await proxy.GetNewRides();

                var newRidesDto = newRides.Select(ride => new GetRideDto
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

                return new GetNewRidesResponse(newRidesDto);
            }
        }
    }
}
