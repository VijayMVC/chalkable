namespace Chalkable.Web.Models.SchoolsViewData
{
    public class LocalSchoolSummaryViewData : LocalSchoolViewData
    {
        public int AttendancesCount { get; set; }
        public int StudentsCount { get; set; }
        public int DisciplinesCount { get; set; }
        public int Avarage { get; set; }
    }
}