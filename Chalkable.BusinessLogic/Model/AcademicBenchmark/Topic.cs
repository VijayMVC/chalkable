using System;

namespace Chalkable.BusinessLogic.Model.AcademicBenchmark
{
    public class Topic
    {
        public Guid Id { get; set; }
        public string Number { get; set; }
        public string Description { get; set; }
        public string Deepest { get; set; }
        public short Level { get; set; }
        public string Status { get; set; }
        public Guid ParentId { get; set; }
        public bool IsDeepest => Deepest == "Y";
        public bool IsActive => Status == "Active";
        public Authority Authority { get; set; }
        public Document Document { get; set; }
        public static Topic Create(AcademicBenchmarkConnector.Models.Topic topic)
        {
            return new Topic
            {
                Id = topic.Id,
                Number = topic.Number,
                Description = topic.Description,
                Deepest = topic.Deepest,
                Level = topic.Level,
                Status = topic.Status,
                ParentId = topic.Parent.Id,
                Authority = Authority.Create(topic.Authority),
                Document = Document.Create(topic.Document)
            };
        }
    }
}

