﻿using Common.DTOs;
using Communication;
using FluentValidation;
using Gateway.CQRS;
using Microsoft.ServiceFabric.Services.Client;
using Microsoft.ServiceFabric.Services.Remoting.Client;

namespace Gateway.Features.Ride
{
    public class GetRidesForDriver
    {
        public record GetRidesForDriverQuery(Guid DriverId) : IQuery<GetRidesForDriverResponse>;
        public record GetRidesForDriverResponse(IEnumerable<GetRideDto> Rides);

        public class Validator : AbstractValidator<GetRidesForDriverQuery>
        {
            public Validator()
            {
                RuleFor(entity => entity.DriverId).NotEmpty().WithMessage("Driver id is required");
            }
        }
        public class CommandHandler : IQueryHandler<GetRidesForDriverQuery, GetRidesForDriverResponse>
        {
            private readonly IConfiguration _configuration;

            public CommandHandler(IConfiguration configuration)
            {
                _configuration = configuration;
            }
            public async Task<GetRidesForDriverResponse> Handle(GetRidesForDriverQuery request, CancellationToken cancellationToken)
            {
                var proxy = ServiceProxy.Create<IRideStatefulCommunication>(
                    new Uri(_configuration.GetValue<string>("ProxyUrls:RideStateful")!), new ServicePartitionKey(2));

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
