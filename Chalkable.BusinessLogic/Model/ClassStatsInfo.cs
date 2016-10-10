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
    public class ClassStatsInfo
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public Guid? DepartmentRef { get; set; }
        public string PrimaryTeacherDisplayName { get; set; }
        public int StudentsCount { get; set; }
        public decimal? AbsenceCount{ get; set; }
        public decimal? Presence { get; set; }
        public int? DisciplinesCount { get; set; }
        public decimal? Average { get; set; }
        public string ClassNumber { get; set; }

        public IList<int> TeacherIds { get; set; } 

        public static ClassStatsInfo Create(SectionSummary section, Class @class, IList<ClassTeacher> classTeachers)
        {
            return new ClassStatsInfo
            {
                Id = section.SectionId,
                Name = section.SectionName,
                PrimaryTeacherDisplayName = section.TeacherName,
                StudentsCount = section.EnrollmentCount,
                Average = section.Average,
                DisciplinesCount = section.DisciplineCount,
                AbsenceCount = section.AbsenceCount,
                Presence = section.EnrollmentCount != 0 ?
                    AttendanceService.CalculatePresencePercent(section.AbsenceCount, section.EnrollmentCount) : 0,
                DepartmentRef = @class?.ChalkableDepartmentRef,
                ClassNumber = @class?.ClassNumber,
                TeacherIds = classTeachers?.Select(x => x.PersonRef).ToList()
            };
        }

        public static IList<ClassStatsInfo> Create(IList<SectionSummary> sections, IList<Class> @classes, IList<ClassTeacher> classTeachers)
        {
            var res = new List<ClassStatsInfo>();

            foreach (var section in sections)
            {
                res.Add(Create(section, @classes.FirstOrDefault(x => x.Id == section.SectionId),
                    classTeachers?.Where(y => y.ClassRef == section.SectionId).ToList()));
            }

            return res;
        }

        public static ClassStatsInfo Create(ClassDetails classDetails)
        {
            return new ClassStatsInfo
            {
                Id = classDetails.Id,
                Name = classDetails.Name,
                DepartmentRef = classDetails.ChalkableDepartmentRef,

                PrimaryTeacherDisplayName = classDetails.PrimaryTeacher?.FullName(false),
                StudentsCount = classDetails.StudentsCount,

                AbsenceCount = null,
                Presence = null,
                Average = null,
                DisciplinesCount = null,
                ClassNumber = classDetails.ClassNumber,
                TeacherIds = new List<int>(classDetails.ClassTeachers.Select(x => x.PersonRef))
            };
        }
    }
}
