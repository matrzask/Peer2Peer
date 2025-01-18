/*var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.Run();*/

using System.Net;
using System.Net.Sockets;
using Peer2Peer.Helpers;
using Peer2Peer.Nodes;

class Program
{
    static void Main()
    {
        var charset = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789".ToCharArray();
        string targetHash = "e604fb2072c286d1fb6378c5cde74ca0c99f3ba1d9f4cef58969020efbc2382e"; //Pass1
        HashAlgorithmType algorithmType = HashAlgorithmType.SHA256;

        Console.WriteLine("Enter node ip to connect, or leave empty to start as a single node:");
        var input = Console.ReadLine();
        var ip = GetLocalIPAddress();

        WorkerNode worker = new WorkerNode(ip, charset);

        if (!string.IsNullOrEmpty(input))
        {
            Console.WriteLine("Enter target port:");
            var port = int.Parse(Console.ReadLine() ?? "0");
            worker.Connect(input, port);
        }
        else
        {
            var hasher = new Hasher(targetHash, algorithmType);
            worker.SetHasher(hasher);
        }

        Console.WriteLine($"Node ip: {ip}:{worker.ListeningPort}");

        worker.Start();


        Console.WriteLine("Press Enter to exit...");
        Console.ReadLine();
    }

    public static string GetLocalIPAddress()
    {
        var host = Dns.GetHostEntry(Dns.GetHostName());
        foreach (var ip in host.AddressList)
        {
            if (ip.AddressFamily == AddressFamily.InterNetwork)
            {
                return ip.ToString();
            }
        }
        throw new Exception("No network adapters with an IPv4 address in the system!");
    }
}