class WorkChunk
{
    public string Start { get; set; }
    public string End { get; set; }
    public char[] CharacterSet { get; set; }

    public WorkChunk(string start, string end, char[] characterSet)
    {
        Start = start;
        End = end;
        CharacterSet = characterSet;
    }

    private long ToNumber(string password)
    {
        long number = 0;
        int baseSize = CharacterSet.Length;

        foreach (char c in password)
        {
            number = number * baseSize + Array.IndexOf(CharacterSet, c);
        }

        return number;
    }

    private string ToString(long number, int length)
    {
        char[] result = new char[length];
        int baseSize = CharacterSet.Length;

        for (int i = length - 1; i >= 0; i--)
        {
            result[i] = CharacterSet[number % baseSize];
            number /= baseSize;
        }

        return new string(result);
    }

    //example: start = "aa", end = "ba", characterSet = "abc".ToCharArray(), result: "aa", "ab", "ac", "ba"
    public IEnumerable<string> GeneratePasswords()
    {
        long start = ToNumber(Start);
        long end = ToNumber(End);

        for (long i = start; i <= end; i++)
        {
            yield return ToString(i, Start.Length);
        }
    }
}