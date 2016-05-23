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

    public class StandardizedTestStats
    {
        public int StandardizedTestId { get; set; }
        public string StandardizedTestName { get; set; }
        public IList<DailyStatsViewData> DailyStats { get; set; } 
    }
}