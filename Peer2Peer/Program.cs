/*var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.Run();*/

class Program
{
    static void Main()
    {
        string targetHash = "ed785a28c1647704a8bdee6af557ac7e"; //Abc1
        var charset = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789".ToCharArray();
        var coordinator = new CoordinatorNode(500000, charset);
        var worker = new WorkerNode(new Hasher(targetHash, HashAlgorithmType.MD5));

        while (true)
        {
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            coordinator.HandleWorkerNode(worker);
            stopwatch.Stop();
            Console.WriteLine($"Chunk time taken: {stopwatch.ElapsedMilliseconds} ms");
        }
    }
}