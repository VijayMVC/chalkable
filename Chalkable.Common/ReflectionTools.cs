using System;
using System.Collections;
using System.Reflection;

namespace Chalkable.Common
{
    public static class ReflectionTools
    {
        public class DeclarationOrderComparator : IComparer
        {
            int IComparer.Compare(Object x, Object y)
            {
                PropertyInfo first = x as PropertyInfo;
                PropertyInfo second = y as PropertyInfo;
                if (first.MetadataToken < second.MetadataToken)
                    return -1;
                else if (first.MetadataToken > second.MetadataToken)
                    return 1;

                return 0;
            }
        }

        public static PropertyInfo[] GetPropertiesInDeclarationOrder(this Type t, bool declaredOnly)
        {
            var flag = BindingFlags.Instance | BindingFlags.Public;
            if (declaredOnly)
                flag = flag | BindingFlags.DeclaredOnly;
            var props = t.GetProperties(flag);
            Array.Sort(props, new DeclarationOrderComparator());
            return props;
        }
    }
}