using Common.DTOs;
using Communication;
using FluentValidation;
using Gateway.CQRS;
using Gateway.Validation;
using Microsoft.ServiceFabric.Services.Client;
using Microsoft.ServiceFabric.Services.Remoting.Client;

namespace Gateway.Features.Ride
{
    public class GetConfirmedRide
    {
        public record GetConfirmedRideQuery(Guid UserId) : IQuery<GetConfirmedRideResponse>;
        public record GetConfirmedRideResponse(GetRideDto Ride);

        public class Validator : AbstractValidator<GetConfirmedRideQuery>
        {
            public Validator()
            {
                RuleFor(entity => entity.UserId).NotEmpty().WithMessage("User id is required");
            }
        }

        public class CommandHandler : IQueryHandler<GetConfirmedRideQuery, GetConfirmedRideResponse>
        {
            private readonly IConfiguration _configuration;

            public CommandHandler(IConfiguration configuration)
            {
                _configuration = configuration;
            }
            public async Task<GetConfirmedRideResponse> Handle(GetConfirmedRideQuery request, CancellationToken cancellationToken)
            {
                var proxy = ServiceProxy.Create<IRideStatefulCommunication>(
                    new Uri(_configuration.GetValue<string>("ProxyUrls:RideStateful")!), new ServicePartitionKey(2));

                var ride = await proxy.GetConfirmedRide(request.UserId);

                if (ride == null)
                {
                    throw new EntityNotFoundException();
                }

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

                return new GetConfirmedRideResponse(rideDto);
            }
        }
    }
}
