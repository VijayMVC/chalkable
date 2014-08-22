using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Chalkable.Data.School.Model;

namespace Chalkable.Web.Models
{
    public class AlternateScoreViewData
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IncludeInAverage { get; set; }
        public decimal? PercentOfMaximumScore { get; set; }


        public static AlternateScoreViewData Create(AlternateScore alternateScore)
        {
            return new AlternateScoreViewData
                {
                    Id = alternateScore.Id,
                    Name = alternateScore.Name,
                    Description = alternateScore.Description,
                    IncludeInAverage = alternateScore.IncludeInAverage,
                    PercentOfMaximumScore = alternateScore.PercentOfMaximumScore
                };
        }
        public static IList<AlternateScoreViewData> Create(IList<AlternateScore> alternateScores)
        {
            return alternateScores.Select(Create).ToList();
        } 
    }
}