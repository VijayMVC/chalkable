using System.Collections.Generic;
using System.Linq;
using Chalkable.StiConnector.SyncModel;

namespace Chalkable.StiImport.Services.SyncModelAdapters
{
    public class GradingScaleAdapter : SyncModelAdapter<GradingScale>
    {
        public GradingScaleAdapter(AdapterLocator locator) : base(locator)
        {
        }

        private Data.School.Model.GradingScale Selector(GradingScale x)
        {
            return new Data.School.Model.GradingScale
            {
                Id = x.GradingScaleID,
                Description = x.Description,
                HomeGradeToDisplay = x.HomeGradeToDisplay,
                Name = x.Name,
                SchoolRef = x.SchoolID
            };
        }

        protected override void InsertInternal(IList<GradingScale> entities)
        {
            var gs = entities.Select(Selector).ToList();
            ServiceLocatorSchool.GradingScaleService.AddGradingScales(gs);
        }

        protected override void UpdateInternal(IList<GradingScale> entities)
        {
            var gs = entities.Select(Selector).ToList();
            ServiceLocatorSchool.GradingScaleService.EditGradingScales(gs);
        }

        protected override void DeleteInternal(IList<GradingScale> entities)
        {
            var gradingScales = entities.Select(x => new Data.School.Model.GradingScale { Id = x.GradingScaleID }).ToList();
            ServiceLocatorSchool.GradingScaleService.DeleteGradingScales(gradingScales);
        }
    }
}