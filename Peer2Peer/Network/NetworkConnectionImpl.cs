using Peer2Peer.Network;
using Peer2Peer.Messages;
using Peer2Peer.Nodes;

namespace Peer2Peer.Network
{
    public class PeerConnection : NetworkConnection
    {
        private TCPListener tCPListener;
        private TCPSender tCPSender;
        public PeerConnection(Node node){
            this.tCPListener = new TCPListener(node);
            this.tCPSender = new TCPSender();
        }
        public void startConnection(){
            this.tCPListener.StartListening();
        }
        public void stopConnection(){
            this.tCPListener.StopListening();
        }
        public void SendMessageToNode(Node node, Message message){
            this.tCPSender.SendMessage(node, message);
        }
    }
}