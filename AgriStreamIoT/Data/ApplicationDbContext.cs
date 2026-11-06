using AgriStreamIoT.Models;
using Microsoft.EntityFrameworkCore;

namespace AgriStreamIoT.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(
            DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<AppUser> AppUsers { get; set; }

        public DbSet<Device> Devices { get; set; }
    }
}

