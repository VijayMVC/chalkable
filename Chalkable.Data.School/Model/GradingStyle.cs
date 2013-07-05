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
}
