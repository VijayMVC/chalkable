using System.Collections.Generic;
using System.Linq;
using Chalkable.BusinessLogic.Services.School;
using Chalkable.Data.School.Model;
using Chalkable.Web.Models.ClassesViewData;
using Chalkable.Web.Models.DisciplinesViewData;
using Chalkable.Web.Models.Settings;

namespace Chalkable.Web.Models
{
    public class ClassPanoramaViewData: ShortClassViewData
    {
        public ClassProfilePanoramaSettingsViewData FilterSettings { get; set; }
        public IList<StandardizedTestViewData> StandardizedTests { get; set; }     
        public ClassDistributionSectionViewData ClassDistributionSection { get; set; } 
        

        protected ClassPanoramaViewData(Class cClass) : base(cClass)
        {
        }
        
        public static ClassPanoramaViewData Create(Class cClass, ClassProfilePanoramaSettings filterSettings, IList<StandardizedTestDetails> standardizedTests)
        {
            return new ClassPanoramaViewData(cClass)
            {
                FilterSettings = filterSettings != null ? ClassProfilePanoramaSettingsViewData.Create(filterSettings) : null,
                StandardizedTests = standardizedTests.Select(x=>StandardizedTestViewData.Create(x, x.Components, x.ScoreTypes)).ToList()
            };
        }

    }

    public class StandardizedTestStatsViewData
    {
        public int StandardizedTestId { get; set; }
        public string StandardizedTestName { get; set; }
        public IList<DailyStatsViewData> DailyStats { get; set; } 
    }


    public class ClassDistributionSectionViewData
    {
        public ClassDistributionStatsViewData GradeAverageDistribution { get; set; }
        public ClassDistributionStatsViewData AbsencesDistribution { get; set; }
        public ClassDistributionStatsViewData DisciplineDistribution { get; set; }
    }

    public class ClassDistributionStatsViewData
    {
        public decimal ClassAvg { get; set; }
        public IList<DistributionItemViewData> DistributionStats { get; set; }
    }

    public class DistributionItemViewData
    {
        public decimal Count { get; set; }
        public string Summary { get; set; }
        public decimal StartInterval { get; set; }
        public decimal EndInterval { get; set; }
    }
}