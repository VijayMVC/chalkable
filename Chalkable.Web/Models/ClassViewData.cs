using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Chalkable.Data.School.Model;
using Chalkable.Web.Models.PersonViewDatas;

namespace Chalkable.Web.Models
{
    public class ClassViewData
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public CourseViewData Course { get; set; }
        public IdNameViewData GradeLevel { get; set; }
        public ShortPersonViewData Teacher { get; set; }
        public IList<Guid> MarkingPeriodsId { get; set; }
        
        public static ClassViewData Create(ClassComplex classComplex)
        {
            var res = new ClassViewData
                {
                    Id = classComplex.Id,
                    Name = classComplex.Name,
                    Description = classComplex.Description,
                    Course = CourseViewData.Create(classComplex.Course),
                    GradeLevel = IdNameViewData.Create(classComplex.GradeLevelRef, classComplex.GradeLevel.Name),
                    Teacher = ShortPersonViewData.Create(classComplex.Teacher),
                    MarkingPeriodsId = classComplex.MarkingPeriodClass.Select(x=>x.MarkingPeriodRef).ToList()
                };
            res.Teacher.DisplayName = classComplex.Teacher.ShortSalutationName;
            return res;
        }
        public static IList<ClassViewData> Create(IList<ClassComplex> classComplexs)
        {
            return classComplexs.Select(Create).ToList();
        } 
    }
}