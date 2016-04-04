using System.Collections.Generic;
using System.Linq;
using Chalkable.BusinessLogic.Common;
using Chalkable.BusinessLogic.Model;
using Chalkable.Common;
using Chalkable.Data.School.Model;
using Chalkable.Web.Models.ClassesViewData;

namespace Chalkable.Web.Models.PersonViewDatas
{
    public class TeacherStatsViewData
    {
        public int? Id { get; set; }
        public string DisplayName { get; set; }
        public IList<ShortClassViewData> Classes { get; set; }
        public int StudentsCount { get; set; }
        public int? DisciplinesCount { get; set; }
        public decimal? AbsenceCount { get; set; }
        public decimal? Presence { get; set; }
        public decimal? Average { get; set; }

        public string Gender { get; set; }

        public static TeacherStatsViewData Create(TeacherStatsInfo teacher)
        {
            return new TeacherStatsViewData
            {
                Id = teacher.Id,
                DisplayName = teacher.DisplayName,
                AbsenceCount = teacher.AbsenceCount,
                Presence = teacher.Presence,
                Average = teacher.Average,
                StudentsCount = teacher.StudentsCount,
                Classes = teacher.Classes.Select(ShortClassViewData.Create).ToList(),
                DisciplinesCount = teacher.DisciplinesCount,
                Gender = teacher.Gender
            };
        }
    }
}