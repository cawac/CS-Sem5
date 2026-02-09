namespace DeviceLibrary
{
    public class PC : IPrintable
    {
        public int Id { get; set; } // первичный ключ
        public string Model { get; set; } = string.Empty;
        public string SerialNumber { get; set; } = string.Empty;
        public PCType PCType { get; set; }
        public int ManufacturerId { get; set; }
        public Manufacturer Manufacturer { get; set; } = null!;

        public PC() { }

        public PC(string model, string serialNumber, PCType pcType, int manufacturerId)
        {
            Model = model;
            SerialNumber = serialNumber;
            PCType = pcType;
            ManufacturerId = manufacturerId;
        }

        public static PC Create(string model, string serialNumber, PCType pcType, int manufacturerId)
            => new PC(model, serialNumber, pcType, manufacturerId);

        public string PrintObject()
            => $"PC: Model={Model}, SerialNumber={SerialNumber}, PCType={PCType}";
    }
}
