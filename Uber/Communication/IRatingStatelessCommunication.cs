using Microsoft.ServiceFabric.Services.Remoting;

namespace Communication
{
    public interface IRatingStatelessCommunication : IService
    {
        Task AddRating(Guid userId, double rating);
        Task<double?> GetRating(Guid userId);
    }
}
