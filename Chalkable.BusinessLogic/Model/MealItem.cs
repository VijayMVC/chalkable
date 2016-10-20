using System.Collections.Generic;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Model
{
    public class MealItem
    {
        public MealType MealType { get; set; }
        public IList<MealCountItem> MealCountItems { get; set; }
        public int Total => MealCountItems?.Count ?? 0;
    }
}
