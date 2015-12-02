using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chalkable.BusinessLogic.Common;
using Chalkable.Common;
using Chalkable.Data.School.Model;
using Chalkable.StiConnector.Connectors.Model;

namespace Chalkable.BusinessLogic.Model
{
    public class TeacherStatsInfo
    {
        public int? Id { get; set; }
        public string DisplayName { get; set; }
        public IList<ShortClassInfo> Classes { get; set; }
        public int StudentsCount { get; set; }
        public int? DisciplinesCount { get; set; }
        public decimal? AttendancesCount { get; set; }
        public decimal? Average { get; set; }

        public static TeacherStatsInfo Create(TeacherSummary teacher)
        {
            return new TeacherStatsInfo
            {
                Id = teacher.TeacherId,
                DisplayName = teacher.TeacherName,

                Classes = teacher.Classes.Select(x => new ShortClassInfo { Id = x.Id, Name = x.Name }).ToList(),

                AttendancesCount = teacher.AbsenceCount,
                Average = teacher.Average,
                
                DisciplinesCount = teacher.DisciplineCount,

                StudentsCount = teacher.EnrollmentCount,
            };
        }

        public static TeacherStatsInfo Create(Staff teacher, IList<ClassDetails> classes)
        {
            return new TeacherStatsInfo
            {
                AttendancesCount = null,
                Average = null,
                DisciplinesCount = null,
                DisplayName = teacher.FullName(false, true),
                Classes = classes.Select(x => new ShortClassInfo {Id = x.Id, Name = x.Name}).ToList(),

                Id = teacher.Id,

                StudentsCount = classes.Sum(x => x.StudentsCount)
            };
        }

        public static PaginatedList<TeacherStatsInfo> Create(IList<TeacherSummary> teachers, int start, int count)
        {
            return new PaginatedList<TeacherStatsInfo>(teachers.Select(Create).Skip(start).Take(count), start/count, count, teachers.Count);
        }

        public static PaginatedList<TeacherStatsInfo> Create(PaginatedList<Staff> teachers, IList<ClassDetails> classes)
        {
            var res = new List<TeacherStatsInfo>();
            foreach (var teacher in teachers)
            {
                res.Add(Create(teacher, classes.Where(x => x.ClassTeachers.Any(y => y.PersonRef == teacher.Id)).ToList()));
            }
            return new PaginatedList<TeacherStatsInfo>(res, teachers.PageIndex, teachers.PageSize, teachers.TotalCount);
        }
    }
}
