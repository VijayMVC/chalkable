using System.Collections.Generic;

namespace Chalkable.StiConnector.Connectors.Model.Reports.ReportCards
{
    public class StudentGradingPeriod
    {
        public StudentGradingPeriod()
        {

            GradedItems = new List<StudentGradedItem>();
            Standards = new List<Standard>();
        }

        public List<StudentGradedItem> GradedItems { get; set; }
        public int GradingPeriodId { get; set; }
        public string GradingPeriodName { get; set; }
        public string Note { get; set; }
        public List<Standard> Standards { get; set; }
    }

    public class StudentGradedItem
    {
        public int AlphaGradeId { get; set; }
        public string AlphaGrade { get; set; }
        public IEnumerable<StudentGradingComment> Comments { get; set; }
        public string GradedItemName { get; set; }
        public bool IsExempt { get; set; }
        public decimal? NumericGrade { get; set; }
    }
    public class Standard
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Grade { get; set; }
        public IEnumerable<StudentGradingComment> Comments { get; set; }
    }
    public class StudentGradingComment
    {
        public string CommentHeaderName { get; set; }
        public string CommentHeaderDescription { get; set; }
        public string CommentCode { get; set; }
        public string Comment { get; set; }
    }

}
