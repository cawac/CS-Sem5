using System.Reflection;

class Program
{
    static void Main()
    {
        var assembly = RunUntilSuccess(
            "Enter path to the DLL file:",
            path => File.Exists(path) ? Assembly.LoadFrom(path) : null
        );

        Console.WriteLine("DLL analysis");

        foreach (var type in assembly.GetTypes().Where(t => t.IsClass))
        {
            Console.WriteLine($"  Class: {type.FullName}");

            var properties = type.GetProperties(
                BindingFlags.Public |
                BindingFlags.NonPublic |
                BindingFlags.Instance
            );

            if (properties.Length == 0)
            {
                Console.WriteLine("No properties");
                continue;
            }

            foreach (var prop in properties)
            {
                var getter = prop.GetMethod;

                string accessModifier = getter switch
                {
                    null => "unknown",
                    _ when getter.IsPublic   => "public",
                    _ when getter.IsPrivate  => "private",
                    _ when getter.IsFamily   => "protected",
                    _                        => "unknown"
                };

                Console.WriteLine(
                    $"    {accessModifier} {prop.PropertyType.Name} {prop.Name}"
                );
            }
            Console.WriteLine();
        }
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
