using DeviceLibrary;
using Microsoft.EntityFrameworkCore;

namespace Data;

public class ProductManufacturerService(AppDbContext context) : IProductManufacturerService
{
    private readonly AppDbContext _context = context;

    public async Task<(bool Success, string? Error)> AddProductForNewManufacturerAsync(
        Manufacturer manufacturer,
        PC product,
        CancellationToken cancellationToken = default)
    {
        using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);

        try
        {
            await _context.Manufacturers.AddAsync(manufacturer, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            product.ManufacturerId = manufacturer.Id;
            await _context.PCs.AddAsync(product, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            await transaction.CommitAsync(cancellationToken);
            return (true, null);
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync(cancellationToken);
            return (false, ex.Message);
        }
    }
}
