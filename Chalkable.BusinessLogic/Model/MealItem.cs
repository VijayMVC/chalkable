﻿using System.Collections.Generic;
using System.Linq;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Model
{
    public class MealItem
    {
        public MealType MealType { get; set; }
        public IList<MealCountItem> MealCountItems { get; set; }
        public int Total => MealCountItems?.Sum(x => x.Count) ?? 0;
    }
}
