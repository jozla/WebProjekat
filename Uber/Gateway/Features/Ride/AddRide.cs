using Common.Models;
using Communication;
using FluentValidation;
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

        public class Validator : AbstractValidator<AddRideCommand>
        {
            public Validator()
            {
                RuleFor(entity => entity.StartingPoint).NotEmpty().WithMessage("Starting point is required");
                RuleFor(entity => entity.EndingPoint).NotEmpty().WithMessage("Ending point is required");
                RuleFor(entity => entity.Price).NotEmpty().WithMessage("Price is required");
                RuleFor(entity => entity.DriverTimeInSeconds).NotEmpty().WithMessage("Driver time is required");
                RuleFor(entity => entity.PassengerId).NotEmpty().WithMessage("Passenger id is required");
            }
        }

        public class CommandHandler : ICommandHandler<AddRideCommand>
        {
            private readonly IConfiguration _configuration;

            public CommandHandler(IConfiguration configuration)
            {
                _configuration = configuration;
            }
            public async Task<Unit> Handle(AddRideCommand request, CancellationToken cancellationToken)
            {
                var proxy = ServiceProxy.Create<IRideStatefulCommunication>(
                    new Uri(_configuration.GetValue<string>("ProxyUrls:RideStateful")!), new ServicePartitionKey(2));

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
