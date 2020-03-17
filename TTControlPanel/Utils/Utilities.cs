using System;
using System.Security.Cryptography;
using System.Text;

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

        public static string RandomLetterUpper()
        {
            string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            Random rand = new Random();
            int num = rand.Next(0, chars.Length);
            return chars[num].ToString();
        }

        public static string RandomLetterLower()
        {
            string chars = "abcdefghijklmnopqrstuvwxyz";
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

    }
}