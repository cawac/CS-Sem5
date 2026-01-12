namespace DeviceLibrary
{
    public class Manufacturer
    {
        public string Name { get; set; }
        public string Address { get; set; }
        private bool IsAChildCompany { get; set; }

        public void Create(string name, string address, bool isAChildCompany)
        {
            Name = name;
            Address = address;
            IsAChildCompany = isAChildCompany;
        }

        public string PrintObject()
        {
            return $"Manufacturer: Name={Name}, Address={Address}, IsAChildCompany={IsAChildCompany}";
        }
    }
}