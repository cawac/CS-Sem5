namespace DeviceLibrary
{
    public class PC
    {
        private int ID { get; set; }

        public string Model { get; set; }
        public string SerialNumber { get; set; }
        public PCType PCType { get; set; }

        public void Create(int id, string model, string serialNumber, PCType pcType)
        {
            ID = id;
            Model = model;
            SerialNumber = serialNumber;
            PCType = pcType;
        }

        public string PrintObject()
        {
            return $"PC: ID={ID}, Model={Model}, SerialNumber={SerialNumber}, PCType={PCType}";
        }
    }
}