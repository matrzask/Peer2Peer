using System.Text.Json;
using Peer2Peer.Helpers;
using Peer2Peer.Messages;
namespace Peer2Peer.Nodes
{
    class WorkerNode : Node
    {
        private Hasher hasher;
        private Node coordinator;

        public WorkerNode(Hasher hasher, Node coordinator, string ip)
        {
            this.hasher = hasher;
            this.coordinator = coordinator;
            nodeId = Guid.NewGuid().ToString();
            ListenigPort = new Random().Next(5001, 6000);
            Ip = ip;
        }

        public void Start()
        {
            SendMessage(new RequestWorkMessage(this, ""), coordinator);
        }

        public void StartWork(WorkChunk chunk)
        {
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            foreach (string input in chunk.GeneratePasswords())
            {
                if (hasher.Compare(input))
                {
                    stopwatch.Stop();
                    Console.WriteLine($"WorkerNode {nodeId} found the correct input: {input}");
                    SendMessage(new PasswordFoundMessage(this, input), coordinator);
                    return;
                }
            }
            stopwatch.Stop();
            Console.WriteLine($"WorkerNode {nodeId} completed work chunk in {stopwatch.ElapsedMilliseconds} ms");
            WorkCompleted(chunk, nodeId);
            SendMessage(new WorkCompletedMessage(this, JsonSerializer.Serialize(chunk)), coordinator); //Should broadcast to all nodes
            SendMessage(new RequestWorkMessage(this, ""), coordinator);
            return;
        }
    }
}