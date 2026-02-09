using System.Collections.Concurrent;
using System.Text;
using DeviceLibrary;

namespace ConcurrencyAsynchrony;

public class FileProcessor
{
    private readonly int _totalObjects;
    private readonly int _fileCount;
    private readonly List<string> _filePaths;
    private readonly ConcurrentDictionary<string, ConcurrentBag<string>> _fileDataDict;
    private readonly IProgress<int> _progress;
    private readonly object _consoleLock = new object();

    public FileProcessor(int totalObjects, int fileCount)
    {
        _totalObjects = totalObjects;
        _fileCount = fileCount;
        _filePaths = Enumerable.Range(1, fileCount)
            .Select(i => $"file{i}.txt")
            .ToList();
        _fileDataDict = new ConcurrentDictionary<string, ConcurrentBag<string>>();
        _progress = new Progress<int>(percent =>
        {
            Console.CursorLeft = 0;
            Console.Write($"Progress: {percent}%");
        });
    }

    public async Task RunAsync()
    {
        var pcs = DataGenerator.GeneratePcs(_totalObjects / 2).ToList<IPrintable>();
        var manufacturers = DataGenerator.GenerateManufacturers(_totalObjects / 2).ToList<IPrintable>();
        List<IPrintable> data = pcs.Concat(manufacturers).ToList();
        await WriteFilesAsync(data);
        
        await ReadAndProcessFilesAsync();
        PrintDictionary();
    }

    private async Task WriteFilesAsync(List<IPrintable> data)
    {
        int perFile = data.Count / _fileCount;
        for (int i = 0; i < _fileCount; i++)
        {
            var dataChunk = data.Skip(i * perFile).Take(perFile);
            using var writer = new StreamWriter(_filePaths[i], false, Encoding.UTF8);
            foreach (var obj in dataChunk)
            {
                await writer.WriteLineAsync(obj.PrintObject());
            }
        }
    }

    private async Task ReadAndProcessFilesAsync()
    {
        var readTasks = _filePaths.Select(path => ReadFileAsync(path)).ToArray();

        var sorterTask = Task.Run(async () =>
        {
            while (!Task.WhenAll(readTasks).IsCompleted)
            {
                foreach (var key in _fileDataDict.Keys)
                {
                    var sorted = _fileDataDict[key].ToList();
                    _fileDataDict[key] = new ConcurrentBag<string>(sorted);
                }
                await Task.Delay(500);
            }
        });

        await Task.WhenAll(readTasks);
    }

    private async Task ReadFileAsync(string path)
    {
        var bag = new ConcurrentBag<string>();
        var lines = await File.ReadAllLinesAsync(path);
        int total = lines.Length;

        for (int i = 0; i < lines.Length; i++)
        {
            bag.Add(lines[i]);
            UpdateProgress(path, (i + 1) * 100 / total);
            await Task.Delay(500);
        }


        _fileDataDict[path] = bag;
    }
    
    private void PrintDictionary()
    {
        foreach (var kvp in _fileDataDict)
        {
            Console.WriteLine($"\nFile: {kvp.Key}");
            foreach (var pc in kvp.Value)
            {
                Console.WriteLine(pc);
            }
        }
    }
    
    private void UpdateProgress(string fileName, int percent)
    {
        lock (_consoleLock)
        {
            int line = _filePaths.IndexOf(fileName);
            Console.SetCursorPosition(0, line);
            Console.Write($"{fileName}: {percent}%   ");
        }
    }
}