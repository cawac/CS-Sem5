using ConcurrencyAsynchrony;

class Program
{
    static async Task Main()
    {
        var processor = new FileProcessor(totalObjects: 50, fileCount: 5);
        await processor.RunAsync();
    }
}