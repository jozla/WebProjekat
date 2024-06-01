using Common.Models;
using Microsoft.ServiceFabric.Services.Remoting;

namespace Communication;
public interface IRatingStatefulCommunication : IService
{
    Task AddRating(RatingModel rating);
    Task<RatingModel> GetRating(Guid userId);
    Task UpdateRating(RatingModel rating);
}
