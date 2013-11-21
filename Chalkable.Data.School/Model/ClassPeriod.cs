﻿using Chalkable.Data.Common;

namespace Chalkable.Data.School.Model
{
    public class ClassPeriod
    {
        public const string PERIOD_REF_FIELD = "PeriodRef";
        public const string DAY_TYPE_REF_FIELD = "DayTypeRef";
        public const string ROOM_REF_FIELD = "RoomRef";
        public const string CLASS_REF_FIELD = "ClassRef";

        [PrimaryKeyFieldAttr]
        public int PeriodRef { get; set; }
        [PrimaryKeyFieldAttr]
        public int ClassRef { get; set; }
        public int? RoomRef { get; set; }
        [PrimaryKeyFieldAttr]
        public int DayTypeRef { get; set; }
        public int SchoolRef { get; set; }

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
