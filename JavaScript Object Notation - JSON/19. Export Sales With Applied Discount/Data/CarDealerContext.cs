using Microsoft.EntityFrameworkCore;
using CarDealer.Models;

namespace CarDealer.Data
{
    public class CarDealerContext : DbContext
    {
        public CarDealerContext()
        {
        }

        public CarDealerContext(DbContextOptions options)
            : base(options)
        {
        }
      
        public virtual DbSet<Car> Cars { get; set; }

        public virtual DbSet<Customer> Customers { get; set; }

        public virtual DbSet<Part> Parts { get; set; }

        public virtual DbSet<PartCar> PartsCars { get; set; }

        public virtual DbSet<Sale> Sales { get; set; }

        public virtual DbSet<Supplier> Suppliers { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(Configuration.ConnectionString);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<PartCar>(e =>
            {
                e.HasKey(k => new { k.CarId, k.PartId });
            });

            modelBuilder.Entity<Car>()
            .HasMany(c => c.PartsCars)
            .WithOne(pc => pc.Car);

            modelBuilder.Entity<PartCar>()
            .HasOne(pc => pc.Part)
            .WithMany(pc => pc.PartsCars);

            modelBuilder.Entity<Supplier>()
                .HasMany(s => s.Parts)
                .WithOne(p => p.Supplier);

            modelBuilder.Entity<Sale>()
                    .HasOne(s => s.Car);
             

            modelBuilder.Entity<Sale>()
                .HasOne(s => s.Customer)
                .WithMany(c => c.Sales)
                .HasForeignKey(s => s.CustomerId);


        }
    }
}
