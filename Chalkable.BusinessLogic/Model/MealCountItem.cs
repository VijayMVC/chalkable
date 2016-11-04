namespace Chalkable.BusinessLogic.Model
{
    public class MealCountItem
    {
        public int Count { get; set; }
        public int? PersonId { get; set; }
        public bool Guest { get; set; }
        public bool IsAbsent { get; set; }
    }
}
