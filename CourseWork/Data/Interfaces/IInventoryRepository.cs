using Models;

namespace Data.Interfaces;

public interface IInventoryRepository : IRepository<Inventory>
{
    Task<IReadOnlyList<Inventory>> GetByProductIdAsync(
        int productId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Inventory>> GetAvailableAsync(
        CancellationToken cancellationToken = default);
}