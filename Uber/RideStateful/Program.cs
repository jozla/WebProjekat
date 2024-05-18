using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.ServiceFabric.Services.Runtime;
using System.Diagnostics;

namespace RideStateful
{
    internal static class Program
    {
        /// <summary>
        /// This is the entry point of the service host process.
        /// </summary>
        private static void Main()
        {
            try
            {
                // The ServiceManifest.XML file defines one or more service type names.
                // Registering a service maps a service type name to a .NET type.
                // When Service Fabric creates an instance of this service type,
                // an instance of the class is created in this host process.

                var provider = new ServiceCollection()
                    .AddDbContext<RidesDbContext>(options => options.UseSqlServer("Server=localhost,1434;Database=Database;User Id=sa;Password=Sifra123!;TrustServerCertificate=true;"))
                    .BuildServiceProvider();

                ServiceRuntime.RegisterServiceAsync("RideStatefulType",
                    context => new RideStateful(context, provider)).GetAwaiter().GetResult();

                ServiceEventSource.Current.ServiceTypeRegistered(Process.GetCurrentProcess().Id, typeof(RideStateful).Name);

                // Prevents this host process from terminating so services keep running.
                Thread.Sleep(Timeout.Infinite);
            }
            catch (Exception e)
            {
                ServiceEventSource.Current.ServiceHostInitializationFailed(e.ToString());
                throw;
            }
        }
    }
}
