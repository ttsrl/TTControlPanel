using System;
using System.Drawing;
using System.Globalization;
using System.Linq;
using Microsoft.AspNetCore.Http;

namespace TTControlPanel.Utilities
{
    public static class Extensions
    {

        public static int ToInt(this char c)
        {
            return c - '0';
        }

        public static string ToFirstCharUpper(this string input)
        {
            switch (input)
            {
                case null: return "";
                case "": return "";
                default: return input.First().ToString().ToUpper() + input.Substring(1);
            }
        }

        public static string ToTitleCase(this string input)
        {
            switch (input)
            {
                case null: return "";
                case "": return "";
                default: return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(input.ToLower());
            }
        }

        public static string ToHex(this Color c)
        {
            return $"#{c.R:X2}{c.G:X2}{c.B:X2}";
        }

        public static string ToRgb(this Color c)
        {
            return $"RGB({c.R},{c.G},{c.B})";
        }

        public static void SetBool(this ISession session, string key, bool value)
        {
            var bytes = new[]
            {
                Convert.ToByte(value)
            };
            session.Set(key, bytes);
        }

        public static bool? GetBool(this ISession session, string key)
        {
            var data = session.Get(key);
            if (data == null || data.Length < 1) return null;
            return Convert.ToBoolean(data[0]);
        }
    }
}