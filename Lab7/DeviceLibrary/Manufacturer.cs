using DeviceLibrary;

public class Manufacturer : IPrintable
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string Address { get; set; } = null!;
    public bool IsAChildCompany { get; set; }

    public ICollection<PC> PCs { get; set; } = new List<PC>();

    public Manufacturer() { }

    public Manufacturer(string name, string address, bool isAChildCompany)
    {
        Name = name;
        Address = address;
        IsAChildCompany = isAChildCompany;
    }

    public static Manufacturer Create(string name, string address, bool isAChildCompany)
        => new Manufacturer(name, address, isAChildCompany);

    public string PrintObject()
        => $"Manufacturer: Name={Name}, Address={Address}, IsAChildCompany={IsAChildCompany}";
}
