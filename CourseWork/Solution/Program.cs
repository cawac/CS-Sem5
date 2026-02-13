using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Controller.Menus;
using Data.Interfaces;
using Data;
using Data.Repositories;
using Solution;

var configuration = new ConfigurationBuilder()
    .SetBasePath(AppContext.BaseDirectory)
    .AddJsonFile("appsettings.json", optional: true)
    .Build();

var databaseProvider = configuration["DatabaseProvider"] ?? "Sqlite";

var connectionString =
    databaseProvider.Equals("SqlServer", StringComparison.OrdinalIgnoreCase)
        ? (configuration.GetConnectionString("SqlServer")
           ?? configuration.GetConnectionString("DefaultConnection"))
        : (configuration.GetConnectionString("DefaultConnection")
           ?? "Data Source=warehouse.db");

var services = new ServiceCollection();

services.AddDbContext<WarehouseDbContext>(options =>
{
    if (databaseProvider.Equals("SqlServer", StringComparison.OrdinalIgnoreCase))
        options.UseSqlServer(connectionString);
    else
        options.UseSqlite(connectionString);
});

services.AddScoped<IProductRepository, ProductRepository>();
services.AddScoped<ICategoryRepository, CategoryRepository>();
services.AddScoped<IInventoryRepository, InventoryRepository>();

var role = "Admin";

services.AddScoped<CategoryMenu>(sp =>
    new CategoryMenu(
        sp.GetRequiredService<ICategoryRepository>(),
        role));

services.AddScoped<ProductMenu>(sp =>
    new ProductMenu(
        sp.GetRequiredService<IProductRepository>(),
        sp.GetRequiredService<IInventoryRepository>(),
        role));

services.AddScoped<InventoryMenu>(sp =>
    new InventoryMenu(
        sp.GetRequiredService<IInventoryRepository>(),
        role));

services.AddScoped<AppMenu>();

var provider = services.BuildServiceProvider(
    new ServiceProviderOptions { ValidateOnBuild = true });

try
{
    using var scope = provider.CreateScope();

    var context = scope.ServiceProvider
        .GetRequiredService<WarehouseDbContext>();

    await context.Database.EnsureCreatedAsync();
    await DataInitializer.InitializeAsync(context);

    var menu = scope.ServiceProvider.GetRequiredService<AppMenu>();
    await menu.ShowMenuAsync();
}
catch (Exception ex)
{
    Console.WriteLine($"Ошибка: {ex.Message}");
    Environment.ExitCode = 1;
}
