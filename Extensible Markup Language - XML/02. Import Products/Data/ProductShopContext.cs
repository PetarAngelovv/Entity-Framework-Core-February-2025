namespace ProductShop.Data
{
    using Microsoft.EntityFrameworkCore;

    using Models;

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
        public virtual DbSet<CategoryProduct> CategoryProducts { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(Configuration.ConnectionString);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Конфигурация за User
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasMany(u => u.ProductsSold)
                      .WithOne(p => p.Seller)
                      .HasForeignKey(p => p.SellerId)
                      .OnDelete(DeleteBehavior.NoAction);

                entity.HasMany(u => u.ProductsBought)
                      .WithOne(p => p.Buyer)
                      .HasForeignKey(p => p.BuyerId)
                      .OnDelete(DeleteBehavior.NoAction);
            });

           
            modelBuilder.Entity<Product>(entity =>
            {
                 entity.Property(p => p.Price).HasColumnType("decimal(18,2)");

             
                entity.HasOne(p => p.Seller)
                      .WithMany(u => u.ProductsSold)
                      .HasForeignKey(p => p.SellerId)
                      .OnDelete(DeleteBehavior.NoAction);

                entity.HasOne(p => p.Buyer)
                      .WithMany(u => u.ProductsBought)
                      .HasForeignKey(p => p.BuyerId)
                      .OnDelete(DeleteBehavior.NoAction);
            });

            // Конфигурация за CategoryProduct
            modelBuilder.Entity<CategoryProduct>(entity =>
            {
                // Съставен ключ (composite key)
                entity.HasKey(cp => new { cp.CategoryId, cp.ProductId });

            
                entity.HasOne(cp => cp.Category)
                      .WithMany(c => c.CategoryProducts)
                      .HasForeignKey(cp => cp.CategoryId);

                
                entity.HasOne(cp => cp.Product)
                      .WithMany(p => p.CategoryProducts)
                      .HasForeignKey(cp => cp.ProductId);
            });
        }
    }
}