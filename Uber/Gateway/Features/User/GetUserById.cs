using Common.DTOs;
using Communication;
using Gateway.CQRS;
using Microsoft.ServiceFabric.Services.Client;
using Microsoft.ServiceFabric.Services.Remoting.Client;

namespace Gateway.Features.User
{
    public class GetUserById
    {
        public record GetUserByIdCommand(Guid Id) : IQuery<GetUserByIdResponse>;
        public record GetUserByIdResponse(GetUserDto user);


        public class QueryHandler : IQueryHandler<GetUserByIdCommand, GetUserByIdResponse>
        {
            public async Task<GetUserByIdResponse> Handle(GetUserByIdCommand request, CancellationToken cancellationToken)
            {
                var proxy = ServiceProxy.Create<IUserStatefullCommunication>(
                    new Uri("fabric:/Uber/UserStatefull"), new ServicePartitionKey(1));

                var existingUser = await proxy.GetUserById(request.Id);

                if (existingUser != null)
                {
                    var userDto = new GetUserDto
                    {
                        Id = existingUser.Id,
                        UserName = existingUser.UserName,
                        Email = existingUser.Email,
                        Name = existingUser.Name,
                        LastName = existingUser.LastName,
                        Birthday = existingUser.Birthday,
                        Address = existingUser.Address,
                        Role = existingUser.Role,
                        Image = existingUser.Image,
                        VerificationState = existingUser.VerificationState
                    };
                    return new GetUserByIdResponse(userDto);
                }

                return null;
            }
        }
    }
}
