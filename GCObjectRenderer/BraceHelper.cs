using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GCObjectRenderer
{
    public static class BraceHelper
    {
        public static string SelectBracesContent(string template, ref int i, char open, char close)
        {
            if (i >= template.Length)
                return null;
            StringBuilder res = new StringBuilder();
            if (template[i] != open)
                return null;
            int braces = 1;
            i++;
            while (i < template.Length && braces > 0)
            {
                if (template[i] == open)
                    braces++;
                if (template[i] == close)
                    braces--;
                if (braces > 0)
                    res.Append(template[i]);
                i++;
            }
            if (braces != 0)
                throw new Exception("Incorect syntax in [" + template + "] at position " + i);
            return res.ToString();
        }
    }
}
