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

        protected override void InsertInternal(IList<AlternateScore> entities)
        {
            var alternateScores = entities.Select(x => new Data.School.Model.AlternateScore
            {
                Id = x.AlternateScoreID,
                Description = x.Description,
                IncludeInAverage = x.IncludeInAverage,
                Name = x.Name,
                PercentOfMaximumScore = x.PercentOfMaximumScore
            }).ToList();
            ServiceLocatorSchool.AlternateScoreService.AddAlternateScores(alternateScores);
        }

        protected override void UpdateInternal(IList<AlternateScore> entities)
        {
            var alternateScores = entities.Select(x => new Data.School.Model.AlternateScore
            {
                Id = x.AlternateScoreID,
                Description = x.Description,
                IncludeInAverage = x.IncludeInAverage,
                Name = x.Name,
                PercentOfMaximumScore = x.PercentOfMaximumScore
            }).ToList();
            ServiceLocatorSchool.AlternateScoreService.EditAlternateScores(alternateScores);
        }

        protected override void DeleteInternal(IList<AlternateScore> entities)
        {
            var alternateScores = entities.Select(x => new Data.School.Model.AlternateScore { Id = x.AlternateScoreID }).ToList();
            ServiceLocatorSchool.AlternateScoreService.Delete(alternateScores);
        }
    }
}