// ********************************************
// TwitEclipseAPI - .Net Twitter API Wrapper  *
// Developed by Ryan Alford                   *
// ********************************************

// TwitEclipseAPI.cs
// This class contains some commonly used functions.


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Globalization;

namespace TwitterAPI
{
    public enum MonthEnum
    {
        Jan = 1,
        Feb = 2,
        Mar = 3,
        Apr = 4,
        May = 5, 
        Jun = 6,
        Jul = 7,
        Aug = 8,
        Sep = 9, 
        Oct = 10, 
        Nov = 11, 
        Dec = 12
    }

    public class Common
    {
        /// <summary>
        /// Parses the DateTime from Twitter into a usable DateTime object
        /// </summary>
        /// <param name="date">Date from Twitter</param>
        /// <returns></returns>
        public static DateTime ParseDateTime(string date)
        {
            // Twitter's dates are formatted as:
            //       Tue Apr 07 22:52:51 +0000 2009
            string dayOfWeek = date.Substring(0, 3).Trim();
            string month = date.Substring(4, 3).Trim();
            string dayInMonth = date.Substring(8, 2).Trim();

            int hours = int.Parse(date.Substring(11, 2));
            int minutes = int.Parse(date.Substring(14, 2));
            int seconds = int.Parse(date.Substring(17, 2));

            string time = date.Substring(11, 9).Trim();
            string offset = date.Substring(20, 5).Trim();
            string year = date.Substring(25, 5).Trim();
            string dateTime = string.Format("{0}-{1}-{2} {3}", dayInMonth, month, year, time);
            DateTime ret = new DateTime(int.Parse(year), Convert.ToInt32(Enum.Parse(typeof(MonthEnum), month)), int.Parse(dayInMonth), hours, minutes, seconds, DateTimeKind.Utc);
            //DateTime ret = DateTime.Parse(dateTime);
            //ret.
            return ret.ToLocalTime();
        }

        private static byte[] UrlEncodeBytesToBytesInternal(byte[] bytes, int offset, int count, bool alwaysCreateReturnValue)
        {
            int num = 0;
            int num2 = 0;
            for (int i = 0; i < count; i++)
            {
                char ch = (char)bytes[offset + i];
                if (ch == ' ')
                {
                    num++;
                }
                else if (!IsSafe(ch))
                {
                    num2++;
                }
            }
            if ((!alwaysCreateReturnValue && (num == 0)) && (num2 == 0))
            {
                return bytes;
            }
            byte[] buffer = new byte[count + (num2 * 2)];
            int num4 = 0;
            for (int j = 0; j < count; j++)
            {
                byte num6 = bytes[offset + j];
                char ch2 = (char)num6;
                if (IsSafe(ch2))
                {
                    buffer[num4++] = num6;
                }
                else if (ch2 == ' ')
                {
                    buffer[num4++] = 0x2b;
                }
                else
                {
                    buffer[num4++] = 0x25;
                    buffer[num4++] = (byte)IntToHex((num6 >> 4) & 15);
                    buffer[num4++] = (byte)IntToHex(num6 & 15);
                }
            }
            return buffer;
        }

        internal static bool IsSafe(char ch)
        {
            if ((((ch >= 'a') && (ch <= 'z')) || ((ch >= 'A') && (ch <= 'Z'))) || ((ch >= '0') && (ch <= '9')))
            {
                return true;
            }
            switch (ch)
            {
                case '\'':
                case '(':
                case ')':
                case '*':
                case '-':
                case '.':
                case '_':
                case '!':
                    return true;
            }
            return false;
        }

        internal static char IntToHex(int n)
        {
            if (n <= 9)
            {
                return (char)(n + 0x30);
            }
            return (char)((n - 10) + 0x61);
        }

        public static string UrlEncode(string data)
        {
            return Encoding.ASCII.GetString(UrlEncodeToBytes(data, Encoding.UTF8));
        }

        public static byte[] UrlEncodeToBytes(string str, Encoding e)
        {
            if (str == null)
            {
                return null;
            }
            byte[] bytes = e.GetBytes(str);
            return UrlEncodeBytesToBytesInternal(bytes, 0, bytes.Length, false);
        }

        private static String UrlDecode(String input)
        {
            // There are much more quicker ways to remplace any % character
            // followed by two digits with the ASCII character those two hex
            // digits represents, but this was rather quick to write). Feel
            // free to replace this with a quicker implementation.
            Regex regex = new Regex("%([0-9a-z][0-9a-z])", RegexOptions.IgnoreCase);
            input = input.Replace('+', ' ');
            return regex.Replace(input, new MatchEvaluator(delegate(Match m) { return (char)UInt32.Parse(m.Groups[1].Value, NumberStyles.HexNumber) + ""; }));
        }

        // Parse a HTTP Query String, returning a dictionary of field name/value pairs
        public static Dictionary<String, String> ParseQueryString(string queryString)
        {
            // Remove the ? character at the start of the string
            if (queryString[0] == '?')
                queryString = queryString.Remove(0, 1);

            // Split out the key/value pairs and place them
            // in a Dictionary.
            Dictionary<String, String> parameters = new Dictionary<string, string>();
            foreach (string keyvalue in queryString.Split('&'))
            {
                // Some keys may not have values
                string[] temp = keyvalue.Split('=');
                if (temp.Length > 1)
                    parameters.Add(UrlDecode(temp[0]), UrlDecode(temp[1]));
                else
                    parameters.Add(UrlDecode(temp[0]), null);
            }

            return parameters;
        }
    }
}
