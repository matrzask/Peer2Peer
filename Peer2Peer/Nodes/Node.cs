using System.Text.Json;
using Peer2Peer.Messages;
using Peer2Peer.Network;

namespace Peer2Peer.Nodes
{
    public class Node
    {
        public string? NodeId { get; set; }
        public int ListeningPort { get; set; }
        public string Ip { get; set; }
        protected List<Node> nodes = new List<Node>();
        protected readonly HashSet<string> _completedChunks = new HashSet<string>();
        protected readonly object _ChunksLock = new object();


        protected PeerConnection peerConnection;


        public Node() { }

        protected void SendMessage(Message message, Node node)
        {
            peerConnection.SendMessageToNode(node, message);
        }

        public void SendNodeRegistry(Node node)
        {
            SendMessage(new NodeRegistryMessage(this, JsonSerializer.Serialize(nodes)), node);
            SendMessage(new CompletedChunksMessage(this, JsonSerializer.Serialize(_completedChunks)), node);
        }

        public void SetNodeRegistry(List<Node> nodes)
        {
            this.nodes = nodes;
            nodes.Add(this);
            foreach (Node node in nodes)
            {
                if (node.NodeId != NodeId)
                {
                    SendMessage(new NewNodeMessage(this, ""), node);
                }
            }
        }

        public void AddNode(Node node)
        {
            nodes.Add(node);
        }

        public void Connect(string ip, int port)
        {
            SendMessage(new ConnectMessage(this, ""), new Node { Ip = ip, ListeningPort = port });
        }

        public void SetCompletedChunks(List<string> completedChunks)
        {
            lock (_ChunksLock)
            {
                _completedChunks.UnionWith(completedChunks);
            }
        }
    }
}