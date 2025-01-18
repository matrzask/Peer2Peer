using Peer2Peer.Helpers;
using Peer2Peer.Messages;
using Peer2Peer.Network;
namespace Peer2Peer.Nodes
{
    class WorkerNode : Node
    {
        private readonly int _chunkSize;
        private readonly int _maxPasswordLength;
        private int _curPasswordLength;
        private readonly char[] _charset;
        private readonly Queue<WorkChunk> _workChunks = new Queue<WorkChunk>();
        private readonly Dictionary<string, Node> _assignedChunks = new Dictionary<string, Node>();
        private bool passwordFound = false;
        private string? _currentChunkHash;
        private volatile bool interrupt = false;


        public WorkerNode(string ip, char[] charset, int chunkSize = 10000000, int minPasswordLength = 1, int maxPasswordLength = 20)
        {
            NodeId = Guid.NewGuid().ToString();
            ListeningPort = new Random().Next(5001, 6000);
            Ip = ip;
            nodes.Add(this);
            _charset = charset;
            _curPasswordLength = minPasswordLength;
            _maxPasswordLength = maxPasswordLength;
            _chunkSize = chunkSize;
            peerConnection = new PeerConnection(this);
            peerConnection.startConnection();
        }

        public void SetHasher(Hasher hasher)
        {
            this.hasher = hasher;
        }

        public void Start()
        {
            while (hasher == null)
            {
                Thread.Sleep(100);
            }
            while (!passwordFound)
            {
                StartWork(GetChunk());
            }
        }

        public void StartWork(WorkChunk chunk)
        {
            _currentChunkHash = chunk.Hash();
            List<Node> nodesCopy;
            lock (_NodesLock)
            {
                nodesCopy = new List<Node>(nodes);
            }
            foreach (Node node in nodesCopy)
            {
                if (node.NodeId != NodeId)
                {
                    SendMessage(new ClaimChunkMessage(this, _currentChunkHash), node);
                }
            }

            lock (_ChunksLock)
            {
                _assignedChunks.Add(chunk.Hash(), this);
            }
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            foreach (string input in chunk.GeneratePasswords())
            {
                if (hasher.Compare(input))
                {
                    stopwatch.Stop();
                    Console.WriteLine($"WorkerNode {NodeId} found the correct input: {input}");
                    lock (_NodesLock)
                    {
                        nodesCopy = new List<Node>(nodes);
                    }
                    foreach (Node node in nodesCopy)
                    {
                        if (node.NodeId != NodeId)
                        {
                            SendMessage(new PasswordFoundMessage(this, input), node);
                        }
                    }

                    passwordFound = true;
                    return;
                }
                if (interrupt)
                {
                    interrupt = false;
                    return;
                }
            }
            stopwatch.Stop();
            Console.WriteLine($"WorkerNode {NodeId} completed work chunk {chunk.Hash()} in {stopwatch.ElapsedMilliseconds} ms");
            WorkCompleted(chunk.Hash());
            lock (_NodesLock)
            {
                nodesCopy = new List<Node>(nodes);
            }
            foreach (Node node in nodesCopy)
            {
                if (node.NodeId != NodeId)
                {
                    SendMessage(new WorkCompletedMessage(this, chunk.Hash()), node);
                }
            }

            return;
        }

        private void GenerateWorkChunks()
        {
            if (_curPasswordLength > _maxPasswordLength)
            {
                throw new InvalidOperationException("No more work to be done.");
            }
            Console.WriteLine($"Generating work chunks for password length {_curPasswordLength}");

            long maxPass = (long)Math.Pow(_charset.Length, _curPasswordLength) - 1;

            for (long i = 0; i < Math.Pow(_charset.Length, _curPasswordLength); i += _chunkSize)
            {
                long chunkEnd = Math.Min(i + _chunkSize - 1, maxPass);
                _workChunks.Enqueue(new WorkChunk(i, chunkEnd, _charset, _curPasswordLength));
            }

            _curPasswordLength++;
        }

        private WorkChunk GetChunk()
        {
            if (_workChunks.Count == 0)
            {
                GenerateWorkChunks();
            }
            WorkChunk chunk = _workChunks.Dequeue();
            lock (_ChunksLock)
            {
                while (_completedChunks.Contains(chunk.Hash()) || _assignedChunks.ContainsKey(chunk.Hash()))
                {
                    if (_workChunks.Count == 0)
                    {
                        GenerateWorkChunks();
                    }
                    chunk = _workChunks.Dequeue();
                }
            }
            return chunk;
        }

        public void HandleChunkClaim(string chunkHash, Node sender)
        {
            if (_currentChunkHash != chunkHash || string.Compare(sender.NodeId, NodeId) < 0)
            {
                lock (_ChunksLock)
                {
                    if (!_assignedChunks.ContainsKey(chunkHash))
                        _assignedChunks.Add(chunkHash, sender);
                }
            }
            else
            {
                interrupt = true;
            }
        }

        public void PasswordFound(string password)
        {
            Console.WriteLine($"Received password found message: {password}");
            passwordFound = true;
        }

        public void WorkCompleted(string chunkHash)
        {
            lock (_ChunksLock)
            {
                _completedChunks.Add(chunkHash);
                _assignedChunks.Remove(chunkHash);
            }
        }
    }
}