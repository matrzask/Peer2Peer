using System.Security.Cryptography;
using System.Text;
using System.Text.Json.Serialization;

namespace Peer2Peer.Helpers
{
    public enum HashAlgorithmType
    {
        MD5,
        SHA1,
        SHA256,
        SHA512
    }

    public class Hasher
    {
        public string TargetHash { get; private set; }
        public HashAlgorithmType AlgorithmType { get; private set; }


        [JsonConstructor]
        public Hasher(string targetHash, HashAlgorithmType algorithmType)
        {
            TargetHash = targetHash;
            AlgorithmType = algorithmType;
        }

        public bool Compare(string input)
        {
            string inputHash = ComputeHash(input);
            return string.Equals(inputHash, TargetHash, StringComparison.OrdinalIgnoreCase);
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
            return AlgorithmType switch
            {
                HashAlgorithmType.MD5 => MD5.Create(),
                HashAlgorithmType.SHA1 => SHA1.Create(),
                HashAlgorithmType.SHA256 => SHA256.Create(),
                HashAlgorithmType.SHA512 => SHA512.Create(),
                _ => throw new NotSupportedException($"Unsupported hash algorithm: {AlgorithmType}")
            };
        }
    }
}