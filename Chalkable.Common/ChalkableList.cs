using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Chalkable.Common
{
    public class ChalkableList<T> : List<T>
    {
        private const char FIRST_LEVEL = ',';

        protected List<T> ConvertToList(string stringValues, Func<string, T> parseAction)
        {
            return stringValues.Split(new[] {FIRST_LEVEL}).Select(parseAction).ToList();
        }
        protected void ConvertAndAddToList(string stringValues, Func<string, T> parseAction)
        {
            AddRange(ConvertToList(stringValues, parseAction));
        }
    }
    public class IntList : ChalkableList<int>
    {
        public IntList() { }
        public IntList(string stringValues)
        {
            ConvertAndAddToList(stringValues, int.Parse);
        }
    }
    public class DoubleList : ChalkableList<double>
    {
        public DoubleList() { }
        public DoubleList(string stringValues)
        {
            ConvertAndAddToList(stringValues, x => double.Parse(x, CultureInfo.InvariantCulture));
        }
    }
    public class GuidList : ChalkableList<Guid>
    {
        public GuidList() { }
        public GuidList(string stringValues)
        {
            ConvertAndAddToList(stringValues, Guid.Parse);
        }
    }
    public class StringList : ChalkableList<string>
    {
        public StringList() { }
        public StringList(string stringValues)
        {
            ConvertAndAddToList(stringValues, x => x);
        }
    }

    public class ListOfStringList : List<List<string>>
    {
        private const char FirstLevel = ',';
        private const char SecondLevel = '|';

        public override string ToString()
        {
            string res = "";
            bool isFirst = true;
            foreach (var set in this)
            {
                if (isFirst)
                    isFirst = false;
                else
                    res += FirstLevel;
                res += set;
            }
            return res;
        }

        public ListOfStringList() { }

        public ListOfStringList(string stringValues)
        {
            var stringList = new StringList(stringValues);
            AddRange(stringList.Select(s => s.Split(SecondLevel).ToList()));
        }
    }

}
