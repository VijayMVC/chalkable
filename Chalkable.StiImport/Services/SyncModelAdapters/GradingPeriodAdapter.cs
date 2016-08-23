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

        private Data.School.Model.GradingPeriod Selector(GradingPeriod x)
        {
            return new Data.School.Model.GradingPeriod
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
            };
        }

        protected override void InsertInternal(IList<GradingPeriod> entities)
        {
            var gPeriods = entities.Select(Selector).ToList();
            ServiceLocatorSchool.GradingPeriodService.Add(gPeriods);
        }

        protected override void UpdateInternal(IList<GradingPeriod> entities)
        {
            var gps = entities.Select(Selector).ToList();
            ServiceLocatorSchool.GradingPeriodService.Edit(gps);
        }

        protected override void DeleteInternal(IList<GradingPeriod> entities)
        {
            var ids = entities.Select(x => x.GradingPeriodID).ToList();
            ServiceLocatorSchool.GradingPeriodService.Delete(ids);
        }

        protected override void PrepareToDeleteInternal(IList<GradingPeriod> entities)
        {
            throw new System.NotImplementedException();
        }
    }
}