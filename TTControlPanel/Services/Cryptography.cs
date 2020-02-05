using Konscious.Security.Cryptography;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace TTControlPanel.Services
{
    public class Cryptography : IDisposable
    {
        private readonly SHA1 _sha1;
        private readonly SHA256 _sha256;
        private readonly SHA512 _sha512;

        public Cryptography()
        {
            _sha1 = SHA1.Create();
            _sha256 = SHA256.Create();
            _sha512 = SHA512.Create();
        }

        public void Dispose()
        {
            _sha1.Dispose();
            _sha256.Dispose();
            _sha512.Dispose();
        }

        public string Sha1Hash(string text)
        {
            var hashedBytes = _sha1.ComputeHash(Encoding.UTF8.GetBytes(text));
            return BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
        }

        public string Sha1Hash(string text, int iterations)
        {
            for (var i = 0; i < iterations; i++) text = Sha1Hash(text);
            return text;
        }

        public string Sha256Hash(string text)
        {
            var hashedBytes = _sha256.ComputeHash(Encoding.UTF8.GetBytes(text));
            return BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
        }

        public string Sha256Hash(string text, int iterations)
        {
            for (var i = 0; i < iterations; i++) text = Sha256Hash(text);
            return text;
        }

        public string Sha512Hash(string text)
        {
            var hashedBytes = _sha512.ComputeHash(Encoding.UTF8.GetBytes(text));
            return BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
        }

        public string Sha512Hash(string text, int iterations)
        {
            for (var i = 0; i < iterations; i++) text = Sha512Hash(text);
            return text;
        }

        public async Task<string> Argon2HashAsync(string text, int iterations = 50, int degreeOfParallelism = 8,
            int memorySize = 8192)
        {
            using (var a = new Argon2i(Encoding.UTF8.GetBytes(text)))
            {
                a.DegreeOfParallelism = degreeOfParallelism;
                a.MemorySize = memorySize;
                a.Iterations = iterations;
                a.Salt = Encoding.UTF8.GetBytes(Strongify(text));
                a.AssociatedData = Encoding.UTF8.GetBytes(Strongify(text));
                var b = await a.GetBytesAsync(512 / 8);
                return BitConverter.ToString(b).Replace("-", "").ToLower();
            }
        }

        public string Argon2Hash(string text, int iterations = 50, int degreeOfParallelism = 8, int memorySize = 8192)
        {
            using (var a = new Argon2i(Encoding.UTF8.GetBytes(text)))
            {
                a.DegreeOfParallelism = degreeOfParallelism;
                a.MemorySize = memorySize;
                a.Iterations = iterations;
                a.Salt = Encoding.UTF8.GetBytes(Strongify(text));
                a.AssociatedData = Encoding.UTF8.GetBytes(Strongify(text));
                var b = a.GetBytes(512 / 8);
                return BitConverter.ToString(b).Replace("-", "").ToLower();
            }
        }

        private static string Strongify(string text)
        {
            return $"<!--123456789qwerty {text}{new string(text.ToCharArray().Reverse().ToArray())} ytrewq987654321-->";
        }
    }
}
