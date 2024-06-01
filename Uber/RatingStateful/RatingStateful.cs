using Common.Models;
using Communication;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.ServiceFabric.Data.Collections;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Remoting.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;
using System.Fabric;

namespace RatingStateful
{
    /// <summary>
    /// An instance of this class is created for each service replica by the Service Fabric runtime.
    /// </summary>
    internal sealed class RatingStateful : StatefulService, IRatingStatefulCommunication
    {
        private RatingDbContext _ratingDbContext;
        public RatingStateful(StatefulServiceContext context, IServiceProvider serviceProvider)
            : base(context)
        {
            _ratingDbContext = serviceProvider.GetService<RatingDbContext>()!;
        }

        #region RatingMethods
        public async Task AddRating(RatingModel rating)
        {
            var stateManager = this.StateManager;
            var ratingsDict = await stateManager.GetOrAddAsync<IReliableDictionary<Guid, RatingModel>>("ratingsDict");

            using (var transaction = stateManager.CreateTransaction())
            {
                await ratingsDict.AddOrUpdateAsync(transaction, rating.Id, rating, (k, v) => v);
                await transaction.CommitAsync();
            }
        }

        public async Task UpdateRating(RatingModel rating)
        {
            var stateManager = this.StateManager;
            var ratingsDict = await stateManager.GetOrAddAsync<IReliableDictionary<Guid, RatingModel>>("ratingsDict");

            using (var transaction = stateManager.CreateTransaction())
            {
                await ratingsDict.AddOrUpdateAsync(transaction, rating.Id, rating, (k, v) => rating);
                await transaction.CommitAsync();
            }
        }

        public async Task<RatingModel> GetRating(Guid userId)
        {
            var stateManager = this.StateManager;
            var ratingsDict = await stateManager.GetOrAddAsync<IReliableDictionary<Guid, RatingModel>>("ratingsDict");

            using (var transaction = stateManager.CreateTransaction())
            {
                var enumerator = (await ratingsDict.CreateEnumerableAsync(transaction)).GetAsyncEnumerator();

                while (await enumerator.MoveNextAsync(default))
                {
                    var rating = enumerator.Current.Value;
                    if (rating.UserId == userId)
                    {
                        return rating;
                    }
                }
            }
            return null;
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
