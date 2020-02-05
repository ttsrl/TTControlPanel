using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using TTControlPanel.Models;

namespace TTControlPanel.Utilities
{
    public static class Utilities
    {

        public static int RandomNumber(int min, int max)
        {
            Random random = new Random();
            return random.Next(min, max + 1);
        }

        public static int RandomCifre()
        {
            string chars = "0123456789";
            Random rand = new Random();
            int num = rand.Next(0, chars.Length);
            var c = chars[num].ToString();
            int v = int.Parse(c);
            return v;
        }

        public static string RandomLetter()
        {
            string chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
            Random rand = new Random();
            int num = rand.Next(0, chars.Length);
            return chars[num].ToString();
        }

        public static int VersionToInt(Version v)
        {
            try
            {
                return Convert.ToInt32((v.Major * 10).ToString() + v.Minor.ToString());
            }
            catch { return 0; }
        }

        public static string Sha256Hash(string text)
        {
            SHA256 _sha256 = SHA256.Create();
            var hashedBytes = _sha256.ComputeHash(Encoding.UTF8.GetBytes(text));
            return BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
        }

        public static string Sha256Hash(string text, int iterations)
        {
            for (var i = 0; i < iterations; i++) text = Sha256Hash(text);
            return text;
        }

    }
}
