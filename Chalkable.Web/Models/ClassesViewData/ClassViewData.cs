using System;
using System.Collections.Generic;
using System.Linq;
using Chalkable.Data.School.Model;
using Chalkable.Web.Models.PersonViewDatas;

namespace Chalkable.Web.Models.ClassesViewData
{
    public class ClassViewData
    {
        public int Id { get; set; }
        public string ClassNumber { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public Guid? DepartmentId { get; set; }
        public GradeLevelViewData GradeLevel { get; set; }
        public ShortPersonViewData Teacher { get; set; }
        public IList<int> MarkingPeriodsId { get; set; }
        

        protected ClassViewData(ClassDetails classComplex)
        {
            Id = classComplex.Id;
            ClassNumber = classComplex.ClassNumber;
            Name = classComplex.Name;
            Description = classComplex.Description;
            DepartmentId = classComplex.ChalkableDepartmentRef;
            GradeLevel =  GradeLevelViewData.Create(classComplex.GradeLevel);
            if (classComplex.PrimaryTeacherRef.HasValue && classComplex.PrimaryTeacher != null)
            {
                Teacher = ShortPersonViewData.Create(classComplex.PrimaryTeacher);
                Teacher.DisplayName = classComplex.PrimaryTeacher.ShortSalutationName;
            }
            MarkingPeriodsId = classComplex.MarkingPeriodClasses.Select(x => x.MarkingPeriodRef).ToList();
        }
        private ClassViewData() {}

        public static ClassViewData Create(ClassDetails classComplex)
        {
           return new ClassViewData(classComplex);
        }
        public static IList<ClassViewData> Create(IList<ClassDetails> classComplexs)
        {
            return classComplexs.Select(Create).ToList();
        } 

        public static ClassViewData Create(int id, string name)
        {
            return  new ClassViewData{Id = id, Name = name};
        }
    }
}