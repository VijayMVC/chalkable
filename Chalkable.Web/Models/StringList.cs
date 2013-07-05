using System.Collections.Generic;
using System.Linq;

namespace Chalkable.Web.Models
{
    public class StringList : List<string>
    {
        private const char FIRST_LEVEL = ',';
        public StringList() { }

        public StringList(string stringValues)
        {
            AddRange(stringValues.Split(new[] { FIRST_LEVEL }).ToList());
        }
    }
}