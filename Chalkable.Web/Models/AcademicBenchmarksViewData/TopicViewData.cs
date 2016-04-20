using System;
using Chalkable.BusinessLogic.Model.AcademicBenchmark;

namespace Chalkable.Web.Models.AcademicBenchmarksViewData
{
    public class TopicViewData
    {
        public Guid Id { get; set; }
        public string Description { get; set; }
        public short Level { get; set; }
        public Guid? ParentId { get; set; }
        public bool IsDeepest { get; set; }
        public bool IsActive { get; set; }
        public Guid SubjectDocumentId { get; set; }
        public Guid CourseId { get; set; }

        public static TopicViewData Create(Topic topic)
        {
            return new TopicViewData
            {
                Id = topic.Id,
                Description = topic.Description,
                IsDeepest = topic.IsDeepest,
                Level = topic.Level,
                IsActive = topic.IsActive,
                ParentId = topic.ParentId,
                SubjectDocumentId = topic.SubjectDocument.Id,
                CourseId = topic.Course.Id
            };
        }
    }
}