using Data.Interfaces;
using Models;

namespace Controller.Menus;

public class CategoryMenu : BaseMenu
{
    private readonly ICategoryRepository _categoryRepository;

    public CategoryMenu(ICategoryRepository categoryRepository, string role)
        : base(role)
    {
        _categoryRepository = categoryRepository;
    }

    public override async Task ShowMenuAsync()
    {
        while (true)
        {
            Console.WriteLine();
            Console.WriteLine("Categories Menu:");
            Console.WriteLine("1. List all");
            Console.WriteLine("2. By Id");

            if (IsAdmin)
            {
                Console.WriteLine("3. Create");
                Console.WriteLine("4. Update");
                Console.WriteLine("5. Delete");
            }

            Console.WriteLine("0. Back");
            Console.Write("Choice: ");

            if (!int.TryParse(Console.ReadLine(), out var choice))
                continue;

            if (choice == 0) return;

            switch (choice)
            {
                case 1:
                    await ListAllAsync();
                    break;
                case 2:
                    await GetByIdAsync();
                    break;
                case 3 when IsAdmin:
                    await CreateAsync();
                    break;
                case 4 when IsAdmin:
                    await UpdateAsync();
                    break;
                case 5 when IsAdmin:
                    await DeleteAsync();
                    break;
                default:
                    Console.WriteLine("Unknown option");
                    break;
            }
        }
    }

    private async Task ListAllAsync()
    {
        var all = await _categoryRepository.GetAllAsync();
        foreach (var c in all)
            Console.WriteLine(c.PrintObject());
    }

    private async Task GetByIdAsync()
    {
        var id = ReadId("Id: ");
        if (id is null) return;

        var c = await _categoryRepository.GetByIdAsync(id.Value);
        Console.WriteLine(c == null ? "Not found" : c.PrintObject());
    }

    private async Task CreateAsync()
    {
        var name = ReadRequired("Name: ", "Name");
        var desc = ReadRequired("Description: ", "Description");
        if (name is null || desc is null) return;

        var entity = Category.Create(name, desc);
        await _categoryRepository.CreateAsync(entity);

        Console.WriteLine("Created");
    }

    private async Task UpdateAsync()
    {
        var id = ReadId("Id: ");
        if (id is null) return;

        var entity = await _categoryRepository.GetByIdAsync(id.Value);
        if (entity == null) return;

        Console.Write("New Name: ");
        var n = Console.ReadLine()?.Trim();
        if (!string.IsNullOrEmpty(n)) entity.Name = n;

        Console.Write("New Description: ");
        var d = Console.ReadLine()?.Trim();
        if (!string.IsNullOrEmpty(d)) entity.Description = d;

        await _categoryRepository.UpdateAsync(entity);
        Console.WriteLine("Updated");
    }

    private async Task DeleteAsync()
    {
        var id = ReadId("Id: ");
        if (id is null) return;

        var entity = await _categoryRepository.GetByIdAsync(id.Value);
        if (entity == null) return;

        await _categoryRepository.DeleteAsync(entity);
        Console.WriteLine("Deleted");
    }
}
