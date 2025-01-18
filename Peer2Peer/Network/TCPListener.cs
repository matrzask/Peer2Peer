using System.Net;
using System.Net.Sockets;
using System.Text;
using Peer2Peer.Messages;
using Peer2Peer.Nodes;

namespace Peer2Peer.Network
{
    public class TCPListener
    {
        Node _myNode;
        private TcpListener _listener;
        private bool _isListening;

        public event Action<Message> OnMessageReceived;

        public TCPListener(Node myNode)
        {
            _myNode = myNode;
            OnMessageReceived += (message) => message.Execute(_myNode);
        }

        public void StartListening()
        {
            try
            {
                _listener = new TcpListener(IPAddress.Any, _myNode.ListeningPort);
                _listener.Start();
                _isListening = true;

                Console.WriteLine($"Listening on port {_myNode.ListeningPort}...");

                Task.Run(() => ListenLoop());
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error starting listener: {ex.Message}");
            }
        }

        public void StopListening()
        {
            _isListening = false;
            _listener?.Stop();
            Console.WriteLine("Listener stopped.");
        }

        private void ListenLoop()
        {
            while (_isListening)
            {
                try
                {
                    TcpClient client = _listener.AcceptTcpClient();
                    Task.Run(() => HandleClient(client));
                }
                catch (SocketException ex)
                {
                    if (_isListening)
                    {
                        Console.WriteLine($"Socket exception: {ex.Message}");
                    }
                }
            }
        }

        private void HandleClient(TcpClient client)
        {
            try
            {
                using (NetworkStream stream = client.GetStream())
                using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
                {
                    string messageJson = reader.ReadToEnd();
                    var message = Message.Deserialize(messageJson);
                    Console.WriteLine($"Received {message.Type} from {message.Sender.NodeId}: {message.Payload}");

                    OnMessageReceived?.Invoke(message);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error handling client: {ex.Message}");
            }
            finally
            {
                client.Close();
            }
        }
    }
}