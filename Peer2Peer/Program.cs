/*var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.Run();*/

class Program
{
    static void Main()
    {
        //target: "Ab1"
        var hasher = new Hasher("c99f1621ddb8d08a4cac6ee4b6989349", HashAlgorithmType.MD5);
        var workerNode = new WorkerNode(hasher);

        var charset = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789".ToCharArray();
        for (int length = 1; length <= 20; length++)
        {
            Console.WriteLine($"Working on passwords of length {length}...");
            var chunk = new WorkChunk(new string(charset[0], length), new string(charset[charset.Length - 1], length), charset);
            //var chunk = new WorkChunk(0, (long)Math.Pow(charset.Length, length), charset, length);
            if (workerNode.StartWork(chunk))
            {
                Console.WriteLine("Work completed.");
                break;
            }
            ;
        }
    }
}