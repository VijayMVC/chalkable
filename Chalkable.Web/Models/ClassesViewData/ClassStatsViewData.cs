using System;

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

        //public static ClassStatsViewData Create( ... ) { }

    }
}