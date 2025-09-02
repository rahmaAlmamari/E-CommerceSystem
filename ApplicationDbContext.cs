using E_CommerceSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace E_CommerceSystem
{
    public class ApplicationDbContext:DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderProducts> OrderProducts { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Supplier> Suppliers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //to set Unique Email for User ...
            modelBuilder.Entity<User>()
                        .HasIndex(u => u.Email)
                        .IsUnique();
            //to set Unique ContactEmail for Supplier ...
            modelBuilder.Entity<Supplier>()
            .HasIndex(s => s.ContactEmail)
            .IsUnique();

        }
    }
}
