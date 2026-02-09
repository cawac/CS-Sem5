namespace DeviceLibrary
{
    public class Manufacturer : IPrintable
    {
        public string Name { get; set; }
        public string Address { get; set; }
        private bool IsAChildCompany { get; set; }

        public Manufacturer(string name, string address, bool isAChildCompany)
        {
            this.Name = name;
            this.Address = address;
            this.IsAChildCompany = isAChildCompany;
        }
        
        public static Manufacturer Create(string name, string address, bool isAChildCompany)
        {
            return new Manufacturer(name, address, isAChildCompany);
        }

        public string PrintObject()
        {
            return $"Manufacturer: Name={Name}, Address={Address}, IsAChildCompany={IsAChildCompany}";
        }
    }
}