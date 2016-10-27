using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Chalkable.BusinessLogic.Common;
using Chalkable.BusinessLogic.Model;
using Chalkable.BusinessLogic.Security;
using Chalkable.Data.School.DataAccess;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.School
{
    public interface ILunchCountService
    {
        Task<LunchCountGrid> GetLunchCountGrid(int classId, DateTime date, bool includeGuests);
        void UpdateLunchCount(int classId, DateTime date, IList<LunchCount> lunchCounts);
    }
    
    public class LunchCountService : SisConnectedService, ILunchCountService
    {
        public LunchCountService(IServiceLocatorSchool serviceLocator) : base(serviceLocator)
        {
        }

        public async Task<LunchCountGrid> GetLunchCountGrid(int classId, DateTime date, bool includeGuests)
        {
            BaseSecurity.EnsureAdminOrTeacher(Context);
            var lunchCounts = ConnectorLocator.LunchConnector.GetLunchCount(classId, date);
            
            var lunchCountGrid = new LunchCountGrid();

            var students = ServiceLocator.StudentService.GetClassStudents(classId, null).OrderBy(x => x.LastName).ThenBy(x => x.FirstName).ToList();
            lunchCountGrid.Students = students;

            var currentClass = ServiceLocator.ClassService.GetById(classId);

            var staffs = ServiceLocator.StaffService.SearchStaff(Context.SchoolYearId, classId, null, null, true, 0, int.MaxValue)
                .OrderBy(x => x.Id == currentClass.PrimaryTeacherRef).ThenBy(x => x.LastName).ThenBy(x => x.FirstName).ToList();
            lunchCountGrid.Staffs = staffs;

            lunchCountGrid.ClassId = classId;
            lunchCountGrid.Date = date;
            lunchCountGrid.IncludeGuest = includeGuests;

            lunchCountGrid.MealItems = new List<MealItem>();

            var mealTypes = DoRead(u => new MealTypeDataAccess(u).GetAll()).OrderBy(x => x.Name);

            await lunchCounts;

            foreach (var mealType in mealTypes)
            {
                var mealItem = new MealItem();

                var mealCountItem = new List<MealCountItem>();
                mealCountItem.AddRange(from staff in staffs
                                       join lunchCount in lunchCounts.Result.Where(x => x.MealTypeId == mealType.Id)
                                            on staff.Id equals lunchCount.StaffId
                                       select CreateMealCount(lunchCount));

                if (includeGuests)
                    mealCountItem.AddRange(from lunchCount in
                                            lunchCounts.Result.Where(x => !x.StaffId.HasValue && !x.StudentId.HasValue && x.MealTypeId == mealType.Id)
                                            select CreateMealCount(lunchCount));

                mealCountItem.AddRange(from student in students
                                       join lunchCount in lunchCounts.Result.Where(x => x.MealTypeId == mealType.Id)
                                            on student.Id equals lunchCount.StudentId
                                       select CreateMealCount(lunchCount));

                mealItem.MealCountItems = mealCountItem;
                mealItem.MealType = mealType;

                lunchCountGrid.MealItems.Add(mealItem);
            }

            return lunchCountGrid;
        }

        private MealCountItem CreateMealCount(StiConnector.Connectors.Model.LunchCount lunchCount)
        {
            return new MealCountItem
            {
                Count = lunchCount.Count,
                Guest = !lunchCount.StudentId.HasValue && !lunchCount.StaffId.HasValue,
                PersonId = lunchCount.StaffId ?? lunchCount.StudentId
            };
        }

        public void UpdateLunchCount(int classId, DateTime date, IList<LunchCount> lunchCounts)
        {
            BaseSecurity.EnsureAdminOrTeacher(Context);
            var lunchCountsSti = lunchCounts.Select(x =>
                new StiConnector.Connectors.Model.LunchCount
                {
                    Date = x.Date,
                    StudentId = x.StudentId,
                    StaffId = x.StaffId,
                    Count = x.Count,
                    MealTypeId = x.MealTypeId
                }).ToList();
            ConnectorLocator.LunchConnector.UpdateLunchCount(classId, date, lunchCountsSti);
        }
    }
}
