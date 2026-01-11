using System.Reflection;

class Program
{
    static void Main()
    {
        var assembly = RunUntilSuccess(
            "Enter path to the DLL file:",
            path => File.Exists(path) ? Assembly.LoadFrom(path) : null
        );
        
        var type = RunUntilSuccess(
            "Enter class name:",
            name => assembly.GetTypes().FirstOrDefault(t => t.Name == name)
        );

        var instance = Activator.CreateInstance(type);

        var method = RunUntilSuccess(
            "Enter method name:",
            name => type.GetMethod(name)
        );

        var parameters = method.GetParameters()
            .Select(p =>
            {
                Console.WriteLine($"Enter parameter {p.Name} ({p.ParameterType.Name}):");
                var value = Console.ReadLine();
                return Convert.ChangeType(value, p.ParameterType);
            })
            .ToArray();

        var result = method.Invoke(instance, parameters);

        if (method.ReturnType != typeof(void))
            Console.WriteLine($"Result: {result}");
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