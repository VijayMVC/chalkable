using System.Collections.Generic;
using System.Linq;
using Chalkable.StiConnector.SyncModel;

namespace Chalkable.StiImport.Services.SyncModelAdapters
{
    public class GradingPeriodAdapter : SyncModelAdapter<GradingPeriod>
    {
        public GradingPeriodAdapter(AdapterLocator locator) : base(locator)
        {
        }

        protected override void InsertInternal(IList<GradingPeriod> entities)
        {
            var gPeriods = entities.Select(x => new Data.School.Model.GradingPeriod
            {
                Id = x.GradingPeriodID,
                AllowGradePosting = x.AllowGradePosting,
                Code = x.Code,
                Description = x.Description,
                EndDate = x.EndDate,
                EndTime = x.EndTime,
                MarkingPeriodRef = x.TermID,
                StartDate = x.StartDate,
                Name = x.Name,
                SchoolAnnouncement = x.SchoolAnnouncement,
                SchoolYearRef = x.AcadSessionID
            }).ToList();
            SchoolLocator.GradingPeriodService.Add(gPeriods);
        }

        protected override void UpdateInternal(IList<GradingPeriod> entities)
        {
            var gps = entities.Select(x => new Data.School.Model.GradingPeriod
            {
                Description = x.Description,
                EndDate = x.EndDate,
                Id = x.GradingPeriodID,
                Name = x.Name,
                SchoolYearRef = x.AcadSessionID,
                StartDate = x.StartDate,
                AllowGradePosting = x.AllowGradePosting,
                Code = x.Code,
                EndTime = x.EndTime,
                MarkingPeriodRef = x.TermID,
                SchoolAnnouncement = x.SchoolAnnouncement
            }).ToList();
            SchoolLocator.GradingPeriodService.Edit(gps);
        }

        protected override void DeleteInternal(IList<GradingPeriod> entities)
        {
            var ids = entities.Select(x => x.GradingPeriodID).ToList();
            SchoolLocator.GradingPeriodService.Delete(ids);
        }
    }
}