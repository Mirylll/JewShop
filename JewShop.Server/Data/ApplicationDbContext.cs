using JewShop.Server.Models;
using Microsoft.EntityFrameworkCore;

namespace JewShop.Server.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        // --- AUTH & USER ---
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<Session> Sessions { get; set; }
        public DbSet<Address> Addresses { get; set; }
        public DbSet<PasswordReset> PasswordResets { get; set; }

        // --- PRODUCT & SUPPLY CHAIN ---
        public DbSet<Supplier> Suppliers { get; set; }
        public DbSet<Product> Products { get; set; }
        
        // --- SALES ---
        public DbSet<Coupon> Coupons { get; set; }
    }
}