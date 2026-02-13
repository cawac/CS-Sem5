namespace Models;

public class Inventory : IPrintable
{
    public int Id { get; set; }

    public int ProductId { get; set; }

    public int QuantityTotal { get; set; }
    public int QuantityReserved { get; set; }

    public int QuantityAvailable => QuantityTotal - QuantityReserved;

    public Product Product { get; set; } = null!;

    public Inventory() { }

    public Inventory(
        int productId,
        int quantityTotal,
        int quantityReserved)
    {
        ProductId = productId;
        QuantityTotal = quantityTotal;
        QuantityReserved = quantityReserved;
    }

    public void Reserve(int qty)
    {
        if (qty > QuantityAvailable)
            throw new InvalidOperationException("Not enough stock");

        QuantityReserved += qty;
    }
    
    public static Inventory Create(
        int productId,
        int quantityTotal,
        int quantityReserved = 0)
        => new Inventory(productId, quantityTotal, quantityReserved);

    public string PrintObject()
        => $"Inventory: ProductId={ProductId}, Total={QuantityTotal}, Reserved={QuantityReserved}, Available={QuantityAvailable}";
}
