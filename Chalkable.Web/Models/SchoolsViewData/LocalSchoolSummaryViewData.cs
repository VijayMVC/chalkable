using System.Linq;
using Chalkable.BusinessLogic.Model;
using Chalkable.Common;

namespace Chalkable.Web.Models.SchoolsViewData
{
    public class LocalSchoolSummaryViewData : LocalSchoolViewData
    {
        public decimal? AbsenceCount { get; set; }
        public decimal? Presence { get; set; }
        public int StudentsCount { get; set; }
        public decimal? DisciplinesCount { get; set; }
        public decimal? Average { get; set; }

        public static LocalSchoolSummaryViewData Create(SchoolSummaryInfo school)
        {
            return new LocalSchoolSummaryViewData()
            {
                Id = school.SchoolDetails.Id,
                Name = school.SchoolDetails.Name,
                StudentsCount = school.SchoolDetails.StudentsCount,
                AbsenceCount = school.AbsenceCount,
                Presence = school.Presence,
                Average = school.Average,
                DisciplinesCount = school.DisciplinCount,
            };
        }

        public static PaginatedList<LocalSchoolSummaryViewData> Create(PaginatedList<SchoolSummaryInfo> schools)
        {
            return new PaginatedList<LocalSchoolSummaryViewData>(schools.Select(Create), schools.PageIndex, schools.PageSize, schools.TotalCount);
        }
    }
}