using System.Collections.Generic;
using System.Linq;
using Chalkable.StiConnector.SyncModel;

namespace Chalkable.StiImport.Services.SyncModelAdapters
{
    public class AlternateScoreAdapter : SyncModelAdapter<AlternateScore>
    {
        public AlternateScoreAdapter(AdapterLocator locator) : base(locator)
        {
        }
        private Data.School.Model.AlternateScore Selector(AlternateScore x)
        {
            return new Data.School.Model.AlternateScore
            {
                Id = x.AlternateScoreID,
                Description = x.Description,
                IncludeInAverage = x.IncludeInAverage,
                Name = x.Name,
                PercentOfMaximumScore = x.PercentOfMaximumScore
            };
        }

        protected override void InsertInternal(IList<AlternateScore> entities)
        {
            var alternateScores = entities.Select(Selector).ToList();
            ServiceLocatorSchool.AlternateScoreService.AddAlternateScores(alternateScores);
        }
        
        protected override void UpdateInternal(IList<AlternateScore> entities)
        {
            var alternateScores = entities.Select(Selector).ToList();
            ServiceLocatorSchool.AlternateScoreService.EditAlternateScores(alternateScores);
        }

        protected override void DeleteInternal(IList<AlternateScore> entities)
        {
            var alternateScores = entities.Select(x => new Data.School.Model.AlternateScore { Id = x.AlternateScoreID }).ToList();
            ServiceLocatorSchool.AlternateScoreService.Delete(alternateScores);
        }
    }
}