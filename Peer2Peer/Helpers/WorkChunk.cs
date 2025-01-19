using Peer2Peer.Password;

namespace Peer2Peer.Helpers
{
    public class WorkChunk
    {
        public long Start { get; set; }
        public long End { get; set; }
        public int Length { get; set; }
        public char[] CharacterSet { get; set; }

        // Parameterless constructor for deserialization
        public WorkChunk() { }

        public WorkChunk(string start, string end, char[] characterSet)
        {
            if (start.Length != end.Length)
                throw new ArgumentException("Start and end must have the same length.");
            Length = start.Length;
            CharacterSet = characterSet;
            Start = ToNumber(start);
            End = ToNumber(end);
        }

        public WorkChunk(long start, long end, char[] characterSet, int passwordLength)
        {
            Start = start;
            End = end;
            Length = passwordLength;
            CharacterSet = characterSet;
        }

        public WorkChunk(string ChunkString, char[] characterSet)
        {
            string[] parts = ChunkString.Split(':');
            Start = long.Parse(parts[0]);
            End = long.Parse(parts[1]);
            Length = int.Parse(parts[2]);
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
        public PasswordCollection GeneratePasswords()
        {
            long start = Start;
            long end = End;

            var collection = new PasswordCollection();

            for (long i = start; i <= end; i++)
            {
                collection.AddItem(ToString(i, Length));
            }

            return collection;
        }

        public string Hash()
        {
            return $"{Start}:{End}:{Length}";
        }
    }
}