using System.Text.Json;
using Peer2Peer.Nodes;

namespace Peer2Peer.Messages
{
    public class NodeRegistryMessage : Message
    {
        public NodeRegistryMessage(Node sender, string payload) : base(sender, payload)
        {
        }
        public override void Execute(Node node)
        {
            try
            {
                List<Node> nodes = JsonSerializer.Deserialize<List<Node>>(Payload);
                node.SetNodeRegistry(nodes);
            }
            catch (JsonException e)
            {
                Console.WriteLine("Error deserializing nodes: " + e.Message);
            }
        }
    }
}