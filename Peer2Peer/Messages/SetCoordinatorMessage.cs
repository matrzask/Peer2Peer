using System.Text.Json;
using Peer2Peer.Nodes;

namespace Peer2Peer.Messages
{
    public class SetCoordinatorMessage : Message
    {
        public SetCoordinatorMessage(Node sender, string payload) : base(sender, payload)
        {
        }
        public override void Execute(Node node)
        {
            try
            {
                Node coordinator = JsonSerializer.Deserialize<Node>(Payload);
                node.SetCoordinator(coordinator);

                if (node is WorkerNode workerNode)
                {
                    workerNode.Start();
                }
            }
            catch (JsonException e)
            {
                Console.WriteLine("Error deserializing nodes: " + e.Message);
            }
        }
    }
}