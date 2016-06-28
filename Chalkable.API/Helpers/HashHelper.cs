using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Chalkable.API.Helpers
{
    public static class HashHelper
    {
        public static IEnumerable<byte> ComputeHash(string s)
        {
            return SHA256.Create().ComputeHash(Encoding.UTF8.GetBytes(s));

        }
        public static string Hex(IEnumerable<byte> bytes)
        {
            return bytes
                .Select(b => b.ToString("x2"))
                .Aggregate(new StringBuilder(), (sb, hex) => sb.Append(hex))
                .ToString()
                .ToLower();
        }

        public static string HexOfCumputedHash(string s)
        {
            return Hex(ComputeHash(s));
        }
    }
}
