using System;

namespace Chalkable.StiConnector.Connectors.Model
{
    public class Score
    {
        public DateTime? ActivityDate { get; set; }
        public string AbsenceCategory { get; set; }
        public bool Absent { get; set; }
        public int ActivityId { get; set; }
        public string ActivityName { get; set; }
        public int? AlphaGradeId { get; set; }
        public int? AlternateScoreId { get; set; }
        public string Comment { get; set; }
        public bool Dropped { get; set; }
        public bool Exempt { get; set; }
        public bool Incomplete { get; set; }
        public bool Late { get; set; }
        public decimal? NumericScore { get; set; }
        public bool OverMaxScore { get; set; }
        public string ScoreValue { get; set; }
        public int StudentId { get; set; }
        public bool Withdrawn { get; set; }
    }
}
