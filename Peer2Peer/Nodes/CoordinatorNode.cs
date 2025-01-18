using System.Text.Json;
using Peer2Peer.Helpers;
using Peer2Peer.Messages;

namespace Peer2Peer.Nodes
{
    class CoordinatorNode : Node
    {
        private readonly int _chunkSize;
        private readonly int _maxPasswordLength;
        private int _curPasswordLength;
        private readonly char[] _charset;
        private readonly Queue<WorkChunk> _workChunks = new Queue<WorkChunk>();
        private readonly Dictionary<string, WorkChunk> _assignedChunks = new Dictionary<string, WorkChunk>();
        private bool passwordFound = false;

        public CoordinatorNode(int chunkSize, char[] charset, string ip, int minPasswordLength = 1, int maxPasswordLength = 20)
        {
            _chunkSize = chunkSize;
            _charset = charset;
            _curPasswordLength = minPasswordLength;
            _maxPasswordLength = maxPasswordLength;
            Ip = ip;
            ListeningPort = new Random().Next(5001, 6000);
            NodeId = Guid.NewGuid().ToString();
            coordinator = this;
            nodes.Add(this);
        }

        public void AssignChunk(Node node, WorkChunk chunk)
        {
            Message message = new AssignWorkMessage(this, JsonSerializer.Serialize(chunk));
            _assignedChunks.Add(node.NodeId, chunk);
            SendMessage(message, node);
        }

        public void HandleWorkerNode(Node node)
        {
            if (passwordFound)
            {
                return;
            }
            if (_workChunks.Count == 0)
            {
                GenerateWorkChunks();
            }

            WorkChunk chunk = _workChunks.Dequeue();
            AssignChunk(node, chunk);
        }

        private void GenerateWorkChunks()
        {
            if (_curPasswordLength > _maxPasswordLength)
            {
                throw new InvalidOperationException("No more work to be done.");
            }

            long maxPass = (long)Math.Pow(_charset.Length, _curPasswordLength) - 1;

            for (long i = 0; i < Math.Pow(_charset.Length, _curPasswordLength); i += _chunkSize)
            {
                long chunkEnd = Math.Min(i + _chunkSize - 1, maxPass);
                _workChunks.Enqueue(new WorkChunk(i, chunkEnd, _charset, _curPasswordLength));
            }

            _curPasswordLength++;
        }

        public override void WorkCompleted(WorkChunk chunk, string workerId)
        {
            base.WorkCompleted(chunk, workerId);
            if (_assignedChunks.ContainsKey(workerId) && _assignedChunks[workerId].Start == chunk.Start && _assignedChunks[workerId].Length == chunk.Length)
            {
                _assignedChunks.Remove(workerId);
            }
            else
            {
                Console.WriteLine("Worker node completed work on an unassigned chunk.");
            }
        }

        public void PasswordFound(string password)
        {
            Console.WriteLine($"CoordinatorNode received password found message: {password}");
            passwordFound = true;
        }
    }
}