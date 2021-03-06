﻿using System.Collections.Generic;
using Chalkable.Data.Common;

namespace Chalkable.Data.School.Model
{
    public class AttendanceReason
    {
        public const string ID_FIELD = "Id";
        [PrimaryKeyFieldAttr]
        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }
        public bool IsSystem { get; set; }
        public bool IsActive { get; set; }

        [NotDbFieldAttr]
        public bool IsExcused => Category == "E";

        [NotDbFieldAttr]
        public IList<AttendanceLevelReason> AttendanceLevelReasons { get; set; } 
    }

    public class AttendanceLevelReason
    {
        public const string ATTENDACNE_REASON_REF_FIELD = "AttendanceReasonRef";
        public const string ID_FIELD = "Id";

        [PrimaryKeyFieldAttr]
        public int Id { get; set; }
        public string Level { get; set; }
        public int AttendanceReasonRef { get; set; }
        public bool IsDefault { get; set; }

        [NotDbFieldAttr]
        public AttendanceReason AttendanceReason { get; set; }
    }
}
