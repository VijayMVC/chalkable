using System.Collections.Generic;
using System.Linq;
using Chalkable.Data.School.Model;
using Chalkable.StiConnector.SyncModel;

namespace Chalkable.StiImport.Services.SyncModelAdapters
{
    public class TermAdapter : SyncModelAdapter<Term>
    {
        public TermAdapter(AdapterLocator locator) : base(locator)
        {
        }

        protected override void InsertInternal(IList<Term> entities)
        {
            var mps = entities.Select(term => new MarkingPeriod
            {
                Description = term.Description,
                EndDate = term.EndDate,
                Id = term.TermID,
                Name = term.Name,
                SchoolYearRef = term.AcadSessionID,
                StartDate = term.StartDate,
                WeekDays = 62
            }).ToList();
            ServiceLocatorSchool.MarkingPeriodService.Add(mps);
        }

        protected override void UpdateInternal(IList<Term> entities)
        {
            var mps = entities.Select(x => new MarkingPeriod
            {
                Description = x.Description,
                EndDate = x.EndDate,
                Id = x.TermID,
                Name = x.Name,
                SchoolYearRef = x.AcadSessionID,
                StartDate = x.StartDate,
                WeekDays = 62
            }).ToList();
            ServiceLocatorSchool.MarkingPeriodService.Edit(mps);
        }

        protected override void DeleteInternal(IList<Term> entities)
        {
            var ids = entities.Select(x => new MarkingPeriod { Id = x.TermID }).ToList();
            ServiceLocatorSchool.MarkingPeriodService.DeleteMarkingPeriods(ids);
        }
    }
}