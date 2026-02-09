using System.Text;
using DeviceLibrary;

public class TplTasks
{
    private readonly object locker = new object();

    public void Task1_Generate_And_Write()
    {
        var pcs = DataGenerator.GeneratePcs(20);
        var manufacturers = DataGenerator.GenerateManufacturers(20);

        List<IPrintable> first = pcs.Take(10).ToList<IPrintable>().Concat(manufacturers.Take(10)).ToList();
        List<IPrintable> second = pcs.Skip(10).Take(10).ToList<IPrintable>().Concat(manufacturers.Skip(10)).ToList();

        Task t1 = Task.Run(() => Write("file1.txt", first));
        Task t2 = Task.Run(() => Write("file2.txt", second));

        Task.WaitAll(t1, t2);
    }

    public void Task2_Read_And_Merge()
    {
        List<string> list1 = null;
        List<string> list2 = null;

        Task t1 = Task.Run(() => list1 = Read("file1.txt"));
        Task t2 = Task.Run(() => list2 = Read("file2.txt"));

        Task.WaitAll(t1, t2);
        
        var result = list1.Concat(list2);

        File.WriteAllLines("file3.txt", result, Encoding.UTF8);
    }

    private static readonly SemaphoreSlim fileReadLock = new SemaphoreSlim(1, 1);

    public async Task Task3_Read_Parallel_Print()
    {
        int taskCount = 5;

        var tasks = new List<Task>();

        for (int i = 0; i < taskCount; i++)
        {
            tasks.Add(Task.Run(async () =>
            {
                using FileStream fs = new FileStream(
                    "file3.txt",
                    FileMode.Open,
                    FileAccess.Read,
                    FileShare.ReadWrite,
                    4096,
                    true);

                using StreamReader reader = new StreamReader(fs, Encoding.UTF8);

                string line;

                while (true)
                {
                    await fileReadLock.WaitAsync();
                    try
                    {
                        line = await reader.ReadLineAsync();
                    }
                    finally
                    {
                        fileReadLock.Release();
                    }

                    if (line == null)
                        break;

                    lock (Console.Out)
                    {
                        Console.WriteLine(line);
                    }
                }
            }));
        }

        await Task.WhenAll(tasks);
    }

    
    private void Write(string path, List<IPrintable> list)
    {
        using StreamWriter writer = new StreamWriter(path, false, Encoding.UTF8);

        foreach (var item in list)
        {
            writer.WriteLine(item.PrintObject());
        }
    }

    private List<string> Read(string path)
    {
        return File.ReadAllLines(path, Encoding.UTF8).ToList();
    }
}