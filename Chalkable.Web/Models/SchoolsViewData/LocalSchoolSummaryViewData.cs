using System.Linq;
using Chalkable.BusinessLogic.Model;
using Chalkable.Common;
using Chalkable.Data.School.Model;
using Chalkable.StiConnector.Connectors.Model;

namespace Chalkable.Web.Models.SchoolsViewData
{
    public class LocalSchoolSummaryViewData : LocalSchoolViewData
    {
        public decimal? AttendancesCount { get; set; }
        public int StudentsCount { get; set; }
        public decimal? DisciplinesCount { get; set; }
        public decimal? Avarage { get; set; }

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