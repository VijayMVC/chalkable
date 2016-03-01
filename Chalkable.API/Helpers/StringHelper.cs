using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Chalkable.API.Helpers
{
    public static class StringHelper
    {
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

        public static string JoinString(this IEnumerable<int> s, string separator = " ")
        {
            return s?.Select(x => x.ToString()).JoinString(separator);
        }

    }
}
