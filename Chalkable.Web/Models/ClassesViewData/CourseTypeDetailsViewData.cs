using System.Collections.Generic;
using System.Linq;
using Chalkable.Data.School.Model;
using Chalkable.Web.Models.SchoolsViewData;
using CourseType = Chalkable.Data.School.Model.CourseType;

namespace Chalkable.Web.Models.ClassesViewData
{
    public class CourseTypeDetailsViewData : ShortCourseTypeViewData
    {
        public IList<CourseViewData> Courses { get; set; }

        public static IList<CourseTypeDetailsViewData> Create(IList<CourseDetails> courses, IList<CourseType> courseTypes)
        {
            var res = new List<CourseTypeDetailsViewData>();
            foreach (var courseType in courseTypes)
            {
                var currentCourses = courses.Where(x => x.CourseTypeRef == courseType.Id).ToList();
                if (currentCourses.Count == 0) continue;
                res.Add(new CourseTypeDetailsViewData
                    {
                         CoureTypeId = courseType.Id,
                         CoureTypeName = courseType.Name,
                         CoureTypeCode = courseType.Code,
                         Courses = CourseViewData.Create(currentCourses)
                    });
            }
            return res;
        }
    }

    public class CourseViewData : ShortClassViewData 
    {
        public IList<ShortClassViewData> Classes { get; set; }
        protected CourseViewData(Class cClass) : base(cClass)
        {
        }

        public static IList<CourseViewData> Create(IList<CourseDetails> courses)
        {
            return courses.Select(x => new CourseViewData(x)
                {
                    Classes = x.Classes.Select(ShortClassViewData.Create).ToList()
                }).ToList();
        }
    }

    public class ShortCourseTypeViewData
    {
        public int CoureTypeId { get; set; }
        public string CoureTypeCode { get; set; }
        public string CoureTypeName { get; set; }

        public static ShortCourseTypeViewData Create(CourseType courseType)
        {
            return new ShortCourseTypeViewData
            {
                CoureTypeId = courseType.Id,
                CoureTypeCode = courseType.Code,
                CoureTypeName = courseType.Name
            };
        }

        public static IList<ShortCourseTypeViewData> Create(IList<CourseType> courseType)
        {
            return courseType.Select(Create).ToList();
        }
    }

    public class AllSchoolsActiveClassesViewData
    {
        public IList<ClassViewData> Classes { get; set; }
        public IList<ShortCourseTypeViewData> CourseTypes { get; set; }
        public IList<LocalSchoolViewData> Schools { get; set; }

        public static AllSchoolsActiveClassesViewData Create(IList<ClassDetails> classes, IList<CourseType> courseTypes, IList<School> schools)
        {
            var res = new AllSchoolsActiveClassesViewData();
            res.Classes = ClassViewData.Create(classes);
            res.CourseTypes = ShortCourseTypeViewData.Create(courseTypes);
            res.Schools = LocalSchoolViewData.Create(schools);
            return res;
        }
    }
}