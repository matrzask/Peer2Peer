/*var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.Run();*/

class Program
{
    static void Main()
    {
        //target: "Abc1"
        var hasher = new Hasher("ed785a28c1647704a8bdee6af557ac7e", HashAlgorithmType.MD5);
        var workerNode = new WorkerNode(hasher);

        var charset = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789".ToCharArray();
        for (int length = 1; length <= 20; length++)
        {
            Console.WriteLine($"Working on passwords of length {length}...");
            var chunk = new WorkChunk(new string(charset[0], length), new string(charset[charset.Length - 1], length), charset);
            if (workerNode.StartWork(chunk))
            {
                Console.WriteLine("Work completed.");
                break;
            };
        }
    }
}