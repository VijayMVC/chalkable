namespace Chalkable.BusinessLogic.Model
{
    public class MealCountItem
    {
        public int Count { get; set; }
        public int? PersonId { get; set; }
        public bool Guest { get; set; }

        public static MealCountItem Create(StiConnector.Connectors.Model.LunchCount lunchCount)
        {
            return new MealCountItem
            {
                Count = lunchCount.Count,
                Guest = !lunchCount.StudentId.HasValue && !lunchCount.StaffId.HasValue,
                PersonId = lunchCount.StaffId ?? lunchCount.StudentId
            };
        }
    }
}
