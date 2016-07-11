using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chalkable.BusinessLogic.Common;
using Chalkable.BusinessLogic.Services.School;
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
        public decimal? AbsenceCount { get; set; }
        public decimal? Presence { get; set; }
        public decimal? Average { get; set; }
        public string Gender { get; set; }

        public static TeacherStatsInfo Create(TeacherSummary teacher)
        {
            return new TeacherStatsInfo
            {
                Id = teacher.TeacherId,
                DisplayName = teacher.TeacherName,

                Classes = teacher.Classes.Select(x => new ShortClassInfo { Id = x.Id, Name = x.Name }).ToList(),
                Presence = teacher.EnrollmentCount != 0 ?
                    AttendanceService.CalculatePresencePercent(teacher.AbsenceCount, teacher.EnrollmentCount) 
                    : (decimal?)null,
                AbsenceCount = teacher.AbsenceCount,
                Average = teacher.Average,
                
                DisciplinesCount = teacher.DisciplineCount,

                StudentsCount = teacher.EnrollmentCount,
                Gender = teacher.TeacherGender
            };

            
        }



        public static TeacherStatsInfo Create(ShortStaffSummary teacher, IList<ClassDetails> classes)
        {
            return new TeacherStatsInfo
            {
                Presence = null,
                AbsenceCount = null,
                Average = null,
                DisciplinesCount = null,
                DisplayName = teacher.FullName(false, true),
                Classes = classes.Select(x => new ShortClassInfo {Id = x.Id, Name = x.Name}).ToList(),

                Id = teacher.Id,

                StudentsCount = teacher.StudentsCount,
                Gender = teacher.Gender
            };
        }

        public static IList<TeacherStatsInfo> Create(IList<ShortStaffSummary> teachers, IList<ClassDetails> classes)
        {
            var res = new List<TeacherStatsInfo>();
            foreach (var teacher in teachers)
            {
                res.Add(Create(teacher, classes.Where(x => x.ClassTeachers.Any(y => y.PersonRef == teacher.Id)).ToList()));
            }
            return res;
        }
    }
}
