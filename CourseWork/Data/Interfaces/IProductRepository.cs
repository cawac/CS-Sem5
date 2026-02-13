namespace Data.Interfaces;

public interface IProductRepository : IRepository<Product>
{
    Task<IReadOnlyList<Product>> GetByCategoryIdAsync(int categoryId, CancellationToken ct = default);
    Task<IReadOnlyList<Product>> GetByBrandAsync(string brand, CancellationToken ct = default);

    Task<IReadOnlyList<Product>> SearchAsync(
        string? model,
        string? brand,
        int? categoryId,
        decimal? minPrice,
        decimal? maxPrice,
        CancellationToken ct = default);
}