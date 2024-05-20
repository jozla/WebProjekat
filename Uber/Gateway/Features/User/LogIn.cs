using Common.Models;
using Communication;
using Gateway.CQRS;
using Microsoft.IdentityModel.Tokens;
using Microsoft.ServiceFabric.Services.Client;
using Microsoft.ServiceFabric.Services.Remoting.Client;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Gateway.Features.User
{
    public class LogIn
    {
        public record LogInCommand(string Email, string Password) : ICommand<LogInResponse>;
        public record LogInResponse(string Token);

        public class CommandHandler : ICommandHandler<LogInCommand, LogInResponse>
        {
            private readonly IConfiguration _configuration;

            public CommandHandler(IConfiguration configuration)
            {
                _configuration = configuration;
            }

            public async Task<LogInResponse> Handle(LogInCommand request, CancellationToken cancellationToken)
            {
                var proxy = ServiceProxy.Create<IUserStatefulCommunication>(
                             new Uri("fabric:/Uber/UserStatefull"), new ServicePartitionKey(1));

                var existingUser = await proxy.GetUserByEmail(request.Email);
                if (existingUser != null &&
                     BCrypt.Net.BCrypt.Verify(request.Password, existingUser.Password))
                {
                    var token = GenerateToken(existingUser, _configuration);
                    return new LogInResponse(token);
                }

                return null;
            }

            /// Helper for token generating
            private string GenerateToken(UserModel user, IConfiguration configuration)
            {
                //Generate token that is valid for 7 days
                var tokenHandler = new JwtSecurityTokenHandler();

                var securityKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(configuration["Jwt:Key"]!));
                var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
                var userClaims = new[]
                {
                new Claim("user_id", user.Id.ToString()),
                new Claim("user_role", user.Role.ToString()),
                new Claim("verification", user.VerificationState.ToString()),
                };

                var token = new JwtSecurityToken(
                    issuer: configuration["Jwt:Issuer"],
                    audience: configuration["Jwt:Audience"],
                    claims: userClaims,
                    signingCredentials: credentials,
                    expires: DateTime.Now.AddDays(5)
                );

                return tokenHandler.WriteToken(token);
            }
        }
    }
}
