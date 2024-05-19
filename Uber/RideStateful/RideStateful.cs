using Common.Enums;
using Common.Models;
using Communication;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.ServiceFabric.Data.Collections;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Remoting.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;
using System.Fabric;

namespace RideStateful
{
    /// <summary>
    /// An instance of this class is created for each service replica by the Service Fabric runtime.
    /// </summary>
    internal sealed class RideStateful : StatefulService, IRideStatefulCommunication
    {
        private RidesDbContext _myDbContext;
        public RideStateful(StatefulServiceContext context, IServiceProvider provider)
            : base(context)
        {
            _myDbContext = provider.GetService<RidesDbContext>()!;
        }

        #region RideMethods
        public async Task AddRide(RideModel ride)
        {
            var stateManager = this.StateManager;
            var ridesDict = await stateManager.GetOrAddAsync<IReliableDictionary<Guid, RideModel>>("ridesDict");

            using (var transaction = stateManager.CreateTransaction())
            {
                await ridesDict.AddOrUpdateAsync(transaction, ride.Id, ride, (k, v) => v);
                await transaction.CommitAsync();
            }
        }

        public async Task<IEnumerable<RideModel>> GetAllRides()
        {
            var stateManager = this.StateManager;
            var ridesDict = await stateManager.GetOrAddAsync<IReliableDictionary<Guid, RideModel>>("ridesDict");

            var rides = new List<RideModel>();

            using (var transaction = stateManager.CreateTransaction())
            {
                var enumerable = await ridesDict.CreateEnumerableAsync(transaction);
                var enumerator = enumerable.GetAsyncEnumerator();

                while (await enumerator.MoveNextAsync(default))
                {
                    rides.Add(enumerator.Current.Value);
                }
            }

            return rides;
        }

        public async Task<RideModel> GetConfirmedRide(Guid userId)
        {
            var stateManager = this.StateManager;
            var ridesDict = await stateManager.GetOrAddAsync<IReliableDictionary<Guid, RideModel>>("ridesDict");

            using (var transaction = stateManager.CreateTransaction())
            {
                var enumerable = await ridesDict.CreateEnumerableAsync(transaction);
                var enumerator = enumerable.GetAsyncEnumerator();

                while (await enumerator.MoveNextAsync(CancellationToken.None))
                {
                    var currentRide = enumerator.Current.Value;
                    if ((currentRide.PassengerId == userId || currentRide.DriverId == userId)
                        && currentRide.Status == RideStatus.Confirmed)
                    {
                        return currentRide;
                    }
                }
            }

            return null;
        }

        public async Task<IEnumerable<RideModel>> GetNewRides()
        {
            var stateManager = this.StateManager;
            var ridesDict = await stateManager.GetOrAddAsync<IReliableDictionary<Guid, RideModel>>("ridesDict");

            var newRides = new List<RideModel>();

            using (var transaction = stateManager.CreateTransaction())
            {
                var enumerable = await ridesDict.CreateEnumerableAsync(transaction);
                var enumerator = enumerable.GetAsyncEnumerator();

                while (await enumerator.MoveNextAsync(default))
                {
                    var currentRide = enumerator.Current.Value;
                    if (currentRide.Status == RideStatus.Pending)
                    {
                        newRides.Add(currentRide);
                    }
                }
            }

            return newRides;
        }

        public async Task<RideModel> GetRideById(Guid rideId)
        {
            var stateManager = this.StateManager;
            var ridesDict = await stateManager.GetOrAddAsync<IReliableDictionary<Guid, RideModel>>("ridesDict");

            using (var transaction = stateManager.CreateTransaction())
            {
                var ride = await ridesDict.TryGetValueAsync(transaction, rideId);

                return ride.Value;
            }
        }

        public async Task<IEnumerable<RideModel>> GetRidesForDriver(Guid driverId)
        {
            var stateManager = this.StateManager;
            var ridesDict = await stateManager.GetOrAddAsync<IReliableDictionary<Guid, RideModel>>("ridesDict");

            var newRides = new List<RideModel>();

            using (var transaction = stateManager.CreateTransaction())
            {
                var enumerable = await ridesDict.CreateEnumerableAsync(transaction);
                var enumerator = enumerable.GetAsyncEnumerator();

                while (await enumerator.MoveNextAsync(default))
                {
                    var currentRide = enumerator.Current.Value;
                    if (currentRide.DriverId == driverId && currentRide.Status == RideStatus.Finished)
                    {
                        newRides.Add(currentRide);
                    }
                }
            }

            return newRides;
        }

        public async Task<IEnumerable<RideModel>> GetRidesForUser(Guid userId)
        {
            var stateManager = this.StateManager;
            var ridesDict = await stateManager.GetOrAddAsync<IReliableDictionary<Guid, RideModel>>("ridesDict");

            var newRides = new List<RideModel>();

            using (var transaction = stateManager.CreateTransaction())
            {
                var enumerable = await ridesDict.CreateEnumerableAsync(transaction);
                var enumerator = enumerable.GetAsyncEnumerator();

                while (await enumerator.MoveNextAsync(default))
                {
                    var currentRide = enumerator.Current.Value;
                    if (currentRide.PassengerId == userId && currentRide.Status == RideStatus.Finished)
                    {
                        newRides.Add(currentRide);
                    }
                }
            }

            return newRides;
        }

        public async Task UpdateRide(RideModel ride)
        {
            var stateManager = this.StateManager;
            var ridesDict = await stateManager.GetOrAddAsync<IReliableDictionary<Guid, RideModel>>("ridesDict");

            using (var transaction = stateManager.CreateTransaction())
            {
                await ridesDict.AddOrUpdateAsync(transaction, ride.Id, ride, (k, v) => ride);
                await transaction.CommitAsync();
            }
        }
        #endregion

        /// <summary>
        /// Optional override to create listeners (e.g., HTTP, Service Remoting, WCF, etc.) for this service replica to handle client or user requests.
        /// </summary>
        /// <remarks>
        /// For more information on service communication, see https://aka.ms/servicefabricservicecommunication
        /// </remarks>
        /// <returns>A collection of listeners.</returns>
        protected override IEnumerable<ServiceReplicaListener> CreateServiceReplicaListeners()
        {
            return this.CreateServiceRemotingReplicaListeners();
        }

        /// <summary>
        /// This is the main entry point for your service replica.
        /// This method executes when this replica of your service becomes primary and has write status.
        /// </summary>
        /// <param name="cancellationToken">Canceled when Service Fabric needs to shut down this service replica.</param>
        protected override async Task RunAsync(CancellationToken cancellationToken)
        {
            // TODO: Replace the following sample code with your own logic 
            //       or remove this RunAsync override if it's not needed in your service.

            var myDictionary = await this.StateManager.GetOrAddAsync<IReliableDictionary<string, long>>("myDictionary");

            while (true)
            {
                cancellationToken.ThrowIfCancellationRequested();

                using (var tx = this.StateManager.CreateTransaction())
                {
                    var result = await myDictionary.TryGetValueAsync(tx, "Counter");

                    ServiceEventSource.Current.ServiceMessage(this.Context, "Current Counter Value: {0}",
                        result.HasValue ? result.Value.ToString() : "Value does not exist.");

                    await myDictionary.AddOrUpdateAsync(tx, "Counter", 0, (key, value) => ++value);

                    // If an exception is thrown before calling CommitAsync, the transaction aborts, all changes are 
                    // discarded, and nothing is saved to the secondary replicas.
                    await tx.CommitAsync();
                }

                await Task.Delay(TimeSpan.FromSeconds(1), cancellationToken);
            }
        }
    }
}
