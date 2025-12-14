using JewShop.Server.Models;
using Microsoft.EntityFrameworkCore;

namespace JewShop.Server.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        public DbSet<Product> Products { get; set; }
        public DbSet<Supplier> Suppliers { get; set; }
<<<<<<< HEAD
        public DbSet<User> Users { get; set; }       // Mới thêm
        public DbSet<Session> Sessions { get; set; } // Mới thêm
=======
        public DbSet<Coupon> Coupons { get; set; }
>>>>>>> phanquy
    }
}