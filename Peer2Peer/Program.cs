/*var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.Run();*/

using Peer2Peer.Nodes;

class Program
{
    static void Main()
    {
        string targetHash = "ed785a28c1647704a8bdee6af557ac7e"; //Abc1
        var charset = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789?!.".ToCharArray();
        var coordinator = new CoordinatorNode(500000, charset);
        var worker = new WorkerNode(new Hasher(targetHash, HashAlgorithmType.MD5), coordinator);

        worker.Start();
    }
}