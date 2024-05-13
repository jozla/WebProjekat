using Common.Models;
using Communication;
using Gateway.CQRS;
using Microsoft.ServiceFabric.Services.Client;
using Microsoft.ServiceFabric.Services.Remoting.Client;

namespace Gateway.Features.User
{
    public class GetAllUsers
    {
        public record GetAllUsersQuery() : IQuery<GetAllUsersResponse>;
        public record GetAllUsersResponse(IEnumerable<UserModel> Users);

        public class QueryHandler : IQueryHandler<GetAllUsersQuery, GetAllUsersResponse>
        {
            public async Task<GetAllUsersResponse> Handle(GetAllUsersQuery request, CancellationToken cancellationToken)
            {
                var proxy = ServiceProxy.Create<IUserStatefullCommunication>(
                    new Uri("fabric:/Uber/UserStatefull"), new ServicePartitionKey(1));

                var users = await proxy.GetAllUsers();

                return new GetAllUsersResponse(users);
            }
        }
    }
}
