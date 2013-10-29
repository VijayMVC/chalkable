using System;
using System.Collections.Generic;
using System.Linq;
using Chalkable.Data.School.Model;

namespace Chalkable.Web.Models
{
    public class CourseViewData
    {
        public Guid Id { get; set; }
        public virtual string Code { get; set; }
        public virtual string Title { get; set; }
        public Guid? DepartmentId { get; set; }

        public static CourseViewData Create(Course course)
        {
            return new CourseViewData
                {
                    Id = course.Id,
                    Code = course.Code,
                    Title = course.Title,
                    DepartmentId = course.ChalkableDepartmentRef
                };
        }
        public static IList<CourseViewData> Create(IList<Course> courses)
        {
            return courses.Select(Create).ToList();
        } 
    }
}