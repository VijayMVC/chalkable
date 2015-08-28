using System.Collections.Generic;
using System.Linq;
using Chalkable.StiConnector.SyncModel;

namespace Chalkable.StiImport.Services.SyncModelAdapters
{
    public class GradedItemAdapter : SyncModelAdapter<GradedItem>
    {
        public GradedItemAdapter(AdapterLocator locator) : base(locator)
        {
        }

        protected override void InsertInternal(IList<GradedItem> entities)
        {
            var res = entities.Select(x => new Data.School.Model.GradedItem
            {
                Id = x.GradedItemID,
                GradingPeriodRef = x.GradingPeriodID,
                AllowExemption = x.AllowExemption,
                AlphaOnly = x.AlphaOnly,
                AppearsOnReportCard = x.AppearsOnReportCard,
                AveragingRule = x.AveragingRule,
                Description = x.Description,
                Name = x.Name,
                DetGradeCredit = x.DetGradCredit,
                DetGradePoints = x.DetGradePoints,
            }).ToList();
            ServiceLocatorSchool.GradedItemService.Add(res);
        }

        protected override void UpdateInternal(IList<GradedItem> entities)
        {
            var res = entities.Select(x => new Data.School.Model.GradedItem
            {
                Id = x.GradedItemID,
                GradingPeriodRef = x.GradingPeriodID,
                AllowExemption = x.AllowExemption,
                AlphaOnly = x.AlphaOnly,
                AppearsOnReportCard = x.AppearsOnReportCard,
                AveragingRule = x.AveragingRule,
                Description = x.Description,
                Name = x.Name,
                DetGradeCredit = x.DetGradCredit,
                DetGradePoints = x.DetGradePoints
            }).ToList();
            ServiceLocatorSchool.GradedItemService.Edit(res);
        }

        protected override void DeleteInternal(IList<GradedItem> entities)
        {
            var gradedItems = entities.Select(x => new Data.School.Model.GradedItem { Id = x.GradedItemID }).ToList();
            ServiceLocatorSchool.GradedItemService.Delete(gradedItems);
        }
    }
}