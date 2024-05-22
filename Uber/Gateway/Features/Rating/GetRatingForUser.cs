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
            private readonly IConfiguration _configuration;

            public QueryHandler(IConfiguration configuration)
            {
                _configuration = configuration;
            }
            public async Task<GetRatingForUserResponse> Handle(GetRatingForUserQuery request, CancellationToken cancellationToken)
            {
                var proxy = ServiceProxy.Create<IRatingStatelessCommunication>(
                    new Uri(_configuration.GetValue<string>("ProxyUrls:RatingStateless")!));

                var rating = await proxy.GetRating(request.UserId);

                return new GetRatingForUserResponse(rating);
            }
        }
    }
}
