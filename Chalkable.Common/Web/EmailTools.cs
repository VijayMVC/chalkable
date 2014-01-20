using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Chalkable.Common.Web
{
    public class EmailTools
    {
        public static bool IsValidEmailAddress(string inputEmail)
        {
            if (string.IsNullOrEmpty(inputEmail))
                return false;
            string strRegex = @"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}" +
                  @"\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\" +
                  @".)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$";
            var re = new Regex(strRegex);
            if (re.IsMatch(inputEmail))
                return (true);
            return (false);
        }
    }
}
