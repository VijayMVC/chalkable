using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chalkable.Common;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Model
{
    public class SchoolSummaryInfo
    {
        public ShortSchoolSummary SchoolDetails { get; set; }
        public int? AttendanceCount { get; set; }
        public int? DisciplinCount { get; set; }
        public int? Avarage { get; set; }

        public static SchoolSummaryInfo Create(ShortSchoolSummary schoolSummary)
        {
            ////TODO: waiting iNow API
            return new SchoolSummaryInfo()
            {
                AttendanceCount = null,
                Avarage = null,
                DisciplinCount = null,
                SchoolDetails = schoolSummary
            };
        }
        public static PaginatedList<SchoolSummaryInfo> Create(PaginatedList<ShortSchoolSummary> schoolSummaries)
        {
            return new PaginatedList<SchoolSummaryInfo>(schoolSummaries.Select(Create), schoolSummaries.PageIndex,
                schoolSummaries.PageSize, schoolSummaries.TotalCount);
        } 
    }
}
