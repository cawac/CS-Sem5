using Data.Interfaces;

namespace Controller.Menus;

public class ProductMenu : BaseMenu
{
    private readonly IProductRepository _productRepository;
    private readonly IInventoryRepository _inventoryRepository;

    public ProductMenu(
        IProductRepository productRepository,
        IInventoryRepository inventoryRepository,
        string role)
        : base(role)
    {
        _productRepository = productRepository;
        _inventoryRepository = inventoryRepository;
    }

    public override async Task ShowMenuAsync()
    {
        while (true)
        {
            Console.WriteLine();
            Console.WriteLine("Products Menu:");
            Console.WriteLine("1. List all");
            Console.WriteLine("2. By Id");
            Console.WriteLine("3. Search");
            Console.WriteLine("4. Reserve Stock");

            if (IsAdmin)
            {
                Console.WriteLine("5. Create");
                Console.WriteLine("6. Update");
                Console.WriteLine("7. Delete");
            }

            Console.WriteLine("0. Back");
            Console.Write("Choice: ");

            if (!int.TryParse(Console.ReadLine(), out var c))
                continue;

            if (c == 0) return;

            switch (c)
            {
                case 1: 
                    await ListAllAsync(); 
                    break;
                case 2: 
                    await GetByIdAsync();
                    break;
                case 3: 
                    await SearchAsync();
                    break;
                case 4: 
                    await ReserveAsync(); 
                    break;
                case 5 when IsAdmin: 
                    await CreateAsync();
                    break;
                case 6 when IsAdmin:
                    await UpdateAsync();
                    break;
                case 7 when IsAdmin: 
                    await DeleteAsync();
                    break;
            }
        }
    }

    private async Task ListAllAsync()
    {
        var all = await _productRepository.GetAllAsync();
        foreach (var p in all)
            Console.WriteLine(p.PrintObject());
    }

    private async Task GetByIdAsync()
    {
        var id = ReadId("Id: ");
        if (id is null) return;

        var p = await _productRepository.GetByIdAsync(id.Value);
        Console.WriteLine(p == null ? "Not found" : p.PrintObject());
    }

    private async Task SearchAsync()
    {
        Console.Write("Model contains (Enter skip): ");
        var model = Console.ReadLine();

        Console.Write("Brand (Enter skip): ");
        var brand = Console.ReadLine();

        Console.Write("CategoryId (Enter skip): ");
        int.TryParse(Console.ReadLine(), out var categoryId);

        Console.Write("Min price (Enter skip): ");
        decimal.TryParse(Console.ReadLine(), out var minPrice);

        Console.Write("Max price (Enter skip): ");
        decimal.TryParse(Console.ReadLine(), out var maxPrice);

        var results = await _productRepository.SearchAsync(
            string.IsNullOrWhiteSpace(model) ? null : model,
            string.IsNullOrWhiteSpace(brand) ? null : brand,
            categoryId > 0 ? categoryId : null,
            minPrice > 0 ? minPrice : null,
            maxPrice > 0 ? maxPrice : null
        );

        if (!results.Any())
        {
            Console.WriteLine("No products found");
            return;
        }

        foreach (var p in results)
            Console.WriteLine(p.PrintObject());
    }


    private async Task ReserveAsync()
    {
        var productId = ReadId("Product Id: ");
        if (productId is null) return;

        Console.Write("Quantity: ");
        if (!int.TryParse(Console.ReadLine(), out var qty) || qty <= 0)
            return;

        var inventories =
            await _inventoryRepository.GetByProductIdAsync(productId.Value);

        var inv = inventories.FirstOrDefault();

        if (inv == null)
        {
            Console.WriteLine("No inventory record");
            return;
        }

        if (inv.QuantityAvailable < qty)
        {
            Console.WriteLine("Not enough stock");
            return;
        }

        inv.QuantityReserved += qty;

        await _inventoryRepository.UpdateAsync(inv);

        Console.WriteLine("Reserved");
    }

    private async Task CreateAsync()
    {
        var model = ReadRequired("Model: ", "Model");
        var brand = ReadRequired("Brand: ", "Brand");
        var categoryId = ReadId("CategoryId: ");

        if (model is null || brand is null || categoryId is null)
            return;

        Console.Write("Price: ");
        if (!decimal.TryParse(Console.ReadLine(), out var price))
            return;

        Console.Write("Description: ");
        var description = Console.ReadLine();

        var entity = Product.Create(
            model,
            brand,
            price,
            categoryId.Value,
            description
        );

        await _productRepository.CreateAsync(entity);

        Console.WriteLine("Created");
    }

    private async Task UpdateAsync()
    {
        var id = ReadId("Id: ");
        if (id is null) return;

        var entity = await _productRepository.GetByIdAsync(id.Value);
        if (entity == null)
        {
            Console.WriteLine("Not found");
            return;
        }

        Console.Write("New Model: ");
        var model = Console.ReadLine()?.Trim();
        if (!string.IsNullOrEmpty(model))
            entity.Model = model;

        Console.Write("New Brand: ");
        var brand = Console.ReadLine()?.Trim();
        if (!string.IsNullOrEmpty(brand))
            entity.Brand = brand;

        Console.Write("New CategoryId: ");
        if (int.TryParse(Console.ReadLine(), out var cid))
            entity.CategoryId = cid;

        Console.Write("New Price: ");
        if (decimal.TryParse(Console.ReadLine(), out var price))
            entity.UnitPrice = price;

        Console.Write("New Description: ");
        var desc = Console.ReadLine();
        if (!string.IsNullOrWhiteSpace(desc))
            entity.Description = desc;

        await _productRepository.UpdateAsync(entity);

        Console.WriteLine("Updated");
    }

    private async Task DeleteAsync()
    {
        var id = ReadId("Id: ");
        if (id is null) return;

        var entity = await _productRepository.GetByIdAsync(id.Value);
        if (entity == null) return;

        await _productRepository.DeleteAsync(entity);

        Console.WriteLine("Deleted");
    }
}
