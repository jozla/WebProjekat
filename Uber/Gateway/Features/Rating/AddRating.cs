using Communication;
using Gateway.CQRS;
using MediatR;
using Microsoft.ServiceFabric.Services.Remoting.Client;

namespace Gateway.Features.Rating
{
    public class AddRating
    {
        public record AddRatingCommand(Guid UserId, int Rating) : ICommand;

        public class CommandHandler : ICommandHandler<AddRatingCommand>
        {
            public async Task<Unit> Handle(AddRatingCommand request, CancellationToken cancellationToken)
            {
                var proxy = ServiceProxy.Create<IRatingStatelessCommunication>(
                   new Uri("fabric:/Uber/RatingStateless"));

                await proxy.AddRating(request.UserId, request.Rating);

                return Unit.Value;
            }
        }
    }
}
