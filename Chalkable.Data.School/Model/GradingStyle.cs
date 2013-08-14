using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chalkable.Data.School.Model
{
    public class GradingStyle
    {
        public Guid Id { get; set; }
        public GradingStyleEnum GradingStyleValue { get; set; }
        public int MaxValue { get; set; }
        public int StyledValue { get; set; }
    }

    public enum GradingStyleEnum
    {
        Numeric100 = 0,
        Abcf = 1,
        Complete = 2,
        Check = 3
    }


    public enum AbcfGradingStyle
    {
        AP = 12,
        A = 11,
        AM = 10,
        BP = 9,
        B = 8,
        BM = 7,
        CP = 6,
        C = 5,
        CM = 4,
        DP = 3,
        D = 2,
        DM = 1,
        F = 0
    }

    public enum CompleteIncompleteGradingStyle
    {
        Incomplete = 0,
        Complete = 1
    }

    public enum CheckGradingStyle
    {
        CheckMinus = 0,
        Check = 1,
        CheckPlus = 2
    }
}
