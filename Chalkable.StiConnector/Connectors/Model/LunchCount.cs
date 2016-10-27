using System;

namespace Chalkable.StiConnector.Connectors.Model
{
    public class LunchCount
    {
        public int Count { get; set; }
        public DateTime Date { get; set; }
        public int MealTypeId { get; set; }
        public int? StaffId { get; set; }
        public int? StudentId { get; set; }
    }
}
