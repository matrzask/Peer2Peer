/*var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.Run();*/

using System.Net;
using System.Net.Sockets;
using Peer2Peer.Helpers;
using Peer2Peer.Network;
using Peer2Peer.Nodes;

class Program
{
    static void Main()
    {
        string targetHash = "e0d0a8a9779f75750c64a45bb350ea59"; //Abcde
        var charset = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray();
        Console.WriteLine("Enter 'c' to start as Coordinator or 'w' to start as Worker:");
        var input = Console.ReadLine();
        var ip = GetLocalIPAddress();

        if (input.ToLower() == "c")
        {
            var coordinator = new CoordinatorNode(5000000, charset, ip);
            Console.WriteLine($"Coordinator IP: {ip}:{coordinator.ListeningPort}");
            PeerConnection peerConnection = new PeerConnection(coordinator);
            peerConnection.startConnection();
        }
        else if (input.ToLower() == "w")
        {
            Console.WriteLine("Enter the IP address of another node:");
            var coordinatorAddress = Console.ReadLine();
            Console.WriteLine("Enter the port of another node:");
            var coordinatorPort = int.Parse(Console.ReadLine());
            var worker = new WorkerNode(new Hasher(targetHash, HashAlgorithmType.MD5), ip);
            Console.WriteLine($"Worker IP: {ip}:{worker.ListeningPort}");
            PeerConnection peerConnection = new PeerConnection(worker);
            peerConnection.startConnection();
            worker.Connect(coordinatorAddress, coordinatorPort);
        }

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