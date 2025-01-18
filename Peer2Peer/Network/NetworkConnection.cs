using Peer2Peer.Messages;
using Peer2Peer.Nodes;

namespace Peer2Peer.Network
{
    public interface NetworkConnection
    {
        void startConnection();
        void stopConnection();
        void SendMessageToNode (Node node, Message message);
    }
}