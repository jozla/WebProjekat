using Common.Models;
using Microsoft.ServiceFabric.Services.Remoting;

namespace Communication
{
    public interface IUserStatefulCommunication : IService
    {
        Task Register(UserModel user);
        Task<UserModel> GetUserByEmail(string email);
        Task<UserModel> GetUserById(Guid id);
        Task<IEnumerable<UserModel>> GetAllUsers();
        Task UpdateUser(UserModel user);
    }
}
