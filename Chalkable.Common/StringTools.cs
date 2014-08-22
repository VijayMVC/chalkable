using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Chalkable.Common
{
    public static class StringTools
    {
        public static int LevenshteinDistance(string s, string t)
        {
            int n = s.Length;
            int m = t.Length;
            int[,] d = new int[n + 1, m + 1];

            // Step 1
            if (n == 0)
            {
                return m;
            }

            if (m == 0)
            {
                return n;
            }

            // Step 2
            for (int i = 0; i <= n; d[i, 0] = i++)
            {
            }

            for (int j = 0; j <= m; d[0, j] = j++)
            {
            }

            // Step 3
            for (int i = 1; i <= n; i++)
            {
                //Step 4
                for (int j = 1; j <= m; j++)
                {
                    // Step 5
                    int cost = (t[j - 1] == s[i - 1]) ? 0 : 1;

                    // Step 6
                    d[i, j] = Math.Min(
                        Math.Min(d[i - 1, j] + 1, d[i, j - 1] + 1),
                        d[i - 1, j - 1] + cost);
                }
            }
            // Step 7
            return d[n, m];
        }

        public static string JoinString(this IEnumerable<int> s, string separator = " ")
        {
            if (s == null)
                return null;
            return s.Select(x => x.ToString()).JoinString(separator);
        }

        public static string JoinString(this IEnumerable<Guid> s, string separator = " ")
        {
            if (s == null)
                return null;
            return s.Select(x => x.ToString()).JoinString(separator);
        }

        public static string JoinString(this IEnumerable<string> s, string separator = " ")
        {
            var res = new StringBuilder();
            bool f = true;
            foreach (var str in s)
            {
                if (f)
                    f = false;
                else
                    res.Append(separator);
                res.Append(str);
            }
            return res.ToString();
        }

        public static string BuildShortText(string description, int shortLength)
        {
            if (string.IsNullOrEmpty(description))  return description;

            if (description.Length <= shortLength) return description;

            var j = shortLength;
            while (!Equals(description[j], ' '))
            {
                if (j == 0)
                {
                    break;
                }
                j--;
            }
            var shortDescription = "";
            if (j == 0) j = shortLength;
            for (var i = 0; i < j; i++)
                shortDescription += description[i];
           
            return shortDescription[shortDescription.Length - 1] == ' ' ? shortDescription : shortDescription + "...";

        }
        public static string NumberToStr(int number)
        {
            if (number != 11 && number != 12)
            {
                if (number % 10 == 1)
                    return "" + number + "st";
                if (number % 10 == 2)
                    return "" + number + "nd";
                if (number % 10 == 3)
                    return "" + number + "rd";
            }
            return "" + number + "th";
        }


        public static string CapitalizeFirstLetter(this string s)
        {
            if (string.IsNullOrEmpty(s) || string.IsNullOrEmpty(s.Trim())) return s;
            if (s.Length == 1) return s.ToUpper();

            s = s.ToLower();
            var charts = s.ToCharArray();
            var index = s.IndexOf(charts.First(char.IsLetter));
            charts[index] = char.ToUpper(charts[index]);
            return new string(charts);
        }
    }
}
