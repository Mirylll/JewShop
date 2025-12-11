using JewShop.Server.Models;
using Microsoft.EntityFrameworkCore;

namespace JewShop.Server.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        public DbSet<Supplier> Suppliers { get; set; }
    }
}
