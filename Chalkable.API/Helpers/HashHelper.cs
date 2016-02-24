using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Chalkable.API.Helpers
{
    public static class HashHelper
    {
        public static IEnumerable<byte> Hmac(string s)
        {
            return new HMACSHA256().ComputeHash(Encoding.UTF8.GetBytes(s));
        }
        public static string Hex(IEnumerable<byte> bytes)
        {
            return bytes
                .Select(b => b.ToString("x2"))
                .Aggregate(new StringBuilder(), (sb, hex) => sb.Append(hex))
                .ToString()
                .ToLower();
        }
    }
}
