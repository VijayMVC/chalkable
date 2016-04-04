using System;

namespace Chalkable.BusinessLogic.Model.AcademicBenchmark
{
    public class Course
    {
        public Guid Id { get; set; }
        public string Description { get; set; }
        public static Course Create(AcademicBenchmarkConnector.Models.Course course)
        {
            return new Course
            {
                Id = course.Id,
                Description = course.Description
            };
        }
    }
}
