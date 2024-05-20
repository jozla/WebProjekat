using Common.Models;
using Communication;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Remoting.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;
using System.Fabric;

namespace RatingStateless
{
    /// <summary>
    /// An instance of this class is created for each service instance by the Service Fabric runtime.
    /// </summary>
    internal sealed class RatingStateless : StatelessService, IRatingStatelessCommunication
    {
        private RatingDbContext _ratingDbContext;
        public RatingStateless(StatelessServiceContext context, IServiceProvider serviceProvider)
            : base(context)
        {
            _ratingDbContext = serviceProvider.GetService<RatingDbContext>()!;
        }

        #region RatingMethods
        public async Task AddRating(Guid userId, double rating)
        {
            var ratings = await _ratingDbContext.Ratings.ToListAsync();
            var existingRating = await _ratingDbContext.Ratings.SingleOrDefaultAsync(r => r.UserId == userId);

            if (existingRating == null)
            {
                var newRating = new RatingModel
                {
                    Id = Guid.NewGuid(),
                    Rating = rating,
                    UserId = userId
                };

                await _ratingDbContext.Ratings.AddAsync(newRating);
                await _ratingDbContext.SaveChangesAsync();
            }
            else
            {
                var sum = existingRating.Rating * existingRating.NumOfRates;
                sum += rating;
                existingRating.NumOfRates++;
                existingRating.Rating = sum / existingRating.NumOfRates;
                await _ratingDbContext.SaveChangesAsync();
            }
        }

        public async Task<double> GetRating(Guid userId)
        {
            var existingRating = await _ratingDbContext.Ratings.SingleOrDefaultAsync(r => r.UserId == userId);
            if (existingRating != null)
            {
                return existingRating.Rating;
            }
            return 0;
        }

        #endregion
        /// <summary>
        /// Optional override to create listeners (e.g., TCP, HTTP) for this service replica to handle client or user requests.
        /// </summary>
        /// <returns>A collection of listeners.</returns>
        protected override IEnumerable<ServiceInstanceListener> CreateServiceInstanceListeners()
        {
            return this.CreateServiceRemotingInstanceListeners();
        }

        /// <summary>
        /// This is the main entry point for your service instance.
        /// </summary>
        /// <param name="cancellationToken">Canceled when Service Fabric needs to shut down this service instance.</param>
        protected override async Task RunAsync(CancellationToken cancellationToken)
        {
            // TODO: Replace the following sample code with your own logic 
            //       or remove this RunAsync override if it's not needed in your service.

            long iterations = 0;

            while (true)
            {
                cancellationToken.ThrowIfCancellationRequested();

                ServiceEventSource.Current.ServiceMessage(this.Context, "Working-{0}", ++iterations);

                await Task.Delay(TimeSpan.FromSeconds(1), cancellationToken);
            }
        }
    }
}
