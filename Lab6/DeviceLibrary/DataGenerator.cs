namespace DeviceLibrary;

public static class DataGenerator
{
    public static List<PC> GeneratePcs(int count)
    {
        var pcs = new List<PC>();
        var types = Enum.GetValues(typeof(PCType)).Cast<PCType>().ToList();

        for (int i = 1; i <= count; i++)
        {
            pcs.Add(PC.Create(i, $"model{i}", $"Serial-{i}", types[i % types.Count]));
        }
        
        return pcs;
    }
    
    public static List<Manufacturer> GenerateManufacturers(int count)
    {
        var manufacturers = new List<Manufacturer>();

        for (int i = 1; i <= count; i++)
        {
            manufacturers.Add(Manufacturer.Create($"name{i}",$"address{i}", i % 2 == 0));
        }
        
        return manufacturers;
    }
}