using System.Xml.Serialization;

namespace DeviceLibrary
{
    public class PC
    {
        [XmlIgnore]
        private int ID { get; set; }

        [XmlAttribute("Model")]
        public string Model { get; set; }

        [XmlAttribute("SerialNumber")]
        public string SerialNumber { get; set; }

        [XmlAttribute("PCType")]
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