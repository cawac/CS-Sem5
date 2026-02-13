using Data.Interfaces;
using Microsoft.EntityFrameworkCore;
using Models;

namespace Data.Repositories;

public class InventoryRepository : Repository<Inventory>, IInventoryRepository
{
    public InventoryRepository(WarehouseDbContext context) : base(context) { }

    public async Task<IReadOnlyList<Inventory>> GetByProductIdAsync(int productId, CancellationToken cancellationToken = default)
        => await _dbSet.Where(i => i.ProductId == productId).ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<Inventory>> GetAvailableAsync(CancellationToken cancellationToken = default)
    {
        var list = _dbSet
            .AsEnumerable()  
            .Where(i => i.QuantityAvailable > 0)
            .ToList();

        return await Task.FromResult(list);
    }
}