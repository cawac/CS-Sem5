using System.Reflection;

class Program
{
    static void Main()
    {
        var assembly = RunUntilSuccess(
            "Enter the path to DeviceLibrary.dll:",
            path => File.Exists(path) ? Assembly.LoadFrom(path) : null
        );

        var type = RunUntilSuccess(
            "Enter class name (PC or Manufacturer):",
            name => assembly.GetTypes().FirstOrDefault(t => t.Name == name)
        );

        var instance = Activator.CreateInstance(type);

        var createMethod = type.GetMethod("Create");

        var parameters = createMethod.GetParameters()
            .Select(p =>
            {
                // Если параметр — enum, выводим все возможные значения
                if (p.ParameterType.IsEnum)
                {
                    var enumValues = Enum.GetNames(p.ParameterType);
                    Console.WriteLine($"Possible values for {p.Name} ({p.ParameterType.Name}): {string.Join(", ", enumValues)}");
                }

                Console.WriteLine($"Enter {p.Name} ({p.ParameterType.Name}):");
                var value = Console.ReadLine();

                if (p.ParameterType.IsEnum)
                {
                    return Enum.Parse(p.ParameterType, value, ignoreCase: true);
                }

                return Convert.ChangeType(value, p.ParameterType);

            })
            .ToArray();

        createMethod.Invoke(instance, parameters);

        var printMethod = type.GetMethod("PrintObject");
        var result = printMethod.Invoke(instance, null);

        Console.WriteLine("\nResult:");
        Console.WriteLine(result);
    }

    private static T RunUntilSuccess<T>(string message, Func<string, T?> func)
    {
        return RunUntilSuccess(() =>
        {
            Console.WriteLine(message);
            var value = Console.ReadLine();
            ArgumentException.ThrowIfNullOrEmpty(value);
            var result = func(value);
            ArgumentNullException.ThrowIfNull(result);
            return result;
        });
    }

    private static T RunUntilSuccess<T>(Func<T> func)
    {
        while (true)
        {
            try
            {
                return func();
            }
            catch (Exception e)
            {
                Console.WriteLine("Something went wrong, try again");
                Console.WriteLine(e.Message);
            }
        }
    }
}
