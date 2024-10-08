﻿using Common.DTOs;
using Communication;
using FluentValidation;
using Gateway.CQRS;
using Gateway.Validation;
using Microsoft.ServiceFabric.Services.Client;
using Microsoft.ServiceFabric.Services.Remoting.Client;

namespace Gateway.Features.Ride
{
    public class GetRideById
    {
        public record GetRideByIdQuery(Guid Id) : IQuery<GetRideByIdResponse>;
        public record GetRideByIdResponse(GetRideDto Ride);
        public class Validator : AbstractValidator<GetRideByIdQuery>
        {
            public Validator()
            {
                RuleFor(entity => entity.Id).NotEmpty().WithMessage("Id is required");
            }
        }
        public class QueryHandler : IQueryHandler<GetRideByIdQuery, GetRideByIdResponse>
        {
            private readonly IConfiguration _configuration;

            public QueryHandler(IConfiguration configuration)
            {
                _configuration = configuration;
            }
            public async Task<GetRideByIdResponse> Handle(GetRideByIdQuery request, CancellationToken cancellationToken)
            {
                var proxy = ServiceProxy.Create<IRideStatefulCommunication>(
                    new Uri(_configuration.GetValue<string>("ProxyUrls:RideStateful")!), new ServicePartitionKey(2));

                var ride = await proxy.GetRideById(request.Id);

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

                return new GetRideByIdResponse(rideDto);
            }
        }
    }
}
