class WorkerNode : Node
{
    public string Id { get; set; }
    private Hasher hasher;

    public WorkerNode(Hasher hasher)
    {
        this.hasher = hasher;
        Id = Guid.NewGuid().ToString();
    }

    public bool StartWork(WorkChunk chunk)
    {
        foreach (string input in chunk.GeneratePasswords())
        {
            if (hasher.Compare(input))
            {
                Console.WriteLine($"WorkerNode {Id} found the correct input: {input}");
                return true;
            }
        }
        return false;
    }
}