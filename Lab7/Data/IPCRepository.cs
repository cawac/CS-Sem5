using DeviceLibrary;

namespace Data;

public interface IPCRepository : IRepository<PC>
{
    Task<IReadOnlyList<PC>> GetByManufacturerIdAsync(int manufacturerId, CancellationToken cancellationToken = default);
}
