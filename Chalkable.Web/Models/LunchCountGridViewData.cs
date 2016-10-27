using System;
using System.Collections.Generic;
using Chalkable.BusinessLogic.Model;
using Chalkable.Web.Models.PersonViewDatas;

namespace Chalkable.Web.Models
{
    public class LunchCountGridViewData
    {
        public IList<StudentViewData> Students { get; set; }
        public IList<StaffViewData> Staffs { get; set; }
        public IList<MealItemViewData> MealItems { get; set; } 
        public int? ClassId { get; set; }
        public DateTime? Date { get; set; }
        public bool IncludeGuest { get; set; }

        public static LunchCountGridViewData Create(LunchCountGrid lunchCountGrid)
        {
            return new LunchCountGridViewData
            {
                Students = StudentViewData.Create(lunchCountGrid.Students),
                Staffs = StaffViewData.Create(lunchCountGrid.Staffs),
                MealItems = MealItemViewData.Create(lunchCountGrid.MealItems),
                Date = lunchCountGrid.Date,
                ClassId = lunchCountGrid.ClassId,
                IncludeGuest = lunchCountGrid.IncludeGuest
            };
        }
    }
}