using System.Security.Cryptography;
using System.Text;

namespace Peer2Peer.Helpers
{
    public enum HashAlgorithmType
    {
        MD5,
        SHA1,
        SHA256,
        SHA512
    }

    class Hasher
    {
        private readonly string targetHash;
        private HashAlgorithmType algorithmType;

        public Hasher(string targetHash, HashAlgorithmType algorithmType)
        {
            this.targetHash = targetHash;
            this.algorithmType = algorithmType;
        }

        public bool Compare(string input)
        {
            string inputHash = ComputeHash(input);
            return string.Equals(inputHash, targetHash, StringComparison.OrdinalIgnoreCase);
        }

        public string ComputeHash(string input)
        {
            if (string.IsNullOrEmpty(input))
                throw new ArgumentException("Input cannot be null or empty.");

            using (var hasher = CreateHashAlgorithm())
            {
                byte[] inputBytes = Encoding.UTF8.GetBytes(input);
                byte[] hashBytes = hasher.ComputeHash(inputBytes);
                return BitConverter.ToString(hashBytes).Replace("-", "").ToLowerInvariant();
            }
        }

        private HashAlgorithm CreateHashAlgorithm()
        {
            return algorithmType switch
            {
                HashAlgorithmType.MD5 => MD5.Create(),
                HashAlgorithmType.SHA1 => SHA1.Create(),
                HashAlgorithmType.SHA256 => SHA256.Create(),
                HashAlgorithmType.SHA512 => SHA512.Create(),
                _ => throw new NotSupportedException($"Unsupported hash algorithm: {algorithmType}")
            };
        }
    }
}