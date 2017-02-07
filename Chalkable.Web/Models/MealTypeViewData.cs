using Chalkable.Data.School.Model;

namespace Chalkable.Web.Models
{
    public class MealTypeViewData
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public static MealTypeViewData Create(MealType mealType)
        {
            return new MealTypeViewData
            {
                Id = mealType.Id,
                Name = mealType.Name
            };
        }
    }
}