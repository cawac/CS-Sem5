using System.ComponentModel;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using DeviceLibrary;

namespace Serialization;

internal static class Program
{
    public static void Main()
    {
        bool exit = false;
        List<PC> pcs = new List<PC>();
        string xmlFilePath = "pcs.xml";
        var allowedChoices = "0123456789";


        while (!exit)
        {
            Console.WriteLine("\nMenu:");
            Console.WriteLine("1) Create 10 PC instances and display them");
            Console.WriteLine("2) Generate 10 random PCs");
            Console.WriteLine("3) Serialize PCs to XML");
            Console.WriteLine("4) Print XML file content");
            Console.WriteLine("5) Deserialize XML and display PCs");
            Console.WriteLine("6) Find all Model names in XML and display(XDocument)");
            Console.WriteLine("7) Find all Model names in XML and display(XMLDocument)");
            Console.WriteLine("8) Edit an attribute in XML (XDocument)");
            Console.WriteLine("9) Edit an attribute in XML (XmlDocument)");
            Console.WriteLine("0) Quit");

            Console.Write("Enter choice: ");

            var choice = ReadUntilSuccess(() =>
            {
                var value = Console.ReadLine()?.ToLower();

                if (string.IsNullOrWhiteSpace(value) || !allowedChoices.Contains(value))
                    throw new ArgumentException($"Allowed values: {string.Join(", ", allowedChoices)}");

                return value;
            });

            switch (choice)
            {
                case "1":
                    pcs = CreatePCs();
                    break;

                case "2":
                    pcs = GenerateRandomPCs();
                    break; 
                
                case "3":
                    SerializePCs(pcs, xmlFilePath);
                    break;
                
                case "4":
                    PrintXmlFileContent(xmlFilePath);
                    break;
                
                case "5":
                    DeserializePCs(xmlFilePath);
                    break;
                
                case "6":
                    PrintModelsUsingXDocument(xmlFilePath);
                    break;
                
                case "7":
                    PrintModelsUsingXmlDocument(xmlFilePath);
                    break;
                
                case "8":
                    EditAttributeUsingXDocument(xmlFilePath);
                    break;

                case "9":
                    EditAttributeUsingXmlDocument(xmlFilePath);
                    break;

                case "0":
                    exit = true;
                    break;

                default:
                    Console.WriteLine("Invalid choice. Try again.");
                    break;
            }
        }
    }

    private static List<PC> CreatePCs()
    {
        var list = new List<PC>();

        for (int i = 0; i < 10; i++)
        {
            Console.WriteLine($"\nPC #{i + 1}");

            int id = ReadUntilSuccess("Enter ID: ", int.Parse);

            string model = ReadUntilSuccess(
                "Enter model: ",
                value => value 
            );

            string serialNumber = ReadUntilSuccess(
                "Enter serial number: ",
                value => value
            );

            PCType pcType = ReadUntilSuccess(
                "Enter PC type (Desktop, Tablet, Laptop): ",
                value =>
                {
                    if (!Enum.TryParse<PCType>(value, true, out var result))
                        throw new InvalidEnumArgumentException(message: "Invalid PC type");
                    return result;
                });

            var pc = new PC();
            pc.Create(id, model, serialNumber, pcType);

            Console.WriteLine(pc.PrintObject());

            list.Add(pc);
        }

        return list;
    }


    private static void SerializePCs(List<PC> pcs, string path)
    {
        if (pcs.Count == 0)
        {
            Console.WriteLine("No objects to serialize.");
            return;
        }

        var serializer = new XmlSerializer(typeof(List<PC>));

        using var fs = new FileStream(path, FileMode.Create);
        serializer.Serialize(fs, pcs);

        Console.WriteLine($"Objects serialized to {path}");
    }

    private static void DeserializePCs(string path)
    {
        if (!File.Exists(path))
        {
            Console.WriteLine("XML file not found.");
            return;
        }

        var serializer = new XmlSerializer(typeof(List<PC>));

        using var fs = new FileStream(path, FileMode.Open);
        var pcs = (List<PC>)serializer.Deserialize(fs);

        Console.WriteLine("\nDeserialized objects:");
        if (pcs == null)
        {
            Console.WriteLine("No objects to display.");
            return;
        }
        
        foreach (var pc in pcs)
        {
            Console.WriteLine(pc.PrintObject());
        }
    }
    
    private static List<PC> GenerateRandomPCs()
    {
        var list = new List<PC>();
        var random = new Random();

        var models = new[] { "Dell", "HP", "Lenovo", "Asus", "Acer" };
        var pcTypes = Enum.GetValues<PCType>();

        for (int i = 0; i < 10; i++)
        {
            var pc = new PC();

            int id = random.Next(1, 10_000);
            string model = models[random.Next(models.Length)];
            string serialNumber = Guid.NewGuid().ToString("N")[..8].ToUpper();
            PCType pcType = pcTypes[random.Next(pcTypes.Length)];

            pc.Create(id, model, serialNumber, pcType);

            Console.WriteLine(pc.PrintObject());
            list.Add(pc);
        }

        return list;
    }
    
    static void PrintXmlFileContent(string path)
    {
        if (!File.Exists(path))
        {
            Console.WriteLine("XML file not found.");
            return;
        }

        Console.WriteLine("XML file content:\n");

        string content = File.ReadAllText(path);
        Console.WriteLine(content);
    }

    static void PrintModelsUsingXDocument(string path)
    {
        if (!File.Exists(path))
        {
            Console.WriteLine("XML file not found.");
            return;
        }

        var document = XDocument.Load(path);

        var models = document
            .Descendants("PC")
            .Attributes("Model")
            .Select(a => a.Value);

        Console.WriteLine("Models found in XML:");

        foreach (var model in models)
        {
            Console.WriteLine(model);
        }
    }
    

    static void PrintModelsUsingXmlDocument(string path)
    {
        if (!File.Exists(path))
        {
            Console.WriteLine("XML file not found.");
            return;
        }

        var doc = new XmlDocument();
        doc.Load(path);

        XmlNodeList pcNodes = doc.SelectNodes("//PC[@Model]");

        Console.WriteLine("Models found in XML:");

        foreach (XmlNode pcNode in pcNodes)
        {
            var modelAttr = pcNode.Attributes["Model"];
            if (modelAttr != null)
                Console.WriteLine(modelAttr.Value);
        }
    }

    static void EditAttributeUsingXDocument(string path)
    {
        if (!File.Exists(path))
        {
            Console.WriteLine("XML file not found.");
            return;
        }

        var doc = XDocument.Load(path);

        var elements = doc.Descendants("PC").ToList();
        if (elements.Count == 0)
        {
            Console.WriteLine("No PC elements found in XML.");
            return;
        }

        var availableAttributes = elements[0].Attributes()
            .Select(a => a.Name.LocalName)
            .ToList();

        Console.WriteLine("Available attributes to edit: " + string.Join(", ", availableAttributes));

        string attrName = ReadUntilSuccess("Enter attribute name: ", value =>
        {
            if (!availableAttributes.Contains(value))
                throw new ArgumentException("Invalid attribute name");
            return value;
        });

        int index = ReadUntilSuccess("Enter element number (starting from 1): ", int.Parse) - 1;

        if (index < 0 || index >= elements.Count)
        {
            Console.WriteLine("Invalid element number.");
            return;
        }

        Console.Write("Enter new value: ");
        var newValue = Console.ReadLine();

        var element = elements[index];
        var attr = element.Attribute(attrName);

        if (attr != null)
        {
            attr.Value = newValue;
            Console.WriteLine($"Attribute {attrName} updated.");
            doc.Save(path);
        }
        else
        {
            Console.WriteLine($"Attribute {attrName} not found.");
        }
    }


    static void EditAttributeUsingXmlDocument(string path)
    {
        if (!File.Exists(path))
        {
            Console.WriteLine("XML file not found.");
            return;
        }

        var doc = new XmlDocument();
        doc.Load(path);

        var nodes = doc.SelectNodes("//PC");
        if (nodes == null || nodes.Count == 0)
        {
            Console.WriteLine("No PC elements found in XML.");
            return;
        }

        var availableAttributes = nodes[0].Attributes
            .Cast<XmlAttribute>()
            .Select(a => a.Name)
            .ToList();

        Console.WriteLine("Available attributes to edit: " + string.Join(", ", availableAttributes));

        string attrName = ReadUntilSuccess("Enter attribute name: ", value =>
        {
            if (!availableAttributes.Contains(value))
                throw new ArgumentException("Invalid attribute name");
            return value;
        });

        int index = ReadUntilSuccess("Enter element number (starting from 1): ", int.Parse) - 1;

        Console.Write("Enter new value: ");
        var newValue = Console.ReadLine();

        if (index < 0 || index >= nodes.Count)
        {
            Console.WriteLine("Invalid element number.");
            return;
        }

        var node = nodes[index];
        var attr = node.Attributes[attrName];

        if (attr != null)
        {
            attr.Value = newValue;
            Console.WriteLine($"Attribute {attrName} updated.");
            doc.Save(path);
        }
        else
        {
            Console.WriteLine($"Attribute {attrName} not found.");
        }
    }

    
    private static T ReadUntilSuccess<T>(string message, Func<string, T?> func)
    {
        return ReadUntilSuccess(() =>
        {
            Console.WriteLine(message);
            var value = Console.ReadLine();
            ArgumentException.ThrowIfNullOrEmpty(value);
            var result = func(value);
            ArgumentNullException.ThrowIfNull(result);
            return result;
        });
    }

    private static T ReadUntilSuccess<T>(Func<T> func)
    {
        while (true)
        {
            try
            {
                return func();
            }
            catch (Exception e)
            {
                Console.WriteLine("Invalid input, try again");
                Console.WriteLine(e.Message);
            }
        }
    }
}