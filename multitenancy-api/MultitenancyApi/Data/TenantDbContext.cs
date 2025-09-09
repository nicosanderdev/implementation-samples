using Microsoft.EntityFrameworkCore;
using MultitenancyApi.Models;

namespace MultitenancyApi.Data
{
    public class TenantDbContext : DbContext
    {
        public TenantDbContext(DbContextOptions<TenantDbContext> options) : base(options)
        {
        }

        public DbSet<Product> Products { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Seeding data for EACH tenant using this context
            modelBuilder.Entity<Product>().HasData(
                new Product { Id = 1, Name = "Laptop Pro", Description = "Potente laptop para profesionales", Price = 1200.50m },
                new Product { Id = 2, Name = "Mouse Inalámbrico", Description = "Mouse ergonómico y preciso", Price = 25.00m }
            );
        }
    }
}
