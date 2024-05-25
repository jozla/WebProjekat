using Common.DTOs;
using Communication;
using FluentValidation;
using Gateway.CQRS;
using Gateway.Validation;
using Microsoft.ServiceFabric.Services.Client;
using Microsoft.ServiceFabric.Services.Remoting.Client;

namespace Gateway.Features.User
{
    public class GetUserById
    {
        public record GetUserByIdQuery(Guid Id) : IQuery<GetUserByIdResponse>;
        public record GetUserByIdResponse(GetUserDto user);

        public class Validator : AbstractValidator<GetUserByIdQuery>
        {
            public Validator()
            {
                RuleFor(entity => entity.Id).NotEmpty().WithMessage("Id is required");
            }
        }
        public class QueryHandler : IQueryHandler<GetUserByIdQuery, GetUserByIdResponse>
        {
            private readonly IConfiguration _configuration;

            public QueryHandler(IConfiguration configuration)
            {
                _configuration = configuration;
            }
            public async Task<GetUserByIdResponse> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
            {
                var proxy = ServiceProxy.Create<IUserStatefulCommunication>(
                    new Uri(_configuration.GetValue<string>("ProxyUrls:UserStateful")!), new ServicePartitionKey(1));

                var existingUser = await proxy.GetUserById(request.Id);

                if (existingUser == null)
                {
                    throw new EntityNotFoundException();
                }
                else
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
            }
        }
    }
}
