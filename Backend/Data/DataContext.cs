using Microsoft.EntityFrameworkCore;

namespace MyAspNetCoreProject.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options)
            : base(options)
        {
        }

        public DbSet<ProductEntity> Products { get; set; }
        public DbSet<SupplierEntity> Suppliers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ProductEntity>()
                .HasMany(s => s.Suppliers)
                .WithMany(s => s.Products)
                .UsingEntity<Dictionary<string, object>>(
                    "ProductSupplier",
                    j => j.HasOne<SupplierEntity>().WithMany().HasForeignKey("SupplierId").OnDelete(DeleteBehavior.Cascade),
                    j => j.HasOne<ProductEntity>().WithMany().HasForeignKey("ProductId").OnDelete(DeleteBehavior.Cascade)
                );

            base.OnModelCreating(modelBuilder);
        }
    }
}
