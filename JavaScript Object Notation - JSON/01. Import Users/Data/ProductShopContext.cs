using Microsoft.EntityFrameworkCore;
using ProductShop.Models;
namespace ProductShop.Data
{
    public class ProductShopContext : DbContext
    {
        public ProductShopContext()
        {
        }

        public ProductShopContext(DbContextOptions options)
            : base(options)
        {
        }

        public virtual DbSet<Category> Categories { get; set; }

        public virtual DbSet<Product> Products { get; set; }

        public virtual DbSet<User> Users { get; set; }

        public virtual DbSet<CategoryProduct> CategoriesProducts { get; set; }


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder
                    .UseSqlServer(Configuration.ConnectionString);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CategoryProduct>(entity =>
            {
                entity.HasKey(x => new { x.CategoryId, x.ProductId });
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasMany(u => u.ProductsBought)
                      .WithOne(p => p.Buyer)
                      .HasForeignKey(p => p.BuyerId)
                      .OnDelete(DeleteBehavior.NoAction);

                entity.HasMany(u => u.ProductsSold)
                      .WithOne(p => p.Seller)
                      .HasForeignKey(p => p.SellerId)
                       .OnDelete(DeleteBehavior.NoAction);
            });
        }
    }
}
