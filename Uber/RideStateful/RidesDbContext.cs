using Common.Models;
using Microsoft.EntityFrameworkCore;

namespace RideStateful
{
    public class RidesDbContext : DbContext
    {
        public RidesDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<RideModel> Rides { get; set; }
    }
}
