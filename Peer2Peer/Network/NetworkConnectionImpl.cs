using Peer2Peer.Messages;
using Peer2Peer.Nodes;

namespace Peer2Peer.Network
{
    public class PeerConnection : NetworkConnection
    {
        private TCPListener tCPListener;
        private TCPSender tCPSender;
        public PeerConnection(Node node)
        {
            tCPListener = new TCPListener(node);
            tCPSender = new TCPSender();
        }
        public void startConnection()
        {
            tCPListener.StartListening();
        }
        public void stopConnection()
        {
            tCPListener.StopListening();
        }
        public void SendMessageToNode(Node node, Message message)
        {
            tCPSender.SendMessage(node, message);
        }
    }
}