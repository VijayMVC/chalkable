using System;
using System.Collections.Generic;

namespace Chalkable.StiConnector.Connectors.Model
{
    public class Activity
    {
        public IEnumerable<ActivityAttachment> Attachments { get; set; }
        public IEnumerable<ActivityStandard> Standards { get; set; }
        public int? CategoryId { get; set; }
        public string CategoryName { get; set; }
        public DateTime Date { get; set; }
        public string Description { get; set; }
        public bool DisplayInHomePortal { get; set; }
        public int Id { get; set; }
        public bool IsAssessment { get; set; }
        public bool IsDropped { get; set; }
        public bool IsScored { get; set; }
        public decimal? MaxScore { get; set; }
        public bool MayBeDropped { get; set; }
        public bool MayBeExempt { get; set; }
        public string Name { get; set; }
        public int SectionId { get; set; }
        public string Unit { get; set; }
        public decimal? WeightAddition { get; set; }
        public decimal? WeightMultiplier { get; set; }
        public bool Complete { get; set; }
    }
}
