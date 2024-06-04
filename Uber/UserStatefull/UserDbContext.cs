using Common.Models;
using Microsoft.EntityFrameworkCore;

namespace UserStatefull;
public class UserDbContext : DbContext
{
    public UserDbContext(DbContextOptions options) : base(options)
    {
    }
    public DbSet<UserModel> Users { get; set; }
}
