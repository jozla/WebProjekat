using Common.Models;
using Communication;
using Gateway.CQRS;
using Microsoft.ServiceFabric.Services.Client;
using Microsoft.ServiceFabric.Services.Remoting.Client;

namespace Gateway.Features.User
{
    public class GetUserById
    {
        public record GetUserByIdCommand(Guid Id) : IQuery<GetUserByIdResponse>;
        public record GetUserByIdResponse(UserModel user);


        public class QueryHandler : IQueryHandler<GetUserByIdCommand, GetUserByIdResponse>
        {
            public async Task<GetUserByIdResponse> Handle(GetUserByIdCommand request, CancellationToken cancellationToken)
            {
                var proxy = ServiceProxy.Create<IUserStatefullCommunication>(
                    new Uri("fabric:/Uber/UserStatefull"), new ServicePartitionKey(1));

                var existingUser = await proxy.GetUserById(request.Id);

                if (existingUser != null)
                {
                    return new GetUserByIdResponse(existingUser);
                }

                return null;
            }
        }
    }
}
