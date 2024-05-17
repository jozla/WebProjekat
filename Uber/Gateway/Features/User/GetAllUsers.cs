using Common.DTOs;
using Communication;
using Gateway.CQRS;
using Microsoft.ServiceFabric.Services.Client;
using Microsoft.ServiceFabric.Services.Remoting.Client;

namespace Gateway.Features.User
{
    public class GetAllUsers
    {
        public record GetAllUsersQuery() : IQuery<GetAllUsersResponse>;
        public record GetAllUsersResponse(IEnumerable<GetUserDto> Users);

        public class QueryHandler : IQueryHandler<GetAllUsersQuery, GetAllUsersResponse>
        {
            public async Task<GetAllUsersResponse> Handle(GetAllUsersQuery request, CancellationToken cancellationToken)
            {
                var proxy = ServiceProxy.Create<IUserStatefulCommunication>(
                    new Uri("fabric:/Uber/UserStatefull"), new ServicePartitionKey(1));

                var users = await proxy.GetAllUsers();


                var userDtos = users.Select(user => new GetUserDto
                {
                    Id = user.Id,
                    UserName = user.UserName,
                    Email = user.Email,
                    Name = user.Name,
                    LastName = user.LastName,
                    Birthday = user.Birthday,
                    Address = user.Address,
                    Role = user.Role,
                    Image = user.Image,
                    VerificationState = user.VerificationState
                });

                return new GetAllUsersResponse(userDtos);
            }
        }
    }
}
