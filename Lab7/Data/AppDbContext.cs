using DeviceLibrary;
using Microsoft.EntityFrameworkCore;

namespace Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Manufacturer> Manufacturers => Set<Manufacturer>();
    public DbSet<PC> PCs => Set<PC>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Manufacturer>(entity =>
        {
            entity.HasKey(x => x.Id);

            entity.Property(x => x.Name)
                  .IsRequired();

            entity.Property(x => x.Address)
                  .IsRequired();

            entity.HasMany(x => x.PCs)
                  .WithOne(x => x.Manufacturer)
                  .HasForeignKey(x => x.ManufacturerId);
        });

        modelBuilder.Entity<PC>(entity =>
        {
            entity.HasKey(x => x.Id);

            entity.Property(x => x.Model)
                  .IsRequired();

            entity.Property(x => x.SerialNumber)
                  .IsRequired();

            entity.Property(x => x.PCType)
                  .IsRequired();
        });
    }

}
