﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chalkable.Data.School.Model
{
    public class ScheduleSection
    {
        public Guid Id { get; set; }
        public const string NUMBER_FIELD = "Number";
        public int Number { get; set; }
        public string Name { get; set; }
        public const string MARKING_PERIOD_REF_FIELD = "MarkingPeriodRef";
        public Guid MarkingPeriodRef { get; set; }
        public int? SisId { get; set; }
    }
}
