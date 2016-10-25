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
        public bool Override { get; set; }
        public bool Enabled { get; set; }

        public static MealCountItemViewData Create(MealCountItem mealCountItem)
        {
            return new MealCountItemViewData
            {
                Count = mealCountItem.Count,
                PersonId = mealCountItem.PersonId,
                Enabled = mealCountItem.Enabled,
                Guest = mealCountItem.Guest,
                Override = mealCountItem.Override
            };
        }

        public static IList<MealCountItemViewData> Create(IList<MealCountItem> mealCountItems)
        {
            return mealCountItems.Select(Create).ToList();
        }
    }
}