using System;
using System.Collections.Generic;
using System.Linq;
using Chalkable.BusinessLogic.Common;
using Chalkable.BusinessLogic.Model;
using Chalkable.BusinessLogic.Security;
using Chalkable.BusinessLogic.Services.School;
using Chalkable.Data.School.DataAccess;
using Chalkable.Data.School.Model;
using Chalkable.StiConnector.Connectors;

namespace Chalkable.BusinessLogic.Services
{
    public interface ILunchCountService
    {
        LunchCountGrid GetLunchCountGrid(int classId, DateTime date, bool includeGuests);
        void UpdateLunchCount(IList<LunchCount> lunchCounts);
    }
    
    public class LunchCountService : SisConnectedService, ILunchCountService
    {
        public LunchCountService(IServiceLocatorSchool serviceLocator) : base(serviceLocator)
        {
        }

        public LunchCountGrid GetLunchCountGrid(int classId, DateTime date, bool includeGuests)
        {
            BaseSecurity.EnsureAdminOrTeacher(Context);
            var lunchCounts = ConnectorLocator.LunchConnector.GetLunchCount(classId, date);
            
            var lunchCountGrid = new LunchCountGrid();

            var students = ServiceLocator.StudentService.GetClassStudents(classId, null);
            lunchCountGrid.Students = students;

            IList<Staff> staffs = ServiceLocator.StaffService.SearchStaff(Context.SchoolYearId, classId, null, null, true, 0, int.MaxValue);
            lunchCountGrid.Staffs = staffs;

            lunchCountGrid.ClassId = classId;
            lunchCountGrid.Date = date;
            lunchCountGrid.IncludeGuest = includeGuests;

            lunchCountGrid.MealItems = new List<MealItem>();

            var mealTypes = DoRead(u => new MealTypeDataAccess(u).GetAll());

            foreach (var mealType in mealTypes)
            {
                var mealItem = new MealItem();

                var mealCountItem = new List<MealCountItem>();
                mealCountItem.AddRange(from staff in staffs
                                       join lunchCount in lunchCounts.Where(x => x.StaffId.HasValue && x.MealTypeId == mealType.Id)
                                            on staff.Id equals lunchCount.StaffId.Value
                                       orderby staff.DisplayName()
                                       select new MealCountItem { Count = lunchCount.Count, Guest = false, PersonId = staff.Id });

                mealCountItem.AddRange(from student in students
                                       join lunchCount in lunchCounts.Where(x => x.StudentId.HasValue && x.MealTypeId == mealType.Id)
                                            on student.Id equals lunchCount.StudentId.Value
                                       orderby student.DisplayName()
                                       select new MealCountItem { Count = lunchCount.Count, Guest = false, PersonId = student.Id });

                if(includeGuests)
                    mealCountItem.AddRange(from lunchCount in
                                       lunchCounts.Where(x => !x.StaffId.HasValue && !x.StudentId.HasValue && x.MealTypeId == mealType.Id)
                                       select new MealCountItem { Count = lunchCount.Count, Guest = true, PersonId = null });

                mealItem.MealCountItems = mealCountItem;
                mealItem.MealType = mealType;

                lunchCountGrid.MealItems.Add(mealItem);
            }

            return lunchCountGrid;
        }

        public void UpdateLunchCount(IList<LunchCount> lunchCounts)
        {
            
        }
    }
}
