
using Microsoft.EntityFrameworkCore;
using Hostelhub.Models;

namespace Hostelhub.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(
            DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }

        public DbSet<Complaint> Complaints { get; set; }
    }
}
