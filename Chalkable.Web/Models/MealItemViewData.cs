using System.Collections.Generic;
using System.Linq;
using Chalkable.BusinessLogic.Model;

namespace Chalkable.Web.Models
{
    public class MealItemViewData
    {
        public MealTypeViewData MealType { get; set; }
        public IList<MealCountItemViewData> MealCountItems { get; set; }
        public int Total { get; set; }

        public static MealItemViewData Create(MealItem mealItem)
        {
            return new MealItemViewData
            {
                MealType = MealTypeViewData.Create(mealItem.MealType),
                MealCountItems = MealCountItemViewData.Create(mealItem.MealCountItems),
                Total = mealItem.Total
            };
        }
        public static IList<MealItemViewData> Create(IList<MealItem> mealItem)
        {
            return mealItem.Select(Create).ToList();
        }
    }
}