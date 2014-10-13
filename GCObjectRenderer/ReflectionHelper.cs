using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace GCObjectRenderer
{
    public static class ReflectionHelper
    {
        public static object ReadObject(string template, ref int i, object model)
        {
            if (i >= template.Length)
                return null;
            string name = "";
            object current;
            if (template[i] == '^')
            {
                current = model;
                i++;
            }
            else
            {
                while (i < template.Length && char.IsLetterOrDigit(template[i]))
                {
                    name += template[i];
                    i++;
                }
                if (name.Length == 0)
                    throw new Exception("Incorect syntax in [" + template + "] at position " + i);

                Type t = model.GetType();


                bool hasParams = i < template.Length && template[i] == '(';
                if (hasParams)
                {
                    string parameters = BraceHelper.SelectBracesContent(template, ref i, '(', ')');
                    List<string> list = ParametersHelper.GetParametersList(parameters);
                    object[] ps = new object[list.Count];
                    int k = 0;
                    foreach (string l in list)
                    {
                        if (l == "^")
                            ps[k] = model;
                        else if (l.StartsWith("^"))
                        {
                            int index = 0;
                            ps[k] = ReadObject(l, ref index, model);
                        }
                        else
                            ps[k] = l;
                        k++;
                    }
                    Type[] types = new Type[ps.Length];
                    for (int j = 0; j < ps.Length; j++)
                        types[j] = ps[j].GetType();
                    MethodInfo mi = t.GetMethod(name, types);
                    if (mi != null)
                        current = mi.Invoke(model, ps);
                    else
                    {
                        throw new Exception("Can not find method:[" + name + "] in object type [" + t + "]");
                    }
                }
                else
                {
                    FieldInfo fi = t.GetField(name);
                    if (fi != null)
                        current = fi.GetValue(model);
                    else
                    {
                        PropertyInfo pi = t.GetProperty(name,
                                                        BindingFlags.Instance | BindingFlags.Public |
                                                        BindingFlags.GetProperty);
                        if (pi != null)
                            current = pi.GetValue(model, null);
                        else
                        {
                            throw new Exception("Can not find property or field:[" + name + "] in object type [" + t + "]");
                        }
                    }
                }    
            }
            if (i >= template.Length || template[i] != '.')
                return current;
            i++;
            return ReadObject(template, ref i, current);
        }
    }
}
