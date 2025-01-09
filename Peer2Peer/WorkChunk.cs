class WorkChunk
{
    public string Start { get; set; }
    public string End { get; set; }

    public WorkChunk(string start, string end)
    {
        Start = start;
        End = end;
    }

    public IEnumerable<string> GeneratePasswords()
    {
        throw new NotImplementedException();
    }
}