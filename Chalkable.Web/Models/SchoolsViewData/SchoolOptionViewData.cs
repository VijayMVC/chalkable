using Chalkable.Data.School.Model;

namespace Chalkable.Web.Models.SchoolsViewData
{
    public class SchoolOptionViewData
    {
        public bool AllowScoreEntryForUnexcused { get; set; }

        public static SchoolOptionViewData Create(SchoolOption schoolOption)
        {
            var res = new SchoolOptionViewData();
            if (schoolOption != null)
            {
                res.AllowScoreEntryForUnexcused = schoolOption.AllowScoreEntryForUnexcused;
            }
            return res;
        }
    }
}