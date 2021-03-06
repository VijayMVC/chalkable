﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Chalkable.BusinessLogic.Model;
using Chalkable.BusinessLogic.Security;

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
            
            var scheduledDate = GetNearestScheduledDate(classId, date);
            var lunchCountsTask = ConnectorLocator.LunchConnector.GetLunchCount(classId, scheduledDate);          
            var students = ServiceLocator.StudentService.GetClassStudents(classId, null, true).OrderBy(x => x.LastName).ThenBy(x => x.FirstName).ToList();
            var currentClass = ServiceLocator.ClassService.GetById(classId);
            var staffs = ServiceLocator.StaffService.SearchStaff(Context.SchoolYearId, classId, null, null, false, 0, int.MaxValue)
                .OrderBy(x => x.Id != currentClass.PrimaryTeacherRef).ToList(); //primary theacher should be on the TOP         
            var mealTypes = ServiceLocator.MealTypeService.GetAll();
            var studentsCustomAlertDetails = ServiceLocator.StudentCustomAlertDetailService.GetListByStudentIds(students.Select(x=>x.Id).ToList());

            return LunchCountGrid.Create(classId, scheduledDate, students, staffs, mealTypes, studentsCustomAlertDetails, await lunchCountsTask, includeGuests);
        }


        private DateTime GetNearestScheduledDate(int classId, DateTime date)
        {
            var days = ServiceLocator.ClassService.GetDays(classId);
            return days.Any(x => x <= date) ? days.Last(x => x <= date) : days.FirstOrDefault(x => x >= date);
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
