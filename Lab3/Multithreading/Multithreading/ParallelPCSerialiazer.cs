using DeviceLibrary;
using System.Diagnostics;
using System.Text;

namespace Multithreading;

public class ParallelPCSerializer
{
    private readonly SemaphoreSlim writeSemaphore = new SemaphoreSlim(1, 1);
    private readonly SemaphoreSlim readSemaphore = new SemaphoreSlim(5, 5);

    private readonly object writeLock = new object();

    public void Task1(List<PC> pcs)
    {
        Console.WriteLine("Task 1");

        Stopwatch sw = Stopwatch.StartNew();

        var firstPart = pcs.GetRange(0, 10);
        var secondPart = pcs.GetRange(10, 10);

        Thread t1 = new Thread(() => WriteToFile("file1.txt", firstPart));
        Thread t2 = new Thread(() => WriteToFile("file2.txt", secondPart));

        t1.Start();
        t2.Start();

        t1.Join();
        t2.Join();

        sw.Stop();
        Console.WriteLine($"[Task 1 | 2 threads write] Time: {sw.ElapsedMilliseconds} ms");
    }

    private void WriteToFile(string fileName, List<PC> pcs)
    {
        using StreamWriter writer = new StreamWriter(fileName);
        foreach (var pc in pcs)
        {
            writer.WriteLine(pc.PrintObject());
        }
    }

    public void Task2(string resultFile)
    {
        Console.WriteLine("Task 2");

        Stopwatch sw = Stopwatch.StartNew();

        using StreamWriter writer = new StreamWriter(resultFile);

        Thread t1 = new Thread(() => ReadAndWrite("file1.txt", writer));
        Thread t2 = new Thread(() => ReadAndWrite("file2.txt", writer));

        t1.Start();
        t2.Start();

        t1.Join();
        t2.Join();

        sw.Stop();
        Console.WriteLine($"[Task 2 | 2 threads read+write] Time: {sw.ElapsedMilliseconds} ms");
    }


    private void ReadAndWrite(string sourceFile, StreamWriter writer)
    {
        using StreamReader reader = new StreamReader(sourceFile);

        while (!reader.EndOfStream)
        {
            string line = reader.ReadLine();

            writeSemaphore.Wait();
            try
            {
                writer.WriteLine(line);
            }
            finally
            {
                writeSemaphore.Release();
            }
        }
    }
    
    public void Task3_1(string file)
    {
        Console.WriteLine("Task 3.1");
        Stopwatch sw = Stopwatch.StartNew();

        using StreamReader reader = new StreamReader(file);
        while (!reader.EndOfStream)
        {
            Console.WriteLine(reader.ReadLine());
        }

        sw.Stop();
        Console.WriteLine($"[1 thread]: {sw.ElapsedMilliseconds} ms");
    }
    
    public void Task3_2(string file)
    {
        Console.WriteLine("Task 3.2");

        string[] lines = File.ReadAllLines(file);
        int middle = lines.Length / 2;

        Stopwatch sw = Stopwatch.StartNew();

        Thread t1 = new Thread(() =>
        {
            for (int i = 0; i < middle; i++)
            {
                readSemaphore.Wait();
                try
                {
                    Console.WriteLine(lines[i]);
                }
                finally
                {
                    readSemaphore.Release();
                }
            }
        });

        Thread t2 = new Thread(() =>
        {
            for (int i = middle; i < lines.Length; i++)
            {
                readSemaphore.Wait();
                try
                {
                    Console.WriteLine(lines[i]);
                }
                finally
                {
                    readSemaphore.Release();
                }
            }
        });

        t1.Start();
        t2.Start();

        t1.Join();
        t2.Join();

        sw.Stop();
        Console.WriteLine($"[2 threads] Time: {sw.ElapsedMilliseconds} ms");
    }


    public void Task3_3(string file)
    {
        Console.WriteLine("Task 3.3");

        int threadCount = 10;
        Thread[] threads = new Thread[threadCount];

        Stopwatch sw = Stopwatch.StartNew();

        for (int i = 0; i < threadCount; i++)
        {
            threads[i] = new Thread(() =>
            {
                readSemaphore.Wait();
                try
                {
                    using StreamReader reader = new StreamReader(file);
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        Console.WriteLine(line);
                    }
                }
                finally
                {
                    readSemaphore.Release();
                }
            });

            threads[i].Start();
        }

        foreach (var t in threads)
            t.Join();

        sw.Stop();
        Console.WriteLine($"[10 threads, semaphore=5] Time: {sw.ElapsedMilliseconds} ms");
    }

}
