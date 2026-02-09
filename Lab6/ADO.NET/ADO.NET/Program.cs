using System;
using System.Threading.Tasks;
using DeviceLibrary;

class Program
{
    static async Task Main()
    {
        string connectionString = "Data Source=DeviceDB.sqlite";
        var dbManager = new DatabaseManager(connectionString);

        await dbManager.CreateTablesAsync();
        await dbManager.SeedDataAsync();

        bool exit = false;
        while (!exit)
        {
            Console.WriteLine("\n=== Menu ===");
            Console.WriteLine("1. Add Manufacturer");
            Console.WriteLine("2. Add PC for Manufacturer");
            Console.WriteLine("3. Show all PCs for Manufacturer");
            Console.WriteLine("4. Exit");
            Console.Write("Select an option: ");
            string choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    await AddManufacturerAsync(dbManager);
                    break;
                case "2":
                    await AddPCAsync(dbManager);
                    break;
                case "3":
                    await ShowPCsAsync(dbManager);
                    break;
                case "4":
                    exit = true;
                    break;
                default:
                    Console.WriteLine("Invalid choice. Please try again.");
                    break;
            }
        }
    }

    private static async Task AddManufacturerAsync(DatabaseManager dbManager)
    {
        Console.Write("Enter manufacturer name: ");
        string name = Console.ReadLine();
        Console.Write("Enter address: ");
        string address = Console.ReadLine();
        Console.Write("Is it a child company? (y/n): ");
        bool isChild = Console.ReadLine()?.Trim().ToLower() == "y";

        var manufacturer = Manufacturer.Create(name, address, isChild);
        long id = await dbManager.AddManufacturerAsync(manufacturer);
        Console.WriteLine($"Manufacturer added with Id = {id}");
    }

    private static async Task AddPCAsync(DatabaseManager dbManager)
    {
        Console.Write("Enter Manufacturer Id: ");
        if (!long.TryParse(Console.ReadLine(), out long manufacturerId))
        {
            Console.WriteLine("Invalid Id.");
            return;
        }

        Console.Write("Enter PC model: ");
        string model = Console.ReadLine();
        Console.Write("Enter serial number: ");
        string serial = Console.ReadLine();
        Console.Write("Enter PC type (Desktop, Laptop, Server, etc.): ");
        if (!Enum.TryParse(Console.ReadLine(), out PCType type))
        {
            Console.WriteLine("Invalid PC type.");
            return;
        }

        var pc = PC.Create(0, model, serial, type);
        await dbManager.AddPCAsync(pc, manufacturerId);
        Console.WriteLine("PC added.");
    }

    private static async Task ShowPCsAsync(DatabaseManager dbManager)
    {
        Console.Write("Enter manufacturer name: ");
        string name = Console.ReadLine();
        var pcs = await dbManager.GetPCsByManufacturerAsync(name);

        if (pcs.Count == 0)
        {
            Console.WriteLine("No PCs found for this manufacturer.");
            return;
        }

        Console.WriteLine($"\nPCs for manufacturer '{name}':");
        foreach (var pc in pcs)
        {
            Console.WriteLine(pc.PrintObject());
        }
    }
}
