using System;
using System.Collections.Generic;

namespace Chalkable.BusinessLogic.Logic.Comperators
{
    public class PracticeGradeScoreComperator : IComparer<string>
    {
        public int Compare(string x, string y)
        {
            return ComparePracticeScore(x, y);
        }

        public static int ComparePracticeScore(string x, string y)
        {
            bool a = string.IsNullOrEmpty(x), b = string.IsNullOrEmpty(y);
            if (a && b) return 0;
            if (a) return -1;
            if (b) return 1;

            x = x.Replace("%", "");
            y = y.Replace("%", "");

            double first, second;
            var xParced = double.TryParse(x, out first);
            var yParced = double.TryParse(y, out second);
            if (xParced && yParced) return first.CompareTo(second);
            if (xParced) return 1;
            if (yParced) return -1;

            return String.Compare(x, y, StringComparison.Ordinal);            
        }
    }
}
