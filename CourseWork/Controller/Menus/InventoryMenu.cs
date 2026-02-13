using Data.Interfaces;
using Models;

namespace Controller.Menus;

public class InventoryMenu : BaseMenu
{
    private readonly IInventoryRepository _inventoryRepository;

    public InventoryMenu(
        IInventoryRepository inventoryRepository,
        string role)
        : base(role)
    {
        _inventoryRepository = inventoryRepository;
    }

    public override async Task ShowMenuAsync()
    {
        while (true)
        {
            Console.WriteLine();
            Console.WriteLine("Inventory Menu:");
            Console.WriteLine("1. List all");
            Console.WriteLine("2. By Product Id");
            Console.WriteLine("3. Available only");

            if (IsAdmin)
            {
                Console.WriteLine("4. Add stock");
                Console.WriteLine("5. Update");
                Console.WriteLine("6. Delete");
            }

            Console.WriteLine("0. Back");
            Console.Write("Choice: ");

            if (!int.TryParse(Console.ReadLine(), out var c))
                continue;

            if (c == 0) return;

            switch (c)
            {
                case 1: await ListAllAsync(); break;
                case 2: await ByProductAsync(); break;
                case 3: await AvailableAsync(); break;
                case 4 when IsAdmin: await AddStockAsync(); break;
                case 5 when IsAdmin: await UpdateAsync(); break;
                case 6 when IsAdmin: await DeleteAsync(); break;
            }
        }
    }

    private async Task ListAllAsync()
    {
        var all = await _inventoryRepository.GetAllAsync();

        foreach (var i in all)
            Console.WriteLine(i.PrintObject());
    }

    private async Task ByProductAsync()
    {
        var id = ReadId("Product Id: ");
        if (id is null) return;

        var list =
            await _inventoryRepository.GetByProductIdAsync(id.Value);

        foreach (var i in list)
            Console.WriteLine(i.PrintObject());
    }

    private async Task AvailableAsync()
    {
        var list = await _inventoryRepository.GetAvailableAsync();

        foreach (var i in list)
            Console.WriteLine(i.PrintObject());
    }

    private async Task AddStockAsync()
    {
        var productId = ReadId("Product Id: ");
        if (productId is null) return;

        Console.Write("Quantity to add: ");
        if (!int.TryParse(Console.ReadLine(), out var qty))
            return;

        var inv = new Inventory
        {
            ProductId = productId.Value,
            QuantityTotal = qty,
            QuantityReserved = 0
        };

        await _inventoryRepository.CreateAsync(inv);

        Console.WriteLine("Stock added");
    }

    private async Task UpdateAsync()
    {
        var id = ReadId("Inventory Id: ");
        if (id is null) return;

        var inv =
            await _inventoryRepository.GetByIdAsync(id.Value);

        if (inv == null) return;

        Console.Write("New total: ");
        if (int.TryParse(Console.ReadLine(), out var a))
            inv.QuantityTotal = a;

        Console.Write("New Reserved: ");
        if (int.TryParse(Console.ReadLine(), out var r))
            inv.QuantityReserved = r;

        await _inventoryRepository.UpdateAsync(inv);

        Console.WriteLine("Updated");
    }

    private async Task DeleteAsync()
    {
        var id = ReadId("Inventory Id: ");
        if (id is null) return;

        var inv =
            await _inventoryRepository.GetByIdAsync(id.Value);

        if (inv == null) return;

        await _inventoryRepository.DeleteAsync(inv);

        Console.WriteLine("Deleted");
    }
}
