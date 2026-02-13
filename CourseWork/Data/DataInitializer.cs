using Microsoft.EntityFrameworkCore;
using Models;

namespace Data;

public static class DataInitializer
{
    private static readonly string[] CategoryNames =
    {
        "Laptops",
        "Desktops",
        "Monitors",
        "Printers",
        "Networking Equipment",
        "Components",
        "Storage Devices",
        "Peripherals"
    };

    private static readonly string[] Brands =
    {
        "Lenovo",
        "Dell",
        "HP",
        "Apple",
        "Asus",
        "Acer",
        "MSI",
        "Samsung",
        "LG",
        "Cisco",
        "Kingston",
        "Seagate",
        "Western Digital"
    };

    public static async Task InitializeAsync(WarehouseDbContext context)
    {
        await context.Database.EnsureCreatedAsync();

        if (await context.Products.AnyAsync())
            return;

        var categories = new List<Category>();

        for (int i = 0; i < CategoryNames.Length; i++)
        {
            categories.Add(
                Category.Create(
                    CategoryNames[i],
                    $"{CategoryNames[i]} equipment"
                ));
        }

        context.Categories.AddRange(categories);
        await context.SaveChangesAsync();

        var products = new List<Product>();

        for (int i = 0; i < 40; i++)
        {
            var category = categories[i % categories.Count];

            products.Add(
                Product.Create(
                    $"Model-{i + 1}",
                    Brands[i % Brands.Length],
                    300 + (i * 25),
                    category.Id,
                    $"Description for Model-{i + 1}"
                ));
        }

        context.Products.AddRange(products);
        await context.SaveChangesAsync();

        var inventories = new List<Inventory>();

        for (int i = 0; i < products.Count; i++)
        {
            inventories.Add(
                Inventory.Create(
                    products[i].Id,
                    10 + (i * 2),
                    i % 5
                ));
        }

        context.Inventories.AddRange(inventories);
        await context.SaveChangesAsync();
    }
}
