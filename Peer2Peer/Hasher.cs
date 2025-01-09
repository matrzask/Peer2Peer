class Hasher
{
    private string targetHash;

    public Hasher(string targetHash)
    {
        this.targetHash = targetHash;
    }

    public bool Compare(string password)
    {
        return ComputeHash(password) == targetHash;
    }

    public static string ComputeHash(string password)
    {
        return password;
    }
}