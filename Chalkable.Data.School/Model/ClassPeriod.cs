using System;
using Chalkable.Data.Common;

namespace Chalkable.Data.School.Model
{
    public class ClassPeriod
    {
        public const string ID_FIELD = "Id";
        public const string PERIOD_REF_FIELD = "PeriodRef";
        public const string DATE_TYPE_REF_FIELD = "DateTypeRef";
        public const string ROOM_REF_FIELD = "RoomRef";
        public const string CLASS_REF_FIELD = "ClassRef";

        public int Id { get; set; }
        public int PeriodRef { get; set; }
        public int ClassRef { get; set; }
        public int? RoomRef { get; set; }
        public int DateTypeRef { get; set; }

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

    public class ClassPeriodDetails : ClassPeriod
    {
        [DataEntityAttr]
        public Room Room { get; set; }
    }
}
