using System.Collections.Generic;
using System.Linq;
using Chalkable.BusinessLogic.Common;
using Chalkable.Common;
using Chalkable.Data.School.Model;
using Chalkable.Web.Models.ClassesViewData;

namespace Chalkable.Web.Models.PersonViewDatas
{
    public class TeacherStatsViewData : StaffViewData
    {
        public IList<ShortClassViewData> Classes { get; set; }
        public int StudentsCount { get; set; }
        public int? DisciplinesCount { get; set; }
        public int? AttendancesCount { get; set; }
        public double? Average { get; set; }

        protected TeacherStatsViewData(Staff staff) : base(staff)
        {
        }

        public static TeacherStatsViewData Create(Staff teacher, IList<ClassDetails> classes)
        {
            return new TeacherStatsViewData(teacher)
            {
                AttendancesCount = null,
                Average = null,
                DisciplinesCount = null,

                DisplayName = teacher.FullName(false, true),

                Classes = classes.Select(ShortClassViewData.Create).ToList(),
                StudentsCount = classes.Sum(x => x.StudentsCount)
            };
        }

        public static PaginatedList<TeacherStatsViewData> Create(PaginatedList<Staff> teachers,
            IList<ClassDetails> classes)
        {
            var res = new List<TeacherStatsViewData>();

            foreach (var teacher in teachers)
            {
                res.Add(Create(teacher, classes.Where(x => x.ClassTeachers.Any(y => y.PersonRef == teacher.Id)).ToList()));
            }

            return new PaginatedList<TeacherStatsViewData>(res, teachers.PageIndex, teachers.PageSize, teachers.TotalCount);
        } 
    }
}