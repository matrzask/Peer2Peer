using System.Text.Json;

class CoordinatorNode : Node
{
    private readonly int _chunkSize;
    private readonly int _maxPasswordLength;
    private int _curPasswordLength;
    private readonly char[] _charset;
    private readonly Queue<WorkChunk> _workChunks = new Queue<WorkChunk>();
    private bool passwordFound = false;

    public CoordinatorNode(int chunkSize, char[] charset, int minPasswordLength = 1, int maxPasswordLength = 20)
    {
        _chunkSize = chunkSize;
        _charset = charset;
        _curPasswordLength = minPasswordLength;
        _maxPasswordLength = maxPasswordLength;
    }

    public void AssignChunk(WorkerNode node, WorkChunk chunk)
    {
        SendMessenge(new Message(this, JsonSerializer.Serialize(chunk), MessageType.AssignWork), node);
    }

    public void HandleWorkerNode(WorkerNode node)
    {
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

    public override void ReceiveMessage(Message message)
    {
        switch (message.Type)
        {
            case MessageType.PasswordFound:
                Console.WriteLine($"CoordinatorNode received password found message: {message.Payload}");
                passwordFound = true;
                break;

            case MessageType.WorkCompleted:
                WorkChunk chunk = JsonSerializer.Deserialize<WorkChunk>(message.Payload);
                if (chunk != null)
                {
                    // Add to completed
                }
                else
                {
                    Console.WriteLine("Failed to deserialize WorkChunk from message payload.");
                }
                break;

            case MessageType.RequestWork:
                if (!passwordFound)
                {
                    HandleWorkerNode((WorkerNode)message.Sender);
                }
                break;

            default:
                Console.WriteLine("Unknown message type received.");
                break;
        }
    }
}