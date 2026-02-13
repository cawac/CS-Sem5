using Microsoft.EntityFrameworkCore;
using Models;

namespace Data;

public class WarehouseDbContext(DbContextOptions<WarehouseDbContext> options)
    : DbContext(options)
{
    public DbSet<Product> Products => Set<Product>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Inventory> Inventories => Set<Inventory>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(x => x.Id);

            entity.Property(x => x.Name)
                .IsRequired();

            entity.Property(x => x.Description);
            
            entity.HasMany(x => x.Products)
                .WithOne(x => x.Category)
                .HasForeignKey(x => x.CategoryId);
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(x => x.Id);

            entity.Property(x => x.Model)
                .IsRequired();

            entity.Property(x => x.Brand)
                .IsRequired();

            entity.Property(x => x.Description);

            entity.Property(x => x.UnitPrice)
                .IsRequired()
                .HasColumnType("decimal(18,2)");

            entity.HasMany(x => x.Inventories)
                .WithOne(x => x.Product)
                .HasForeignKey(x => x.ProductId);
        });

        modelBuilder.Entity<Inventory>(entity =>
        {
            entity.HasKey(x => x.Id);

            entity.Property(x => x.QuantityTotal)
                .IsRequired();

            entity.Property(x => x.QuantityReserved)
                .IsRequired();
        });
    }
}