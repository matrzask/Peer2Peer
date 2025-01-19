using System.Net.Sockets;
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
                ProgressStatus.CompletedChunks[ProgressStatus.CompletedChunks.Count - 1] = ProgressStatus.TotalChunks[ProgressStatus.CompletedChunks.Count - 1] - _workChunks.Count;
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
            var collection = chunk.GeneratePasswords();
            foreach (string input in collection)
            {
                if (hasher.Compare(input))
                {
                    stopwatch.Stop();
                    Console.WriteLine($"WorkerNode {NodeId} found the correct input: {input}");
                    ProgressStatus.FoundPassword = input;
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
            if (ProgressStatus.CompletedChunks.Count > 0)
            {
                ProgressStatus.CompletedChunks[ProgressStatus.CompletedChunks.Count - 1] = ProgressStatus.TotalChunks[ProgressStatus.CompletedChunks.Count - 1];
            }

            ProgressStatus.TotalChunks.Add(_workChunks.Count);
            ProgressStatus.CompletedChunks.Add(0);

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
                lock (_ChunksLock)
                {
                    if (_assignedChunks.ContainsKey(chunkHash))
                        _assignedChunks[chunkHash] = sender;
                }
                interrupt = true;
            }
        }

        public void PasswordFound(string password)
        {
            Console.WriteLine($"Received password found message: {password}");
            passwordFound = true;
            ProgressStatus.FoundPassword = password;
        }

        public void WorkCompleted(string chunkHash)
        {
            lock (_ChunksLock)
            {
                _completedChunks.Add(chunkHash);
                _assignedChunks.Remove(chunkHash);
            }
        }

        protected override void SendMessage(Message message, Node node)
        {
            try
            {
                peerConnection.SendMessageToNode(node, message);
            }
            catch (SocketException)
            {
                lock (_NodesLock)
                {
                    nodes.Remove(node);
                }
                lock (_ChunksLock)
                {
                    var chunkString = _assignedChunks.FirstOrDefault(x => x.Value.NodeId == node.NodeId).Key;
                    if (chunkString != null)
                    {
                        _workChunks.Enqueue(new WorkChunk(chunkString, _charset));
                        _assignedChunks.Remove(chunkString);
                        Console.WriteLine($"Reassigned chunk {chunkString}");
                    }
                }
            }
        }
    }
}