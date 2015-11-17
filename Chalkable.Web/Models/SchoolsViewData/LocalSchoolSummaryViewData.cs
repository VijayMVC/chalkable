using System.Linq;
using Chalkable.BusinessLogic.Model;
using Chalkable.Common;

namespace Chalkable.Web.Models.SchoolsViewData
{
    public class LocalSchoolSummaryViewData : LocalSchoolViewData
    {
        public int? AttendancesCount { get; set; }
        public int StudentsCount { get; set; }
        public int? DisciplinesCount { get; set; }
        public int? Avarage { get; set; }

        public static LocalSchoolSummaryViewData Create(SchoolSummaryInfo school)
        {
            return new LocalSchoolSummaryViewData()
            {
                Id = school.SchoolDetails.Id,
                Name = school.SchoolDetails.Name,
                StudentsCount = school.SchoolDetails.StudentsCount,
                AttendancesCount = school.AttendanceCount,
                Avarage = school.Avarage,
                DisciplinesCount = school.DisciplinCount,
            };
        }

        public static PaginatedList<LocalSchoolSummaryViewData> Create(PaginatedList<SchoolSummaryInfo> schools)
        {
            return new PaginatedList<LocalSchoolSummaryViewData>(schools.Select(Create), schools.PageIndex, schools.PageSize, schools.TotalCount);
        }
    }
}