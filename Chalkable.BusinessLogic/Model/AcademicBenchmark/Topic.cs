using System;

namespace Chalkable.BusinessLogic.Model.AcademicBenchmark
{
    public class Topic
    {
        public Guid Id { get; set; }
        public string Description { get; set; }
        public short Level { get; set; }
        public Guid? ParentId { get; set; }
        public bool IsDeepest { get; set; } 
        public bool IsActive { get; set; }
        public Course Course { get; set; }
        public SubjectDocument SubjectDocument { get; set; }
        public static Topic Create(AcademicBenchmarkConnector.Models.Topic topic)
        {
            return new Topic
            {
                Id = topic.Id,
                Description = topic.Description,
                IsDeepest = topic.IsDeepest,
                Level = topic.Level,
                IsActive = topic.IsActive,
                ParentId = topic.Parent?.Id,
                SubjectDocument = SubjectDocument.Create(topic.SubjectDocument),
                Course = Course.Create(topic.Course)
            };
        }
    }
}

