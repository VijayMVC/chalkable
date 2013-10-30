﻿using System;

namespace Chalkable.Data.School.Model
{
    public class MarkingPeriodClass
    {
        public const string ID_FIELD = "Id";
        public const string CLASS_REF_FIELD = "ClassRef";
        public const string MARKING_PERIOD_REF_FIELD = "MarkingPeriodRef";
        
        public int Id { get; set; }
        public int ClassRef { get; set; }
        public int MarkingPeriodRef { get; set; }
        public int SchoolRef { get; set; }
    }
}
