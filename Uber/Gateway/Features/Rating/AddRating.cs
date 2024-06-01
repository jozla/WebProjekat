using Common.Models;
using Communication;
using FluentValidation;
using Gateway.CQRS;
using MediatR;
using Microsoft.ServiceFabric.Services.Client;
using Microsoft.ServiceFabric.Services.Remoting.Client;

namespace Gateway.Features.Rating
{
    public class AddRating
    {
        public record AddRatingCommand(Guid UserId, int Rating) : ICommand;
        public class Validator : AbstractValidator<AddRatingCommand>
        {
            public Validator()
            {
                RuleFor(entity => entity.UserId).NotEmpty().WithMessage("Userd id is required");
                RuleFor(entity => entity.Rating).NotEmpty().WithMessage("Rating is required");
            }
        }
        public class CommandHandler : ICommandHandler<AddRatingCommand>
        {
            private readonly IConfiguration _configuration;

            public CommandHandler(IConfiguration configuration)
            {
                _configuration = configuration;
            }
            public async Task<Unit> Handle(AddRatingCommand request, CancellationToken cancellationToken)
            {
                var proxy = ServiceProxy.Create<IRatingStatefulCommunication>(
                   new Uri(_configuration.GetValue<string>("ProxyUrls:RatingStateful")!), new ServicePartitionKey(3));

                var proxyStateless = ServiceProxy.Create<IRatingStatelessCommunication>(
                   new Uri(_configuration.GetValue<string>("ProxyUrls:RatingStateless")!));

                var existingRating = await proxy.GetRating(request.UserId);

                if (existingRating != null)
                {
                    var newRating = await proxyStateless.CalculateNewRating(
                        existingRating.NumOfRates, existingRating.Rating, request.Rating);

                    existingRating.Rating = newRating;
                    existingRating.NumOfRates++;
                    await proxy.UpdateRating(existingRating);
                }
                else
                {
                    var rating = new RatingModel
                    {
                        Id = Guid.NewGuid(),
                        NumOfRates = 1,
                        Rating = request.Rating,
                        UserId = request.UserId,
                    };

                    await proxy.AddRating(rating);
                }

                return Unit.Value;
            }
        }
    }
}
