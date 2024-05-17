using Common.DTOs;
using Communication;
using Gateway.CQRS;
using Microsoft.ServiceFabric.Services.Client;
using Microsoft.ServiceFabric.Services.Remoting.Client;

namespace Gateway.Features.Ride
{
    public class GetRideById
    {
        public record GetRideByIdQuery(Guid Id) : IQuery<GetRideByIdResponse>;
        public record GetRideByIdResponse(GetRideDto Ride);

        public class QueryHandler : IQueryHandler<GetRideByIdQuery, GetRideByIdResponse>
        {
            public async Task<GetRideByIdResponse> Handle(GetRideByIdQuery request, CancellationToken cancellationToken)
            {
                var proxy = ServiceProxy.Create<IRideStatefulCommunication>(
                    new Uri("fabric:/Uber/RideStateful"), new ServicePartitionKey(2));

                var ride = await proxy.GetRideById(request.Id);

                var rideDto = new GetRideDto
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
                };

                return new GetRideByIdResponse(rideDto);
            }
        }
    }
}
