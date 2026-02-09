using DeviceLibrary;
using Microsoft.EntityFrameworkCore;

namespace Data;

public static class DataInitializer
{
    private static readonly string[] ManufacturerNames =
    {
        "Rolex", "Omega", "Casio", "Seiko", "Citizen", "Tag Heuer", "Breitling", "Patek Philippe",
        "Audemars Piguet", "Swatch", "Tissot", "Longines", "Hamilton", "Mido", "Rado",
        "Fossil", "Michael Kors", "Diesel", "Guess", "Timex", "Garmin", "Samsung", "Apple",
        "G-Shock", "Orient", "Invicta", "Bulova", "Movado", "Cartier", "IWC"
    };

    private static readonly string[] AddressParts =
    {
        "Geneva", "Zurich", "Tokyo", "Osaka", "New York", "London", "Paris", "Berlin",
        "Milan", "Hong Kong", "Singapore", "Seoul", "Shanghai", "Dubai", "Moscow"
    };

    public static async Task InitializeAsync(AppDbContext context)
    {
        await context.Database.EnsureCreatedAsync();

        if (await context.Manufacturers.AnyAsync())
            return;

        var manufacturers = new List<Manufacturer>();
        for (int i = 0; i < 30; i++)
        {
            manufacturers.Add(Manufacturer.Create
            (
                ManufacturerNames[i % ManufacturerNames.Length] + " " + (i + 1),
                $"{AddressParts[i % AddressParts.Length]}, Street {i + 1}",
                i % 2 == 0
            ));
        }

        context.Manufacturers.AddRange(manufacturers);
        await context.SaveChangesAsync();

        var PCTypes = new[] { PCType.Desktop, PCType.Tablet, PCType.Laptop };
        var PCs = new List<PC>();
        for (int i = 0; i < 30; i++)
        {
            PCs.Add(PC.Create(
                $"Model-{i + 1}",
                $"SN-{1000 + i:D6}",
                PCTypes[i % PCTypes.Length],
                manufacturers[i].Id
            ));
        }

        context.PCs.AddRange(PCs);
        await context.SaveChangesAsync();
    }
}
