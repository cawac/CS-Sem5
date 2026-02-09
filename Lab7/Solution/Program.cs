using Data;
using DeviceLibrary;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Solution;

var configuration = new ConfigurationBuilder()
    .SetBasePath(AppContext.BaseDirectory)
    .AddJsonFile("appsettings.json", optional: true)
    .Build();

var databaseProvider = configuration["DatabaseProvider"] ?? "Sqlite";
var connectionString = databaseProvider.Equals("SqlServer", StringComparison.OrdinalIgnoreCase)
    ? (configuration.GetConnectionString("SqlServer") ?? configuration.GetConnectionString("DefaultConnection"))
    : (configuration.GetConnectionString("DefaultConnection") ?? "Data Source=efcore.db");

var services = new ServiceCollection();
services.AddDbContext<AppDbContext>(options =>
{
    if (databaseProvider.Equals("SqlServer", StringComparison.OrdinalIgnoreCase))
        options.UseSqlServer(connectionString);
    else
        options.UseSqlite(connectionString);
});
services.AddScoped<IRepository<Manufacturer>, Repository<Manufacturer>>();
services.AddScoped<IPCRepository, PCRepository>();
services.AddScoped<IProductManufacturerService, ProductManufacturerService>();
services.AddScoped<AppMenu>();

var provider = services.BuildServiceProvider(new ServiceProviderOptions { ValidateOnBuild = true });

try
{
    using var scope = provider.CreateScope();
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await context.Database.EnsureCreatedAsync();
    await DataInitializer.InitializeAsync(context);
    var menu = scope.ServiceProvider.GetRequiredService<AppMenu>();
    await menu.RunAsync();

}
catch (Exception ex)
{
    Console.WriteLine($"Ошибка: {ex.Message}");
    Environment.ExitCode = 1;
}