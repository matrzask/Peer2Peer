using System.Text.Json;
using Peer2Peer.Helpers;
using Peer2Peer.Messages;
namespace Peer2Peer.Nodes
{
    class WorkerNode : Node
    {
        private Hasher hasher;

        public WorkerNode(Hasher hasher, string ip)
        {
            this.hasher = hasher;
            NodeId = Guid.NewGuid().ToString();
            ListeningPort = new Random().Next(5001, 6000);
            Ip = ip;
            nodes.Add(this);
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
                    Console.WriteLine($"WorkerNode {NodeId} found the correct input: {input}");
                    foreach (Node node in nodes)
                    {
                        if (node.NodeId != NodeId)
                        {
                            SendMessage(new PasswordFoundMessage(this, input), node);
                        }
                    }
                    return;
                }
            }
            stopwatch.Stop();
            Console.WriteLine($"WorkerNode {NodeId} completed work chunk in {stopwatch.ElapsedMilliseconds} ms");
            WorkCompleted(chunk, NodeId);
            foreach (Node node in nodes)
            {
                if (node.NodeId != NodeId)
                {
                    SendMessage(new WorkCompletedMessage(this, JsonSerializer.Serialize(chunk)), node);
                }
            }
            //SendMessage(new RequestWorkMessage(this, ""), coordinator);
            return;
        }
    }
}