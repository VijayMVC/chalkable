using System;
using System.Collections.Generic;
using System.Linq;
using Chalkable.BusinessLogic.Common;
using Chalkable.Data.School.Model;
using Chalkable.Web.Models.PersonViewDatas;

namespace Chalkable.Web.Models.ClassesViewData
{
    public class ShortClassViewData
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string ClassNumber { get; set; }
        public string Description { get; set; }

        protected ShortClassViewData(Class cClass)
        {
            Id = cClass.Id;
            Name = cClass.Name;
            Description = cClass.Description;
            ClassNumber = cClass.ClassNumber;
        }

        public static ShortClassViewData Create(Class cClass)
        {
            return new ShortClassViewData(cClass);
        }
    }

    public class ClassViewData : ShortClassViewData
    {
        public Guid? DepartmentId { get; set; }
        public GradeLevelViewData GradeLevel { get; set; }
        public ShortPersonViewData Teacher { get; set; }
        public IList<int> MarkingPeriodsId { get; set; }
        
        protected ClassViewData(ClassDetails classComplex) : base(classComplex)
        {
            DepartmentId = classComplex.ChalkableDepartmentRef;
            GradeLevel =  GradeLevelViewData.Create(classComplex.GradeLevel);
            if (classComplex.PrimaryTeacherRef.HasValue && classComplex.PrimaryTeacher != null)
            {
                Teacher = ShortPersonViewData.Create(classComplex.PrimaryTeacher);
                Teacher.DisplayName = classComplex.PrimaryTeacher.DisplayName();
            }
            MarkingPeriodsId = classComplex.MarkingPeriodClasses.Select(x => x.MarkingPeriodRef).ToList();
        }
        
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
            return new ClassViewData(new ClassDetails {Id = id, Name = name});
        }
    }
}