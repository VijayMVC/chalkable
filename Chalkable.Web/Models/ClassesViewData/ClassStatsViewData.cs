using System;
using System.Linq;
using Chalkable.BusinessLogic.Common;
using Chalkable.Common;
using Chalkable.Data.School.Model;

namespace Chalkable.Web.Models.ClassesViewData
{
    public class ClassStatsViewData
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public Guid? DepartmentRef { get; set; }
        public string PrimaryTeacherDisplayName { get; set; }
        public int StudentsCount { get; set; }
        public int? AttendancesCount { get; set; }
        public int? DisciplinesCount { get; set; }
        public int? Avarage { get; set; }

        public static ClassStatsViewData Create(ClassDetails classDetails)
        {
            return new ClassStatsViewData()
            {
                Id = classDetails.Id,
                Name = classDetails.Name,
                DepartmentRef = classDetails.ChalkableDepartmentRef,

                PrimaryTeacherDisplayName = classDetails.PrimaryTeacher.FullName(false, true),
                StudentsCount = classDetails.StudentsCount,

                AttendancesCount = null,
                Avarage = null,
                DisciplinesCount = null
            };
        }

        public static PaginatedList<ClassStatsViewData> Create(PaginatedList<ClassDetails> classesDetails)
        {
            return new PaginatedList<ClassStatsViewData>(classesDetails.Select(Create), classesDetails.PageIndex,
                classesDetails.PageSize, classesDetails.TotalCount);
        }

    }
}