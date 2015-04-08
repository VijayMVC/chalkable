using System;
using System.Collections.Generic;


namespace GCObjectRenderer
{
    public static class ParametersHelper
    {
        public static List<string> GetParametersList(string parameters)
        {
            List<string> list = new List<string>();
            string s = "";
            int braces = 0;
            for (int j = 0; j < parameters.Length; j++)
            {
                if (parameters[j] == '(')
                    braces++;
                if (parameters[j] == ')')
                    braces--;
                if (parameters[j] == ',' && braces == 0)
                {
                    if (!string.IsNullOrEmpty(s.Trim()))
                        list.Add(s.Trim());
                    s = "";
                }
                else
                {
                    s += parameters[j];
                }
            }
            if (braces != 0)
                throw new Exception("Incorect syntax in parameters string [" + parameters + "]");

            if (!string.IsNullOrEmpty(s.Trim()))
                list.Add(s.Trim());
            return list;
        }
    }
}
