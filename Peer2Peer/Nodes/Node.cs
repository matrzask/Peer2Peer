using System.Numerics;
using System.Text.Json;
using Peer2Peer.Helpers;
using Peer2Peer.Messages;
using Peer2Peer.Network;

namespace Peer2Peer.Nodes
{
    public class Node
    {
        public string NodeId { get; set; }
        public int ListeningPort { get; set; }
        public string Ip { get; set; }
        protected List<Node> nodes = new List<Node>();
        protected Node? coordinator;
        private readonly HashSet<string> _completedChunks = new HashSet<string>();
        TCPSender sender = new TCPSender();

        public Node() { }

        protected void SendMessage(Message message, Node node)
        {
            sender.SendMessage(node, message);
        }

        public virtual void WorkCompleted(WorkChunk chunk, string workerId)
        {
            string chunkString = chunk.Length + ":" + chunk.Start;
            _completedChunks.Add(chunkString);

        }

        public void SendNodeRegistry(Node node)
        {
            sender.SendMessage(node, new NodeRegistryMessage(this, JsonSerializer.Serialize(nodes)));
            sender.SendMessage(node, new SetCoordinatorMessage(this, JsonSerializer.Serialize(coordinator)));
        }

        public void SetNodeRegistry(List<Node> nodes)
        {
            this.nodes = nodes;
            nodes.Add(this);
            foreach (Node node in nodes)
            {
                if (node.NodeId != NodeId)
                {
                    sender.SendMessage(node, new NewNodeMessage(this, ""));
                }
            }
        }

        public void SetCoordinator(Node coordinator)
        {
            Console.WriteLine($"Node {NodeId} received new coordinator: {coordinator.NodeId}");
            this.coordinator = coordinator;
        }

        public void AddNode(Node node)
        {
            nodes.Add(node);
        }

        public void Connect(string ip, int port)
        {
            sender.SendMessage(new Node { Ip = ip, ListeningPort = port }, new ConnectMessage(this, ""));
        }
    }
}