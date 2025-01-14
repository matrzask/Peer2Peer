class CoordinatorNode : Node
{
    private readonly int _chunkSize;
    private readonly int _maxPasswordLength;
    private int _curPasswordLength;
    private readonly char[] _charset;
    private readonly Queue<WorkChunk> _workChunks = new Queue<WorkChunk>();

    public CoordinatorNode(int chunkSize, char[] charset, int minPasswordLength = 1, int maxPasswordLength = 20)
    {
        _chunkSize = chunkSize;
        _charset = charset;
        _curPasswordLength = minPasswordLength;
        _maxPasswordLength = maxPasswordLength;
    }

    public void AssignChunk(WorkerNode node, WorkChunk chunk)
    {
        node.StartWork(chunk);
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
}