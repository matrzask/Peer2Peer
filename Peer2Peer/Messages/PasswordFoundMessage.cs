using Peer2Peer.Nodes;
using System.Text.Json;

namespace Peer2Peer.Messages
{
    public class PasswordFoundMessage : Message
    {
        public PasswordFoundMessage(Node sender, string payload) : base(sender, payload)
        {
        }
        public override void Execute(Node node)
        {
            if (node is WorkerNode worker)
            {
                worker.PasswordFound(Payload);
            }
        }
    }
}