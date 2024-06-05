using Common.Enums;
using Common.Models;
using Communication;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.ServiceFabric.Data.Collections;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Remoting.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;
using System.Fabric;

namespace UserStatefull
{
    /// <summary>
    /// An instance of this class is created for each service replica by the Service Fabric runtime.
    /// </summary>
    internal sealed class UserStatefull : StatefulService, IUserStatefulCommunication
    {
        private UserDbContext _userDbContext;
        public UserStatefull(StatefulServiceContext context, IServiceProvider serviceProvider)
            : base(context)
        {
            _userDbContext = serviceProvider.GetService<UserDbContext>()!;
        }

        #region UserMethods
        public async Task<IEnumerable<UserModel>> GetAllDrivers()
        {
            var stateManager = this.StateManager;
            var usersDict = await stateManager.GetOrAddAsync<IReliableDictionary<Guid, UserModel>>("usersDict");

            var drivers = new List<UserModel>();

            using (var transaction = stateManager.CreateTransaction())
            {
                var enumerable = await usersDict.CreateEnumerableAsync(transaction);
                var enumerator = enumerable.GetAsyncEnumerator();

                while (await enumerator.MoveNextAsync(default))
                {
                    if (enumerator.Current.Value.Role == UserRole.Driver)
                    {
                        drivers.Add(enumerator.Current.Value);
                    }
                }
            }

            return drivers;
        }

        public async Task<UserModel> GetUserByEmail(string email)
        {
            var stateManager = this.StateManager;
            var usersDict = await stateManager.GetOrAddAsync<IReliableDictionary<Guid, UserModel>>("usersDict");

            using (var transaction = stateManager.CreateTransaction())
            {
                var enumerator = (await usersDict.CreateEnumerableAsync(transaction)).GetAsyncEnumerator();

                while (await enumerator.MoveNextAsync(default))
                {
                    var user = enumerator.Current.Value;
                    if (user.Email == email)
                    {
                        return user;
                    }
                }
            }
            return null;
        }

        public async Task<UserModel> GetUserById(Guid id)
        {
            var stateManager = this.StateManager;
            var usersDict = await stateManager.GetOrAddAsync<IReliableDictionary<Guid, UserModel>>("usersDict");

            using (var transaction = stateManager.CreateTransaction())
            {
                var user = await usersDict.TryGetValueAsync(transaction, id);

                return user.Value;
            }
        }

        public async Task Register(UserModel user)
        {
            var stateManager = this.StateManager;
            var usersDict = await stateManager.GetOrAddAsync<IReliableDictionary<Guid, UserModel>>("usersDict");

            using (var transaction = stateManager.CreateTransaction())
            {
                await usersDict.AddOrUpdateAsync(transaction, user.Id, user, (k, v) => v);
                await transaction.CommitAsync();
            }
            await _userDbContext.Users.AddAsync(user);
            await _userDbContext.SaveChangesAsync();
        }

        public async Task UpdateUser(UserModel user)
        {
            var stateManager = this.StateManager;
            var usersDict = await stateManager.GetOrAddAsync<IReliableDictionary<Guid, UserModel>>("usersDict");

            using (var transaction = stateManager.CreateTransaction())
            {
                await usersDict.AddOrUpdateAsync(transaction, user.Id, user, (k, v) => user);
                await transaction.CommitAsync();
            }
            var existingUser = await _userDbContext.Users.FirstOrDefaultAsync(u => u.Id == user.Id);

            if (existingUser != null)
            {
                existingUser.UserName = user.UserName;
                existingUser.Email = user.Email;
                existingUser.Password = user.Password;
                existingUser.Name = user.Name;
                existingUser.LastName = user.LastName;
                existingUser.Birthday = user.Birthday;
                existingUser.Address = user.Address;
                existingUser.Image = user.Image;
                existingUser.VerificationState = user.VerificationState;

                await _userDbContext.SaveChangesAsync();
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

            try
            {

                var users = await _userDbContext.Users.ToListAsync();

                if (users != null && users.Count > 0)
                {
                    foreach (var user in users)
                    {
                        await this.Register(user);
                    }
                }
            }
            catch (Exception ex)
            {

            }

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
