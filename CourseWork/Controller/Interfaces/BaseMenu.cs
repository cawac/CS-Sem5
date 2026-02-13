using Controller.Interfaces;

namespace Controller.Menus;

public abstract class BaseMenu : RoleAware
{
    protected BaseMenu(string role) : base(role)
    {
    }

    public abstract Task ShowMenuAsync();

    protected int? ReadId(string label)
    {
        Console.Write(label);
        return int.TryParse(Console.ReadLine(), out var id) ? id : null;
    }

    protected string? ReadRequired(string label, string field)
    {
        Console.Write(label);
        var value = Console.ReadLine()?.Trim();

        if (string.IsNullOrEmpty(value))
        {
            Console.WriteLine($"{field} required");
            return null;
        }

        return value;
    }
}