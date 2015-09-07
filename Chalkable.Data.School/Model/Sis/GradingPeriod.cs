﻿using System;
using Chalkable.Data.Common;

namespace Chalkable.Data.School.Model.Sis
{
    public class GradingPeriod
    {
        public const string ID_FIELD = "Id";
        public const string MARKING_PERIOD_REF_FIELD = "MarkingPeriodRef";
        public const string SCHOOL_YEAR_REF_FIELD = "SchoolYearRef";
        public const string START_DATE_FIELD = "StartDate";
        public const string END_DATE_FIELD = "EndDate";

        [PrimaryKeyFieldAttr]
        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }        
        public int MarkingPeriodRef { get; set; }
        public int SchoolYearRef { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime EndTime { get; set; }
        public string SchoolAnnouncement { get; set; }
        public bool AllowGradePosting { get; set; } 
    }
}
