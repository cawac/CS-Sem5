using Models;

public class Product : IPrintable
{
    public int Id { get; set; }

    public string Model { get; set; } = null!;
    public string Brand { get; set; } = null!;
    public string? Description { get; set; }
    public decimal UnitPrice { get; set; }
    public int CategoryId { get; set; }

    public Category Category { get; set; } = null!;
    public ICollection<Inventory> Inventories { get; set; } = new List<Inventory>();

    public Product() { }

    public Product(
        string model,
        string brand,
        decimal unitPrice,
        int categoryId,
        string? description = null)
    {
        Model = model;
        Brand = brand;
        UnitPrice = unitPrice;
        CategoryId = categoryId;
        Description = description;
    }

    public static Product Create(
        string model,
        string brand,
        decimal unitPrice,
        int categoryId,
        string? description = null)
        => new Product(model, brand, unitPrice, categoryId, description);

    public string PrintObject()
        => $"Product: Model={Model}, Brand={Brand}, Price={UnitPrice}";
}