using System;
using System.Collections.Generic;
using System.Linq;
using Chalkable.BusinessLogic.Common;
using Chalkable.BusinessLogic.Model;
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
        public int? SchoolYearId { get; set; }
        public int CourseTypeId { get; set; }
        
        protected ShortClassViewData(Class cClass)
        {
            Id = cClass.Id;
            Name = cClass.Name;
            Description = cClass.Description;
            ClassNumber = cClass.ClassNumber;
            SchoolYearId = cClass.SchoolYearRef;
            CourseTypeId = cClass.CourseTypeRef;
        }

        public static ShortClassViewData Create(Class cClass)
        {
            return new ShortClassViewData(cClass);
        }

        public static ShortClassViewData Create(ShortClassInfo clazz)
        {
            return new ShortClassViewData(new Class { Id = clazz.Id, Name = clazz.Name });
        }
    }

    public class ClassViewData : ShortClassViewData
    {
        public Guid? DepartmentId { get; set; }
        public ShortPersonViewData Teacher { get; set; }
        public IList<int> MarkingPeriodsId { get; set; }
        public IList<int> TeachersIds { get; set; } 
        public SchoolYearViewData SchoolYear { get; set; }

        protected ClassViewData(ClassDetails classComplex) : base(classComplex)
        {
            DepartmentId = classComplex.ChalkableDepartmentRef;
            if (classComplex.PrimaryTeacherRef.HasValue && classComplex.PrimaryTeacher != null)
            {
                Teacher = ShortPersonViewData.Create(classComplex.PrimaryTeacher);
                Teacher.DisplayName = classComplex.PrimaryTeacher.DisplayName();
            }
            if (classComplex.SchoolYear != null)
                SchoolYear = SchoolYearViewData.Create(classComplex.SchoolYear);
            MarkingPeriodsId = classComplex.MarkingPeriodClasses.Select(x => x.MarkingPeriodRef).ToList();
            TeachersIds = classComplex.ClassTeachers.Select(x => x.PersonRef).ToList();
        }
        
        public static ClassViewData Create(ClassDetails classComplex)
        {
           return new ClassViewData(classComplex);
        }
        public static IList<ClassViewData> Create(IList<ClassDetails> classComplexs)
        {
            return classComplexs.Select(Create).ToList();
        } 
    }
}