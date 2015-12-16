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
    public class ClassStatsInfo
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public Guid? DepartmentRef { get; set; }
        public string PrimaryTeacherDisplayName { get; set; }
        public int StudentsCount { get; set; }
        public decimal? AttendancesCount { get; set; }
        public int? DisciplinesCount { get; set; }
        public decimal? Average { get; set; }

        public static ClassStatsInfo Create(SectionSummary section, Class clazz)
        {
            return new ClassStatsInfo()
            {
                Id = section.SectionId,
                Name = section.SectionName,
                PrimaryTeacherDisplayName = section.TeacherName,
                StudentsCount = section.EnrollmentCount,
                Average = section.Average,
                DisciplinesCount = section.DisciplineCount,
                AttendancesCount = section.AbsenceCount,
                DepartmentRef = clazz.ChalkableDepartmentRef
            };
        }

        public static ClassStatsInfo Create(ClassDetails classDetails)
        {
            return new ClassStatsInfo()
            {
                Id = classDetails.Id,
                Name = classDetails.Name,
                DepartmentRef = classDetails.ChalkableDepartmentRef,

                PrimaryTeacherDisplayName = classDetails.PrimaryTeacher?.FullName(upper: false),
                StudentsCount = classDetails.StudentsCount,

                AttendancesCount = null,
                Average = null,
                DisciplinesCount = null
            };
        }

        public static PaginatedList<ClassStatsInfo> Create(PaginatedList<ClassDetails> classes)
        {
            return new PaginatedList<ClassStatsInfo>(classes.Select(Create), classes.PageIndex, classes.PageSize, classes.TotalCount);
        }
    }
}
