using Microsoft.ServiceFabric.Services.Remoting;

namespace Communication
{
    public interface IRatingStatelessCommunication : IService
    {
        Task<double> CalculateNewRating(int numOfRates, double rating, int newRating);
    }
}
