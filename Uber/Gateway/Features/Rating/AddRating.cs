using Communication;
using FluentValidation;
using Gateway.CQRS;
using MediatR;
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
                var proxy = ServiceProxy.Create<IRatingStatelessCommunication>(
                   new Uri(_configuration.GetValue<string>("ProxyUrls:RatingStateless")!));

                await proxy.AddRating(request.UserId, request.Rating);

                return Unit.Value;
            }
        }
    }
}
