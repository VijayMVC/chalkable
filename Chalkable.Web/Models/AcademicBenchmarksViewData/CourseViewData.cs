using System;
using Chalkable.BusinessLogic.Model.AcademicBenchmark;

namespace Chalkable.Web.Models.AcademicBenchmarksViewData
{
    public class CourseViewData
    {
        public Guid Id { get; set; }
        public string Description { get; set; }
        public static CourseViewData Create(Course course)
        {
            return new CourseViewData
            {
                Id = course.Id,
                Description = course.Description
            };
        }

        public static CourseViewData Create(Data.AcademicBenchmark.Model.Course model)
        {
            return new CourseViewData
            {
                Id = model.Id,
                Description = model.Description
            };
        }
    }
}