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
        public DbSet<RefreshToken> RefreshTokens { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // safe default call ...
            base.OnModelCreating(modelBuilder); 

            //===============User========================
            //to set Unique Email for User ...
            modelBuilder.Entity<User>()
                        .HasIndex(u => u.Email)
                        .IsUnique();
            // to set default value for CreatedAt in User ...
            modelBuilder.Entity<User>()
            .Property(u => u.CreatedAt)
            .HasDefaultValueSql("GETUTCDATE()");
            //to set default value for user role ...
            modelBuilder.Entity<User>()
            .Property(u => u.Role)
            .HasDefaultValue("customer");
            //to add constraint to user role ...
            modelBuilder.Entity<User>()
                .HasCheckConstraint("CK_User_Role",
                    "[Role] IN ('admin','manager','customer')");


            //===============Supplier========================
            //to set Unique ContactEmail for Supplier ...
            modelBuilder.Entity<Supplier>()
            .HasIndex(s => s.ContactEmail)
            .IsUnique();

            //===============RefreshToken========================
            //to set Unique Token for RefreshToken ...
            modelBuilder.Entity<RefreshToken>()
                .HasIndex(rt => rt.Token)
                .IsUnique();
            // Configure one-to-many relationship between User and RefreshToken ...
            modelBuilder.Entity<RefreshToken>()
            .HasOne(rt => rt.User)
            .WithMany(u => u.RefreshTokens)
            .HasForeignKey(rt => rt.UserId)
            .OnDelete(DeleteBehavior.Cascade);
            // Composite index on UserId, Revoked, and Expires for efficient queries ...
            modelBuilder.Entity<RefreshToken>()
            .HasIndex(rt => new { rt.UserId, rt.Revoked, rt.Expires });


        }
    }
}
