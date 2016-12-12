using Chalkable.Data.Common;

namespace Chalkable.Data.School.Model
{
    public class StudentCustomAlertDetail
    {
        [PrimaryKeyFieldAttr]
        public int Id { get; set; }
        public int CustomAlertDetailId { get; set; }
        public int StudentRef { get; set; }
        public int SchoolYearRef { get; set; }
        public string AlertText { get; set; }
        public string CurrentValue { get; set; }
    }
}
