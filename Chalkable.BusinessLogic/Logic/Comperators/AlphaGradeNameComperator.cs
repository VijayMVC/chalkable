using System;
using System.Collections.Generic;

namespace Chalkable.BusinessLogic.Logic.Comperators
{
    public class AlphaGradeNameComperator : IComparer<string>
    {
        private static readonly string[] defaultAlphaGrades = new[] { "A+", "A", "A-", "B+", "B", "B-", "C+", "C", "C-", "D+", "D", "D-", "F" };

        public int Compare(string x, string y)
        {
            return CompareAlphaGrade(x, y);
        }

        public static int CompareAlphaGrade(string x, string y)
        {
            if (!string.IsNullOrEmpty(x) && !string.IsNullOrEmpty(y))
            {
                var xIndex = Array.IndexOf(defaultAlphaGrades, x);
                var yIndex = Array.IndexOf(defaultAlphaGrades, y);

                if (xIndex < 0 && yIndex < 0)
                    return String.Compare(x, y, StringComparison.Ordinal);

                if (xIndex >= 0 && xIndex < yIndex || yIndex < 0) return 1;
                if (yIndex >= 0 && xIndex > yIndex || xIndex < 0) return -1;

            }
            return String.Compare(x, y, StringComparison.Ordinal);            
        }
    }
}
