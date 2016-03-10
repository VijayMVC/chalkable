using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Chalkable.BusinessLogic.Model.AcademicBenchmark;

namespace Chalkable.Web.Models.AcademicBenchmarksViewData
{
    public class TopicViewData
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
        public AuthorityViewData Authority { get; set; }
        public DocumentViewData Document { get; set; }
        public static TopicViewData Create(Topic topic)
        {
            return new TopicViewData
            {
                Id = topic.Id,
                Description = topic.Description,
                Number = topic.Number,
                Deepest = topic.Deepest,
                Level = topic.Level,
                Status = topic.Status,
                ParentId = topic.ParentId,
                Authority = AuthorityViewData.Create(topic.Authority),
                Document = DocumentViewData.Create(topic.Document)
            };
        }
    }
}