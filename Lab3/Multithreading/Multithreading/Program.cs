using DeviceLibrary;

namespace Multithreading;

public static class Program
{
    public static void Main()
    {
        var pcs = GeneratePCs();

        ParallelPCSerializer serializer = new ParallelPCSerializer();
        serializer.Task1(pcs);
        serializer.Task2("file3.txt");
        serializer.Task3_1("file3.txt");
        serializer.Task3_2("file3.txt");
        serializer.Task3_3("file3.txt");
    }

    static List<PC> GeneratePCs()
    {
        var pcs = new List<PC>();

        for (int i = 1; i <= 20; i++)
        {
            pcs.Add(PC.Create(
                i,
                $"Model_{i}",
                $"SN-{1000 + i}",
                (PCType)(i % 3)
            ));
        }

        return pcs;
    }
}