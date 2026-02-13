namespace Models;

public class Category : IPrintable
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string? Description { get; set; }

    public ICollection<Product> Products { get; set; } = new List<Product>();

    public Category() { }

    public Category(string name, string? description = null)
    {
        Name = name;
        Description = description;
    }

    public static Category Create(string name, string? description = null)
        => new Category(name, description);

    public string PrintObject()
        => $"Category: ID={Id}, Name={Name}, Description={Description}";
}