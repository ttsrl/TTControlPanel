﻿using System;
using System.Collections;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Http;

namespace TTControlPanel.Utilities
{
    public static class Extensions
    {
        public static DateTime ToDateTimeCE(this DateTime d)
        {
            return TimeZoneInfo.ConvertTime(new DateTime(d.Year, d.Month, d.Day, d.Hour, d.Minute, d.Second, DateTimeKind.Utc), TimeZoneInfo.FindSystemTimeZoneById("Central European Standard Time"));
        }

        public static DateTime TruncateMillis(this DateTime date)
        {
            return new DateTime(date.Year, date.Month, date.Day, date.Hour, date.Minute, date.Second, date.Kind);
        }

        public static DateTime TruncateSeconds(this DateTime date)
        {
            return new DateTime(date.Year, date.Month, date.Day, date.Hour, date.Minute, 0, date.Kind);
        }

        public static string ReplaceBINDigits(this string key)
        {
            return Regex.Replace(key, @"\d", "1");
        }

        public static string ReplaceBINAlpha(this string key)
        {
            return Regex.Replace(key, @"[a-zA-Z]", "0");
        }

        public static bool[] ToBoolArray(this int value)
        {
            BitArray b = new BitArray(new int[] { value });
            bool[] bits = new bool[b.Count];
            b.CopyTo(bits, 0);
            return bits;
        }

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

        public static DateTime FromUnixTime(this long unixTime)
        {
            var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return epoch.AddSeconds(unixTime);
        }

        public static long ToUnixTime(this DateTime date)
        {
            var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return Convert.ToInt64((date - epoch).TotalSeconds);
        }

        //public static DateTime ToDateTime(this DateTimeCE dtce)
        //{
        //    return new DateTime(dtce.Year, dtce.Month, dtce.Day, dtce.Hour, dtce.Minute, dtce.Second, DateTimeKind.Unspecified);
        //}

        //public static DateTimeCE FromUnixTime(this long unixTime)
        //{
        //    var epoch = new DateTimeCE(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        //    return epoch.AddSeconds(unixTime);
        //}

        //public static long ToUnixTime(this DateTimeCE date)
        //{
        //    var epoch = new DateTimeCE(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        //    return Convert.ToInt64((date - epoch).TotalSeconds);
        //}
    }
}