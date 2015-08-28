using System.Collections.Generic;
using System.Linq;
using Chalkable.StiConnector.SyncModel;

namespace Chalkable.StiImport.Services.SyncModelAdapters
{
    public class BellScheduleAdapter : SyncModelAdapter<BellSchedule>
    {
        public BellScheduleAdapter(AdapterLocator locator) : base(locator)
        {
        }

        protected override void InsertInternal(IList<BellSchedule> entities)
        {
            var bellSchedules = entities.Select(x => new Data.School.Model.BellSchedule
            {
                Id = x.BellScheduleID,
                Code = x.Code,
                Description = x.Description,
                IsActive = x.IsActive,
                IsSystem = x.IsSystem,
                Name = x.Name,
                SchoolYearRef = x.AcadSessionID,
                TotalMinutes = x.TotalMinutes,
                UseStartEndTime = x.UseStartEndTime
            }).ToList();
            ServiceLocatorSchool.BellScheduleService.Add(bellSchedules);
        }

        protected override void UpdateInternal(IList<BellSchedule> entities)
        {
            var bellSchedules = entities.Select(x => new Data.School.Model.BellSchedule
            {
                Id = x.BellScheduleID,
                Code = x.Code,
                Description = x.Description,
                IsActive = x.IsActive,
                IsSystem = x.IsSystem,
                Name = x.Name,
                SchoolYearRef = x.AcadSessionID,
                TotalMinutes = x.TotalMinutes,
                UseStartEndTime = x.UseStartEndTime
            }).ToList();
            ServiceLocatorSchool.BellScheduleService.Edit(bellSchedules);
        }

        protected override void DeleteInternal(IList<BellSchedule> entities)
        {
            var bs = entities.Select(x => new Data.School.Model.BellSchedule
            {
                Id = x.BellScheduleID
            }).ToList();
            ServiceLocatorSchool.BellScheduleService.Delete(bs);
        }
    }
}