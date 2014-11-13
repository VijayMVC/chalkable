using Chalkable.Data.Common;

namespace Chalkable.Data.School.Model
{
    public class ClassPeriod
    {
        public const string PERIOD_REF_FIELD = "PeriodRef";
        public const string DAY_TYPE_REF_FIELD = "DayTypeRef";
        public const string CLASS_REF_FIELD = "ClassRef";

        [PrimaryKeyFieldAttr]
        public int PeriodRef { get; set; }
        [PrimaryKeyFieldAttr]
        public int DayTypeRef { get; set; }
        [PrimaryKeyFieldAttr]
        public int ClassRef { get; set; }

        private Period period;
        [DataEntityAttr]
        public Period Period
        {
            get { return period; }
            set
            {
                period = value;
                if (value != null && value.Id != 0)
                    PeriodRef = value.Id;
            }
        }
    }
}
