using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chalkable.BusinessLogic.Services.School;
using Chalkable.Common;
using Chalkable.Data.School.Model;
using Chalkable.StiConnector.Connectors.Model;

namespace Chalkable.BusinessLogic.Model
{
    public class SchoolSummaryInfo
    {
        public ShortSchoolSummary SchoolDetails { get; set; }
        public decimal? AbsenceCount { get; set; }
        public decimal? Presence { get; set; }
        public decimal? DisciplinCount { get; set; }
        public decimal? Average { get; set; }

        public static SchoolSummaryInfo Create(ShortSchoolSummary schoolSummary)
        {
            return new SchoolSummaryInfo()
            {
                Presence = null,
                AbsenceCount = null,
                Average = null,
                DisciplinCount = null,
                SchoolDetails = schoolSummary
            };
        }

        public static SchoolSummaryInfo Create(SchoolSummary schoolSummary)
        {
            return new SchoolSummaryInfo()
            {
                Presence = schoolSummary.EnrollmentCount != 0 ?
                    AttendanceService.CalculatePresencePercent(schoolSummary.AbsenceCount, schoolSummary.EnrollmentCount) 
                    : (decimal?)null,
                AbsenceCount = schoolSummary.AbsenceCount,
                Average = schoolSummary.Average,
                DisciplinCount = schoolSummary.DisciplineCount,
                SchoolDetails = new ShortSchoolSummary()
                {
                    Id = schoolSummary.SchoolId,
                    Name = schoolSummary.SchoolName,
                    StudentsCount = schoolSummary.EnrollmentCount
                }
            };
        }

        public static PaginatedList<SchoolSummaryInfo> Create(PaginatedList<ShortSchoolSummary> schoolSummaries)
        {
            return new PaginatedList<SchoolSummaryInfo>(schoolSummaries.Select(Create), schoolSummaries.PageIndex,
                schoolSummaries.PageSize, schoolSummaries.TotalCount);
        }

        public static PaginatedList<SchoolSummaryInfo> Create(IList<SchoolSummary> schoolSummaries, int start, int count, int allCount)
        {
            return new PaginatedList<SchoolSummaryInfo>(schoolSummaries.Select(Create).Skip(start).Take(count), start/count, count, allCount);
        } 
    }
}
