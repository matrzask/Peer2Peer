using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Peer2Peer.Messages;
using Peer2Peer.Nodes;

namespace Peer2Peer.Network
{
    public class TCPSender
    {
        public TCPSender()
        {
        }

        public void SendMessage(Node node, Message message)
        {
            try
            {
                using (TcpClient client = new TcpClient(node.Ip, node.ListeningPort))
                using (NetworkStream stream = client.GetStream())
                {
                    string serializedMessage = message.Serialize();
                    byte[] data = Encoding.UTF8.GetBytes(serializedMessage);
                    stream.Write(data, 0, data.Length);
                    Console.WriteLine($"{message.Type} sent to {node.Ip}:{node.ListeningPort}");
                }
            }
            catch (SocketException ex)
            {
                Console.WriteLine($"Error sending {message.Type}: {ex.Message}");
                throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending {message.Type}: {ex.Message}");
            }
        }
    }
}

