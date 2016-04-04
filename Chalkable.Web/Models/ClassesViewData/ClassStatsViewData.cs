using System;
using System.Collections.Generic;
using System.Linq;
using Chalkable.BusinessLogic.Common;
using Chalkable.BusinessLogic.Model;
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
        public decimal? AbsenceCount { get; set; }
        public decimal? Presence { get; set; }
        public int? DisciplinesCount { get; set; }
        public decimal? Average { get; set; }
        public string ClassNumber { get; set; }
        public IList<int> TeacherIds { get; set; } 
        public static ClassStatsViewData Create(ClassStatsInfo classDetails)
        {
            return new ClassStatsViewData
            {
                Id = classDetails.Id,
                Name = classDetails.Name,
                DepartmentRef = classDetails.DepartmentRef,

                PrimaryTeacherDisplayName = classDetails.PrimaryTeacherDisplayName,
                StudentsCount = classDetails.StudentsCount,

                AbsenceCount = classDetails.AbsenceCount,
                Presence = classDetails.Presence,
                Average = classDetails.Average,
                DisciplinesCount = classDetails.DisciplinesCount,
                ClassNumber = classDetails.ClassNumber,
                TeacherIds = classDetails.TeacherIds
            };
        }
    }
}