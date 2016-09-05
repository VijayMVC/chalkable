using System.Collections.Generic;
using System.Linq;
using Chalkable.StiConnector.SyncModel;

namespace Chalkable.StiImport.Services.SyncModelAdapters
{
    public class AttendanceMonthAdapter : SyncModelAdapter<AttendanceMonth>
    {
        public AttendanceMonthAdapter(AdapterLocator locator) : base(locator)
        {
        }
        private Data.School.Model.AttendanceMonth Selector(AttendanceMonth x)
        {
            return new Data.School.Model.AttendanceMonth
            {
                Id = x.AttendanceMonthID,
                SchoolYearRef = x.AcadSessionID,
                Name = x.Name,
                StartDate = x.StartDate,
                EndDate = x.EndDate,
                EndTime = x.EndTime,
                IsLockedAttendance = x.IsLockedAttendance,
                IsLockedDiscipline = x.IsLockedDiscipline,
            };
        }

        protected override void InsertInternal(IList<AttendanceMonth> entities)
        {
            var res = entities.Select(Selector).ToList();
            ServiceLocatorSchool.AttendanceMonthService.Add(res);
        }

        protected override void UpdateInternal(IList<AttendanceMonth> entities)
        {
            var res = entities.Select(Selector).ToList();
            ServiceLocatorSchool.AttendanceMonthService.Edit(res);
        }

        protected override void DeleteInternal(IList<AttendanceMonth> entities)
        {
            var attendanceMonthes = entities.Select(x => new Data.School.Model.AttendanceMonth { Id = x.AttendanceMonthID }).ToList();
            ServiceLocatorSchool.AttendanceMonthService.Delete(attendanceMonthes);
        }

        protected override void PrepareToDeleteInternal(IList<AttendanceMonth> entities)
        {
            throw new System.NotImplementedException();
        }
    }
}