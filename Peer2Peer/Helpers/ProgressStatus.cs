namespace Peer2Peer.Helpers
{
    public static class ProgressStatus
    {
        //e.g. TotalChunks[0] gives total number of chunks for passwords of length 1
        public static List<int> TotalChunks { get; set; } = new List<int>();
        public static List<int> CompletedChunks { get; set; } = new List<int>();
        public static string? FoundPassword { get; set; }
    }
}