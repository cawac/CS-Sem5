using Controller.Menus;
using Data;

namespace Solution;

public class AppMenu : BaseMenu
{
    private readonly WarehouseDbContext _context;
    private readonly CategoryMenu _categoryMenu;
    private readonly ProductMenu _productMenu;
    private readonly InventoryMenu _inventoryMenu;

    public AppMenu(
        WarehouseDbContext context,
        CategoryMenu categoryMenu,
        ProductMenu productMenu,
        InventoryMenu inventoryMenu,
        string role = "User") : base(role)
    {
        _context = context;
        _categoryMenu = categoryMenu;
        _productMenu = productMenu;
        _inventoryMenu = inventoryMenu;
    }

    public override async Task ShowMenuAsync()
    {
        while (true)
        {
            Console.WriteLine();
            Console.WriteLine("Computer Equipment Warehouse:");
            Console.WriteLine($"Current role: {Role}");
            if (IsAdmin) Console.WriteLine("0. Initialize DB");
            Console.WriteLine("1. Categories");
            Console.WriteLine("2. Products");
            Console.WriteLine("3. Inventory");
            Console.WriteLine("4. Switch Role");
            Console.WriteLine("5. Exit");
            Console.Write("Choose: ");

            if (!int.TryParse(Console.ReadLine(), out var choice))
                continue;

            switch (choice)
            {
                case 0 when IsAdmin:
                    await InitializeDatabaseAsync();
                    break;
                case 1:
                    await _categoryMenu.ShowMenuAsync();
                    break;
                case 2:
                    await _productMenu.ShowMenuAsync();
                    break;
                case 3:
                    await _inventoryMenu.ShowMenuAsync();
                    break;
                case 4:
                    SwitchRole();
                    break;
                case 5:
                    return;
            }
        }
    }

    private async Task InitializeDatabaseAsync()
    {
        await DataInitializer.InitializeAsync(_context);
        Console.WriteLine("Database initialized");
    }

    private void SwitchRole()
    {
        Console.Write("Enter role (Admin/User): ");
        var input = Console.ReadLine()?.Trim();

        if (string.Equals(input, "Admin", StringComparison.OrdinalIgnoreCase))
            Role = "Admin";
        else
            Role = "User";

        ApplyRoleToMenus();
        Console.WriteLine($"Role switched to: {Role}");
    }

    private void ApplyRoleToMenus()
    {
        _categoryMenu.SetRole(Role);
        _productMenu.SetRole(Role);
        _inventoryMenu.SetRole(Role);
    }
}
