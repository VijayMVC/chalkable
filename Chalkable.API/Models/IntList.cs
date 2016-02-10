using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Chalkable.API.Models
{
    public class IntList : List<int>
    {
        private const char FIRST_LEVEL = ',';
        public IntList() { }

        public IntList(string stringValues)
        {
            if(!string.IsNullOrEmpty(stringValues))
                AddRange(stringValues.Split(new[] { FIRST_LEVEL }).Select(x => int.Parse(x)).ToList());
        }

        public override string ToString()
        {
            var res = new StringBuilder();
            if (Count > 0)
            {
                res.Append(this[0]);
                for (int i = 1; i < Count; i++)
                    res.Append(",").Append(this[i]);
            }
            return res.ToString();
        }
    }

}