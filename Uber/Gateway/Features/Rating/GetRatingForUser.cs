using Communication;
using Gateway.CQRS;
using Gateway.Validation;
using Microsoft.ServiceFabric.Services.Client;
using Microsoft.ServiceFabric.Services.Remoting.Client;

namespace Gateway.Features.Rating
{
    public class GetRatingForUser
    {
        public record GetRatingForUserQuery(Guid UserId) : IQuery<GetRatingForUserResponse>;
        public record GetRatingForUserResponse(double? Rating);

        public class QueryHandler : IQueryHandler<GetRatingForUserQuery, GetRatingForUserResponse>
        {
            private readonly IConfiguration _configuration;

            public QueryHandler(IConfiguration configuration)
            {
                _configuration = configuration;
            }
            public async Task<GetRatingForUserResponse> Handle(GetRatingForUserQuery request, CancellationToken cancellationToken)
            {
                var proxy = ServiceProxy.Create<IRatingStatefulCommunication>(
                    new Uri(_configuration.GetValue<string>("ProxyUrls:RatingStateful")!), new ServicePartitionKey(3));

                var rating = await proxy.GetRating(request.UserId);

                if (rating == null)
                {
                    throw new EntityNotFoundException();
                }

                return new GetRatingForUserResponse(rating.Rating);
            }
        }
    }
}
