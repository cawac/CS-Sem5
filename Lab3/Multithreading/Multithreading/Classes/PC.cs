namespace DeviceLibrary
{
    public class PC
    {
        private int ID { get; set; }

        public string Model { get; set; }
        public string SerialNumber { get; set; }
        public PCType PCType { get; set; }

        public static PC Create(int id, string model, string serialNumber, PCType pcType)
        {
            return new PC
            {
                ID = id,
                Model = model,
                SerialNumber = serialNumber,
                PCType = pcType
            };
        }

        public string PrintObject()
        {
            return $"PC: ID={ID}, Model={Model}, SerialNumber={SerialNumber}, PCType={PCType}";
        }

        public static PC FromString(string line)
        {
            var parts = line.Replace("PC: ", "").Split(", ");

            return new PC
            {
                ID = int.Parse(parts[0].Split('=')[1]),
                Model = parts[1].Split('=')[1],
                SerialNumber = parts[2].Split('=')[1],
                PCType = Enum.Parse<PCType>(parts[3].Split('=')[1])
            };
        }
    }
}