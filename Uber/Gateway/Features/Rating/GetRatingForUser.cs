using Communication;
using Gateway.CQRS;
using Microsoft.ServiceFabric.Services.Remoting.Client;

namespace Gateway.Features.Rating
{
    public class GetRatingForUser
    {
        public record GetRatingForUserQuery(Guid UserId) : IQuery<GetRatingForUserResponse>;
        public record GetRatingForUserResponse(double Rating);

        public class QueryHandler : IQueryHandler<GetRatingForUserQuery, GetRatingForUserResponse>
        {
            public async Task<GetRatingForUserResponse> Handle(GetRatingForUserQuery request, CancellationToken cancellationToken)
            {
                var proxy = ServiceProxy.Create<IRatingStatelessCommunication>(
                    new Uri("fabric:/Uber/RatingStateless"));

                var rating = await proxy.GetRating(request.UserId);

                return new GetRatingForUserResponse(rating);
            }
        }
    }
}
