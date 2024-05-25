using Common.DTOs;
using Communication;
using FluentValidation;
using Gateway.CQRS;
using Microsoft.ServiceFabric.Services.Client;
using Microsoft.ServiceFabric.Services.Remoting.Client;

namespace Gateway.Features.Ride
{
    public class GetRidesForUser
    {
        public record GetRidesForUserQuery(Guid UserId) : IQuery<GetRidesForUserResponse>;
        public record GetRidesForUserResponse(IEnumerable<GetRideDto> Rides);

        public class Validator : AbstractValidator<GetRidesForUserQuery>
        {
            public Validator()
            {
                RuleFor(entity => entity.UserId).NotEmpty().WithMessage("User id is required");
            }
        }
        public class CommandHandler : IQueryHandler<GetRidesForUserQuery, GetRidesForUserResponse>
        {
            private readonly IConfiguration _configuration;

            public CommandHandler(IConfiguration configuration)
            {
                _configuration = configuration;
            }
            public async Task<GetRidesForUserResponse> Handle(GetRidesForUserQuery request, CancellationToken cancellationToken)
            {
                var proxy = ServiceProxy.Create<IRideStatefulCommunication>(
                    new Uri(_configuration.GetValue<string>("ProxyUrls:RideStateful")!), new ServicePartitionKey(2));

                var rides = await proxy.GetRidesForUser(request.UserId);

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

                return new GetRidesForUserResponse(ridesDto);
            }
        }
    }
}
