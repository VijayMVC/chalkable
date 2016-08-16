using System.Collections.Generic;
using System.Linq;
using Chalkable.StiConnector.SyncModel;

namespace Chalkable.StiImport.Services.SyncModelAdapters
{
    public class GradingScaleRangeAdapter : SyncModelAdapter<GradingScaleRange>
    {
        public GradingScaleRangeAdapter(AdapterLocator locator) : base(locator)
        {
        }

        private Data.School.Model.GradingScaleRange Selector(GradingScaleRange x)
        {
            return new Data.School.Model.GradingScaleRange
            {
                AlphaGradeRef = x.AlphaGradeID,
                AveragingEquivalent = x.AveragingEquivalent,
                AwardGradCredit = x.AwardGradCredit,
                GradingScaleRef = x.GradingScaleID,
                HighValue = x.HighValue,
                IsPassing = x.IsPassing,
                LowValue = x.LowValue
            };
        }

        protected override void InsertInternal(IList<GradingScaleRange> entities)
        {
            var gsr = entities.Select(Selector).ToList();
            ServiceLocatorSchool.GradingScaleService.AddGradingScaleRanges(gsr);
        }

        protected override void UpdateInternal(IList<GradingScaleRange> entities)
        {
            var gsr = entities.Select(Selector).ToList();
            ServiceLocatorSchool.GradingScaleService.EditGradingScaleRanges(gsr);
        }

        protected override void DeleteInternal(IList<GradingScaleRange> entities)
        {
            var gsr = entities.Select(x => new Data.School.Model.GradingScaleRange
            {
                AlphaGradeRef = x.AlphaGradeID,
                GradingScaleRef = x.GradingScaleID
            }).ToList();
            ServiceLocatorSchool.GradingScaleService.DeleteGradingScaleRanges(gsr);
        }

        protected override void PrepareToDeleteInternal(IList<GradingScaleRange> entities)
        {
            throw new System.NotImplementedException();
        }
    }
}