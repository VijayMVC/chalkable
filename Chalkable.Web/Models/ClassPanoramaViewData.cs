using System;
using System.Collections.Generic;
using System.Linq;
using Chalkable.BusinessLogic.Model.PanoramaSettings;
using Chalkable.BusinessLogic.Model.PanoramaStuff;
using Chalkable.BusinessLogic.Services.School;
using Chalkable.Data.School.Model;
using Chalkable.Web.Models.ClassesViewData;
using Chalkable.Web.Models.DisciplinesViewData;
using Chalkable.Web.Models.Settings;

namespace Chalkable.Web.Models
{
    public class ClassPanoramaViewData: ClassViewData
    {
        public ClassProfilePanoramaSettingsViewData FilterSettings { get; set; }
        public IList<StandardizedTestViewData> StandardizedTests { get; set; }     
        public ClassDistributionSectionViewData ClassDistributionSection { get; set; } 
        public IList<StandardizedTestStatsViewData> StandardizedTestsStatsByClass { get; set; }
        public IList<StandardizedTestStatsViewData> SelectStandardizedTestsStats { get; set; }


        protected ClassPanoramaViewData(ClassDetails cClass) : base(cClass)
        {
        }
        
        public static ClassPanoramaViewData Create(ClassDetails cClass, ClassProfilePanoramaSetting filterSetting, IList<StandardizedTestDetails> standardizedTests, ClassPanorama panorama)
        {
            return new ClassPanoramaViewData(cClass)
            {
                FilterSettings = filterSetting != null ? ClassProfilePanoramaSettingsViewData.Create(filterSetting) : null,
                StandardizedTests = standardizedTests.Select(x=>StandardizedTestViewData.Create(x, x.Components, x.ScoreTypes)).ToList()
            };
        }

    }

    public class StandardizedTestStatsViewData
    {
        public ShortStandardizedTestViewData StandardizedTest { get; set; }
        public StandardizedTestComponentViewData Component { get; set; }
        public StandardizedTestScoreTypeViewData ScoreType { get; set; }
        public IList<DailyStatsViewData> DailyStats { get; set; } 
    }


    public class ClassDistributionSectionViewData
    {
        public ClassDistributionStatsViewData GradeAverageDistribution { get; set; }
        public ClassDistributionStatsViewData AbsencesDistribution { get; set; }
        public ClassDistributionStatsViewData DisciplineDistribution { get; set; }

        private static ClassDistributionStatsViewData CreateGradeAvgViewData(IList<StudentAverageGradeInfo> models)
        {
            var res = new ClassDistributionStatsViewData();
            res.ClassAvg = models.Average(x => x.AverageGrade);

            return res;
        }

        private static ClassDistributionStatsViewData CreateAbsencesViewData(IList<ShortStudentAbsenceInfo> models)
        {
            var res = new ClassDistributionStatsViewData();
            var absencePersents = models.Where(x => x.NumberOfDaysEnrolled != 0)
                .Select(x => (int) decimal.Round(x.NumberOfAbsences/x.NumberOfDaysEnrolled) * 100)
                .OrderBy(x => x).ToList();
            
            var maxPersent = absencePersents.Max();

            res.DistributionStats = new List<DistributionItemViewData>();
            for (var currentPersent = 0; currentPersent <= maxPersent; ++currentPersent)
            {
                res.DistributionStats.Add(new DistributionItemViewData
                {
                    StartInterval = currentPersent,
                    EndInterval = currentPersent,
                    Count = absencePersents.Count(x => x == currentPersent),
                    Summary = $"{currentPersent}%"
                });
            }

            res.ClassAvg = res.DistributionStats.Average(x => x.Count);

            return res;
        }

        private static ClassDistributionStatsViewData CreateInfractionViewData(IList<ShortStudentInfractionsInfo> models)
        {
            var res = new ClassDistributionStatsViewData
            {
                DistributionStats = new List<DistributionItemViewData>()
            };
            models = models.OrderBy(x => x.NumberOfInfractions).ToList();

            var maxInfractionCount = models.Max(x => x.NumberOfInfractions);
            if(maxInfractionCount % 3 != 0)
                maxInfractionCount += (3 - maxInfractionCount % 3);
            
            res.DistributionStats.Add(new DistributionItemViewData
            {
                Count = models.Count(x => x.NumberOfInfractions == 0),
                StartInterval = 0,
                EndInterval = 0,
                Summary = $"{0}"
            });
            for (var i = 1; i <= maxInfractionCount; i += 3)
            {
                res.DistributionStats.Add(new DistributionItemViewData
                {
                    Count = models.Count(x => x.NumberOfInfractions >= i && x.NumberOfInfractions < i+3),
                    StartInterval = i,
                    EndInterval = i + 2,
                    Summary = $"{i}-{i+2}"
                });
            }

            res.ClassAvg = res.DistributionStats.Average(x => x.Count);

            return res;
        }
    }

    public class ClassDistributionStatsViewData
    {
        public decimal ClassAvg { get; set; }
        public IList<DistributionItemViewData> DistributionStats { get; set; }

        public static ClassDistributionStatsViewData Create()
        {
            return new ClassDistributionStatsViewData();
        }
    }

    public class DistributionItemViewData
    {
        public decimal Count { get; set; }
        public string Summary { get; set; }
        public decimal StartInterval { get; set; }
        public decimal EndInterval { get; set; }
    }
}