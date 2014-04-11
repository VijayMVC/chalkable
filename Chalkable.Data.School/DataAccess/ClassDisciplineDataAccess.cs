using System;
using System.Collections.Generic;
using System.Linq;
using Chalkable.Data.Common;
using Chalkable.Data.Common.Orm;
using Chalkable.Data.School.Model;

namespace Chalkable.Data.School.DataAccess
{
    public class ClassDisciplineQuery
    {
        public int? PersonId { get; set; }
        public int? MarkingPeriodId { get; set; }
        public int? ClassId { get; set; }
        public int? TeacherId { get; set; }
        public int? Type { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public int? StartTime { get; set; }
        public int? EndTime { get; set; }
        public int? SchoolYearId { get; set; }
        public int? Id { get; set; }
        public bool NeedAllData { get; set; }
    }
}
