using System.Net.Sockets;
using System.Text.Json;
using Peer2Peer.Helpers;
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
        protected readonly object _NodesLock = new object();
        protected PeerConnection peerConnection;
        protected Hasher? hasher;


        public Node() { }

        protected virtual void SendMessage(Message message, Node node)
        {
            try
            {
                peerConnection.SendMessageToNode(node, message);
            }
            catch (SocketException)
            {
                lock (_NodesLock)
                {
                    nodes.Remove(node);
                }
            }
        }

        public void SendInfo(Node node)
        {
            SendMessage(new NodeRegistryMessage(this, JsonSerializer.Serialize(nodes)), node);
            SendMessage(new CompletedChunksMessage(this, JsonSerializer.Serialize(_completedChunks)), node);
            SendMessage(new SetHasherMessage(this, JsonSerializer.Serialize(hasher)), node);
        }

        public void SetNodeRegistry(List<Node> nodes)
        {
            List<Node> nodesCopy;
            lock (_NodesLock)
            {
                this.nodes = nodes;
                nodes.Add(this);
                nodesCopy = new List<Node>(nodes);
            }
            foreach (Node node in nodesCopy)
            {
                if (node.NodeId != NodeId)
                {
                    SendMessage(new NewNodeMessage(this, ""), node);
                }
            }
        }

        public void AddNode(Node node)
        {
            lock (_NodesLock)
            {
                nodes.Add(node);
            }
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