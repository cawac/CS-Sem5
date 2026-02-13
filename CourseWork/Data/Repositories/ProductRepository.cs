using Data.Interfaces;
using Microsoft.EntityFrameworkCore;
using Models;

namespace Data.Repositories;

public class ProductRepository : Repository<Product>, IProductRepository
{
    public ProductRepository(WarehouseDbContext context) : base(context) { }

    public async Task<IReadOnlyList<Product>> GetByCategoryIdAsync(int categoryId, CancellationToken cancellationToken = default)
        => await _dbSet.Where(p => p.CategoryId == categoryId).ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<Product>> GetByBrandAsync(string brand, CancellationToken cancellationToken = default)
        => await _dbSet.Where(p => p.Brand == brand).ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<Product>> SearchAsync(
        string? model,
        string? brand,
        int? categoryId,
        decimal? minPrice,
        decimal? maxPrice,
        CancellationToken ct = default)
    {
        var query = _dbSet.AsQueryable();

        if (!string.IsNullOrWhiteSpace(model))
            query = query.Where(p =>
                p.Model.Contains(model));

        if (!string.IsNullOrWhiteSpace(brand))
            query = query.Where(p =>
                p.Brand.Contains(brand));

        if (categoryId.HasValue)
            query = query.Where(p =>
                p.CategoryId == categoryId);

        if (minPrice.HasValue)
            query = query.Where(p =>
                p.UnitPrice >= minPrice);

        if (maxPrice.HasValue)
            query = query.Where(p =>
                p.UnitPrice <= maxPrice);

        return await query.ToListAsync(ct);
    }
}