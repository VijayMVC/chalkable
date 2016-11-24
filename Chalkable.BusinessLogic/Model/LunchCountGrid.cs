using System;
using System.Collections.Generic;
using System.Linq;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Model
{
    public class LunchCountGrid
    {
        public IList<StudentLunchCount> Students { get; set; }
        public IList<Staff> Staffs { get; set; }
        public IList<MealItem> MealItems { get; set; }
        public int ClassId { get; set; }
        public DateTime Date { get; set; }
        public bool IncludeGuest { get; set; }


        public static LunchCountGrid Create(int classId, DateTime date, IList<Student> students, IList<Staff> staffs,
            IList<MealType> mealTypes, IList<StiConnector.Connectors.Model.LunchCount> lunchCounts, bool includeGuests)
        {
            var mealItems = new List<MealItem>();
            foreach (var mealType in mealTypes)
            {
                var mealCountItem = lunchCounts.Where(x => x.MealTypeId == mealType.Id)
                    .Where(x => includeGuests || x.StudentId.HasValue || x.StaffId.HasValue) //guest if studentId and staffId is null.
                    .Select(MealCountItem.Create).ToList();

                mealItems.Add(MealItem.Create(mealType, mealCountItem));
            }


            return new LunchCountGrid
            {
                Students = students.Select(student => StudentLunchCount.Create(student, lunchCounts.Any(x => x.StudentId == student.Id && x.IsAbsent))).ToList(),
                MealItems = mealItems,
                Date = date,
                ClassId = classId,
                IncludeGuest = includeGuests,
                Staffs = staffs
            };
        }
    }
}
