using DeviceLibrary;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Data;

public class PCRepository(AppDbContext context) : Repository<PC>(context), IPCRepository
{
    public async Task<IReadOnlyList<PC>> GetByManufacturerIdAsync(int manufacturerId, CancellationToken cancellationToken = default)
    {
        return await Context.PCs
            .Where(pc => pc.ManufacturerId == manufacturerId)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }
}
