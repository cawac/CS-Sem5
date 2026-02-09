using DeviceLibrary;

public static class Program
{
    public static async Task Main()
    {
        var tasks = new TplTasks();

        while (true)
        {
            Console.WriteLine("1 - Task 1");
            Console.WriteLine("2 - Task 2");
            Console.WriteLine("3 - Task 3");
            Console.WriteLine("0 - Exit");

            var key = Console.ReadLine();

            switch (key)
            {
                case "1":
                    tasks.Task1_Generate_And_Write();
                    Console.WriteLine("Files 1 and 2 have been created");
                    break;

                case "2":
                    tasks.Task2_Read_And_Merge();
                    Console.WriteLine("File 3 has been created");
                    break;

                case "3":
                    await tasks.Task3_Read_Parallel_Print();
                    break;

                case "0":
                    return;
            }
        }
    }
}
