using Common.Models;
using Microsoft.ServiceFabric.Services.Remoting;

namespace Communication
{
    public interface IRideStatefulCommunication : IService
    {
        Task AddRide(RideModel ride);
        Task<RideModel> GetRideById(Guid rideId);
        Task UpdateRide(RideModel ride);
        Task<IEnumerable<RideModel>> GetAllRides();
        Task<IEnumerable<RideModel>> GetNewRides();
        Task<IEnumerable<RideModel>> GetRidesForDriver(Guid driverId);
        Task<IEnumerable<RideModel>> GetRidesForUser(Guid userId);
        Task<RideModel> GetConfirmedRide(Guid userId);
    }
}
