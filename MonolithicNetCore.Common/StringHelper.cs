using System;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace MonolithicNetCore.Common
{
    public class StringHelper
    {
        public static string ToUnsignString(string input)
        {
            input = input.Trim();
            for (int i = 0x20; i < 0x30; i++)
            {
                input = input.Replace(((char)i).ToString(), " ");
            }
            input = input.Replace(".", "-");
            input = input.Replace(" ", "-");
            input = input.Replace(",", "-");
            input = input.Replace(";", "-");
            input = input.Replace(":", "-");
            input = input.Replace("  ", "-");
            Regex regex = new Regex(@"\p{IsCombiningDiacriticalMarks}+");
            string str = input.Normalize(NormalizationForm.FormD);
            string str2 = regex.Replace(str, string.Empty).Replace('đ', 'd').Replace('Đ', 'D');
            while (str2.IndexOf("?") >= 0)
            {
                str2 = str2.Remove(str2.IndexOf("?"), 1);
            }
            while (str2.Contains("--"))
            {
                str2 = str2.Replace("--", "-").ToLower();
            }
            return str2.ToLower();
        }

        public static string CreateSaltKey(int size)
        {
            // Generate a cryptographic random number
            var rng = new RNGCryptoServiceProvider();
            var buff = new byte[size];
            rng.GetBytes(buff);

            // Return a Base64 string representation of the random number
            return Convert.ToBase64String(buff);
        }

        public static string GetKeyYoutube(string url)
        {
            string pattern1 = @"\w+:+\/+\/+\w+\.+\w+\/";
            string pattern2 = @"\w+:+\/+\/+\w+\.+\w+\.+\w+\/+\w+\?\w\=";

            Match m1 = Regex.Match(url, pattern1);
            Match m2 = Regex.Match(url, pattern2);
            if (!m1.Value.Equals(""))
                return url.Replace(m1.Value, "");
            if (!m2.Value.Equals(""))
                return url.Replace(m2.Value, "");
            return url;
        }

        public static string NumberFormatPriceVI(dynamic price)
        {
            CultureInfo culture = CultureInfo.GetCultureInfo("vi-VN");
            return double.Parse(price.ToString()).ToString("#,###", culture.NumberFormat);
        }
    }
}
