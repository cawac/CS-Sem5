namespace DeviceLibrary
{
    public class PC : IPrintable
    {
        public int ID { get; set; }
        public string Model { get; set; }
        public string SerialNumber { get; set; }
        public PCType PCType { get; set; }

        public PC(int id, string model, string serialNumber, PCType pcType)
        {
            this.ID = id;
            this.Model = model;
            this.SerialNumber = serialNumber;
            this.PCType = pcType;
        }
        
        public static PC Create(int id, string model, string serialNumber, PCType pcType)
        {
            return new PC(id, model, serialNumber, pcType);
        }

        public string PrintObject()
        {
            return $"PC: ID={ID}, Model={Model}, SerialNumber={SerialNumber}, PCType={PCType}";
        }
    }
}