using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chalkable.Common
{
    public class ChalkableList<T> : List<T>
    {
        private const char FIRST_LEVEL = ',';
        protected void ConvertToList(string stringValues, Func<string, T> parseAction)
        {
            AddRange(stringValues.Split(new[] { FIRST_LEVEL }).Select(parseAction).ToList());
        }
    }
    public class IntList : ChalkableList<int>
    {
        public IntList() { }
        public IntList(string stringValues)
        {
            ConvertToList(stringValues, int.Parse);
        }
    }
    public class DoubleList : ChalkableList<double>
    {
        public DoubleList() { }
        public DoubleList(string stringValues)
        {
            ConvertToList(stringValues, x => double.Parse(x, CultureInfo.InvariantCulture));
        }
    }
    public class GuidList : ChalkableList<Guid>
    {
        public GuidList() { }
        public GuidList(string stringValues)
        {
            ConvertToList(stringValues, Guid.Parse);
        }
    }
    public class StringList : ChalkableList<string>
    {
        public StringList() { }
        public StringList(string stringValues)
        {
            ConvertToList(stringValues, x => x);
        }
    }
}
