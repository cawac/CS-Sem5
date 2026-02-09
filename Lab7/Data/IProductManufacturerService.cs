using DeviceLibrary;

namespace Data;

public interface IProductManufacturerService
{

    Task<(bool Success, string? Error)> AddProductForNewManufacturerAsync(
        Manufacturer manufacturer,
        PC product,
        CancellationToken cancellationToken = default);
}
