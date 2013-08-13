using System;
using Chalkable.Data.Common;

namespace Chalkable.Data.School.Model
{
    public class ClassPeriod
    {
        public const string ID_FIELD = "Id";
        public Guid Id { get; set; }
        public const string PERIOD_REF_FIELD = "PeriodRef";
        public Guid PeriodRef { get; set; }
        public Guid ClassRef { get; set; }
        public Guid RoomRef { get; set; }
        public int? SisId { get; set; }

        private Period period;
        [DataEntityAttr]
        public Period Period
        {
            get { return period; }
            set
            {
                period = value;
                if (value != null && value.Id != Guid.Empty)
                    PeriodRef = value.Id;
            }
        }
    }

    public class ClassPeriodDetails : ClassPeriod
    {
        [DataEntityAttr]
        public Room Room { get; set; }
    }
}
