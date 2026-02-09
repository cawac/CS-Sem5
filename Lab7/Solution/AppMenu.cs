using Data;
using DeviceLibrary;

namespace Solution;

public class AppMenu(
    AppDbContext context,
    IRepository<Manufacturer> manufacturerRepository,
    IPCRepository pcRepository,
    IProductManufacturerService productManufacturerService)
{
    public async Task RunAsync()
    {
        while (true)
        {
            Console.WriteLine();
            Console.WriteLine("=== Menu ===");
            Console.WriteLine("0. Initialize DB (Code First, 30 records per table)");
            Console.WriteLine("1. CRUD — Manufacturers");
            Console.WriteLine("2. CRUD — PCs");
            Console.WriteLine("3. Business operation: add PC for new Manufacturer");
            Console.WriteLine("4. Query: list all PCs for a Manufacturer");
            Console.WriteLine("5. Exit");
            Console.Write("Choose an option: ");

            if (!int.TryParse(Console.ReadLine(), out var choice))
            {
                Console.WriteLine("Invalid input. Enter a number.");
                continue;
            }

            switch (choice)
            {
                case 0:
                    await InitializeDatabaseAsync();
                    break;
                case 1:
                    await ManufacturerCrudMenuAsync();
                    break;
                case 2:
                    await PCCrudMenuAsync();
                    break;
                case 3:
                    await AddPCForNewManufacturerAsync();
                    break;
                case 4:
                    await ListPCsByManufacturerAsync();
                    break;
                case 5:
                    Console.WriteLine("Goodbye.");
                    return;
                default:
                    Console.WriteLine("Unknown option.");
                    break;
            }
        }
    }

    private async Task InitializeDatabaseAsync()
    {
        await DataInitializer.InitializeAsync(context);
        Console.WriteLine("DB initialized. 30 records created in Manufacturers and PCs tables.");
    }

    private static int? ReadId(string prompt)
    {
        Console.Write(prompt);
        if (!int.TryParse(Console.ReadLine(), out var id) || id <= 0)
            return null;
        return id;
    }

    private static string? ReadRequired(string prompt, string fieldName)
    {
        Console.Write(prompt);
        var value = Console.ReadLine()?.Trim();
        if (string.IsNullOrEmpty(value))
        {
            Console.WriteLine($"Field '{fieldName}' cannot be empty.");
            return null;
        }
        return value;
    }

    private async Task ManufacturerCrudMenuAsync()
    {
        while (true)
        {
            Console.WriteLine();
            Console.WriteLine("--- CRUD: Manufacturers ---");
            Console.WriteLine("1. List all  2. By Id  3. Create  4. Update  5. Delete  0. Back");
            Console.Write("Choice: ");
            if (!int.TryParse(Console.ReadLine(), out var crud))
            {
                Console.WriteLine("Invalid input.");
                continue;
            }
            if (crud == 0) return;

            switch (crud)
            {
                case 1:
                    var all = await manufacturerRepository.GetAllAsync();
                    foreach (var m in all)
                        Console.WriteLine($"  Id: {m.Id}, Name: {m.Name}, Address: {m.Address}");
                    break;
                case 2:
                    var id = ReadId("Id: ");
                    if (id is null) { Console.WriteLine("Invalid Id."); break; }
                    var m2 = await manufacturerRepository.GetByIdAsync(id.Value);
                    Console.WriteLine(m2 == null ? "Not found." : $"  Id: {m2.Id}, Name: {m2.Name}, Address: {m2.Address}");
                    break;
                case 3:
                    var name = ReadRequired("Name: ", "Name");
                    var address = ReadRequired("Address: ", "Address");
                    if (name is null || address is null) break;
                    var newM = Manufacturer.Create(name, address, false);
                    await manufacturerRepository.CreateAsync(newM);
                    Console.WriteLine($"Created Manufacturer Id: {newM.Id}");
                    break;
                case 4:
                    var updId = ReadId("Id of Manufacturer to update: ");
                    if (updId is null) { Console.WriteLine("Invalid Id."); break; }
                    var m4 = await manufacturerRepository.GetByIdAsync(updId.Value);
                    if (m4 == null) { Console.WriteLine("Not found."); break; }
                    Console.Write("New Name (Enter to skip): ");
                    var n = Console.ReadLine()?.Trim();
                    if (!string.IsNullOrEmpty(n)) m4.Name = n;
                    Console.Write("New Address (Enter to skip): ");
                    var a = Console.ReadLine()?.Trim();
                    if (!string.IsNullOrEmpty(a)) m4.Address = a;
                    await manufacturerRepository.UpdateAsync(m4);
                    Console.WriteLine("Updated.");
                    break;
                case 5:
                    var delId = ReadId("Id of Manufacturer to delete: ");
                    if (delId is null) { Console.WriteLine("Invalid Id."); break; }
                    var m5 = await manufacturerRepository.GetByIdAsync(delId.Value);
                    if (m5 == null) { Console.WriteLine("Not found."); break; }
                    await manufacturerRepository.DeleteAsync(m5);
                    Console.WriteLine("Deleted.");
                    break;
                default:
                    Console.WriteLine("Unknown option.");
                    break;
            }
        }
    }

    private async Task PCCrudMenuAsync()
    {
        while (true)
        {
            Console.WriteLine();
            Console.WriteLine("--- CRUD: PCs ---");
            Console.WriteLine("1. List all  2. By Id  3. Create  4. Update  5. Delete  0. Back");
            Console.Write("Choice: ");
            if (!int.TryParse(Console.ReadLine(), out var crud))
            {
                Console.WriteLine("Invalid input.");
                continue;
            }
            if (crud == 0) return;

            switch (crud)
            {
                case 1:
                    var all = await pcRepository.GetAllAsync();
                    foreach (var pc in all)
                        Console.WriteLine($"  Id: {pc.Id}, Model: {pc.Model}, Serial: {pc.SerialNumber}, Type: {pc.PCType}, ManufacturerId: {pc.ManufacturerId}");
                    break;
                case 2:
                    var id = ReadId("Id: ");
                    if (id is null) { Console.WriteLine("Invalid Id."); break; }
                    var pc2 = await pcRepository.GetByIdAsync(id.Value);
                    Console.WriteLine(pc2 == null ? "Not found." : $"  Id: {pc2.Id}, Model: {pc2.Model}, Serial: {pc2.SerialNumber}, Type: {pc2.PCType}, ManufacturerId: {pc2.ManufacturerId}");
                    break;
                case 3:
                    var model = ReadRequired("Model: ", "Model");
                    var serial = ReadRequired("SerialNumber: ", "Serial Number");
                    if (model is null || serial is null) break;
                    var manId = ReadId("ManufacturerId: ");
                    if (manId is null) { Console.WriteLine("Invalid ManufacturerId."); break; }
                    Console.Write("PCType (0=Desktop, 1=Laptop, 2=Server): ");
                    if (!int.TryParse(Console.ReadLine(), out var t) || t < 0 || t > 2) t = 0;
                    var pcType = (PCType)t;
                    var newPC = new PC(model, serial, pcType, manId.Value);
                    await pcRepository.CreateAsync(newPC);
                    Console.WriteLine($"Created PC Id: {newPC.Id}");
                    break;
                case 4:
                    var updId = ReadId("Id of PC to update: ");
                    if (updId is null) { Console.WriteLine("Invalid Id."); break; }
                    var pc4 = await pcRepository.GetByIdAsync(updId.Value);
                    if (pc4 == null) { Console.WriteLine("Not found."); break; }
                    Console.Write("New Model (Enter to skip): ");
                    var m = Console.ReadLine()?.Trim();
                    if (!string.IsNullOrEmpty(m)) pc4.Model = m;
                    Console.Write("New SerialNumber (Enter to skip): ");
                    var s = Console.ReadLine()?.Trim();
                    if (!string.IsNullOrEmpty(s)) pc4.SerialNumber = s;
                    Console.Write("New ManufacturerId (Enter to skip): ");
                    var midInput = Console.ReadLine()?.Trim();
                    if (!string.IsNullOrEmpty(midInput) && int.TryParse(midInput, out var newManId) && newManId > 0)
                        pc4.ManufacturerId = newManId;
                    Console.WriteLine("Updated.");
                    await pcRepository.UpdateAsync(pc4);
                    break;
                case 5:
                    var delId = ReadId("Id of PC to delete: ");
                    if (delId is null) { Console.WriteLine("Invalid Id."); break; }
                    var pc5 = await pcRepository.GetByIdAsync(delId.Value);
                    if (pc5 == null) { Console.WriteLine("Not found."); break; }
                    await pcRepository.DeleteAsync(pc5);
                    Console.WriteLine("Deleted.");
                    break;
                default:
                    Console.WriteLine("Unknown option.");
                    break;
            }
        }
    }

    private async Task AddPCForNewManufacturerAsync()
    {
        var name = ReadRequired("Manufacturer Name: ", "Manufacturer Name");
        var address = ReadRequired("Manufacturer Address: ", "Address");
        var model = ReadRequired("PC Model: ", "PC Model");
        var serial = ReadRequired("Serial Number: ", "Serial Number");
        if (name is null || address is null || model is null || serial is null) return;

        Console.Write("PCType (0=Desktop, 1=Laptop, 2=Server): ");
        if (!int.TryParse(Console.ReadLine(), out var t) || t < 0 || t > 2) t = 0;
        var pcType = (PCType)t;

        var manufacturer = new Manufacturer(name, address, false);
        var pc = new PC(model, serial, pcType, 0);

        var (success, error) = await productManufacturerService.AddProductForNewManufacturerAsync(manufacturer, pc);
        if (success)
            Console.WriteLine($"Added Manufacturer and PC. ManufacturerId: {manufacturer.Id}, PCId: {pc.Id}");
        else
            Console.WriteLine($"Error (transaction rolled back): {error}");
    }

    private async Task ListPCsByManufacturerAsync()
    {
        var manufacturerId = ReadId("Manufacturer Id: ");
        if (manufacturerId is null) { Console.WriteLine("Invalid Id."); return; }
        var pcs = await pcRepository.GetByManufacturerIdAsync(manufacturerId.Value);
        Console.WriteLine($"PCs for Manufacturer {manufacturerId}: {pcs.Count}");
        foreach (var pc in pcs)
            Console.WriteLine($"  Id: {pc.Id}, Model: {pc.Model}, Serial: {pc.SerialNumber}, Type: {pc.PCType}");
    }
}
