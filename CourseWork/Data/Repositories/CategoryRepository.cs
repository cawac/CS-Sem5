using Data.Interfaces;
using Microsoft.EntityFrameworkCore;
using Models;

namespace Data.Repositories;

public class CategoryRepository : Repository<Category>, ICategoryRepository
{
    public CategoryRepository(WarehouseDbContext context) : base(context) { }

    public async Task<Category?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
        => await _dbSet.FirstOrDefaultAsync(c => c.Name == name, cancellationToken);
}