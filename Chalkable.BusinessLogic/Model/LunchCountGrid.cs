using System;
using System.Collections.Generic;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Model
{
    public class LunchCountGrid
    {
        public IList<Student> Students { get; set; }
        public IList<Staff> Staffs { get; set; }
        public IList<MealItem> MealItems { get; set; }
        public int? ClassId { get; set; }
        public DateTime? Date { get; set; }
        public bool IncludeGuest { get; set; }
    }
}
