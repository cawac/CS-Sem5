using Models;

namespace Data.Interfaces;

public interface ICategoryRepository : IRepository<Category>
{
    Task<Category?> GetByNameAsync(
        string name,
        CancellationToken cancellationToken = default);
}