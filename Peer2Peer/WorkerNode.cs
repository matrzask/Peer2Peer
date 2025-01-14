using System.Text.Json;

class WorkerNode : Node
{
    public string Id { get; set; }
    private Hasher hasher;
    private Node coordinator;

    public WorkerNode(Hasher hasher, Node coordinator)
    {
        this.hasher = hasher;
        this.coordinator = coordinator;
        Id = Guid.NewGuid().ToString();
    }

    public void Start()
    {
        SendMessenge(new Message(this, "", MessageType.RequestWork), coordinator);
    }

    private void StartWork(WorkChunk chunk)
    {
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        foreach (string input in chunk.GeneratePasswords())
        {
            if (hasher.Compare(input))
            {
                stopwatch.Stop();
                Console.WriteLine($"WorkerNode {Id} found the correct input: {input}");
                SendMessenge(new Message(this, input, MessageType.PasswordFound), coordinator);
                return;
            }
        }
        stopwatch.Stop();
        Console.WriteLine($"WorkerNode {Id} completed work chunk in {stopwatch.ElapsedMilliseconds} ms");
        SendMessenge(new Message(this, JsonSerializer.Serialize(chunk), MessageType.WorkCompleted), coordinator);
        SendMessenge(new Message(this, "", MessageType.RequestWork), coordinator);
        return;
    }

    public override void ReceiveMessage(Message message)
    {
        if (message.Type == MessageType.AssignWork)
        {
            WorkChunk chunk = JsonSerializer.Deserialize<WorkChunk>(message.Payload);
            if (chunk != null)
            {
                StartWork(chunk);
            }
            else
            {
                Console.WriteLine("Failed to deserialize WorkChunk from message payload.");
            }
        }
    }
}