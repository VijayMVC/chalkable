using System.Collections.Generic;
using System.Linq;
using Chalkable.BusinessLogic.Model;

namespace Chalkable.Web.Models
{
    public class MealCountItemViewData
    {
        public int Count { get; set; }
        public int? PersonId { get; set; }
        public bool Guest { get; set; }
        public bool IsAbsent { get; set; }

        public static MealCountItemViewData Create(MealCountItem mealCountItem)
        {
            return new MealCountItemViewData
            {
                Count = mealCountItem.Count,
                PersonId = mealCountItem.PersonId,
                Guest = mealCountItem.Guest,
                IsAbsent = mealCountItem.IsAbsent
            };
        }

        public static IList<MealCountItemViewData> Create(IList<MealCountItem> mealCountItems)
        {
            return mealCountItems.Select(Create).ToList();
        }
    }
}