using System.Collections.Generic;
using System.Linq;
using Chalkable.BusinessLogic.Model.PanoramaSettings;
using Chalkable.BusinessLogic.Model.PanoramaStuff;
using Chalkable.Data.School.Model;
using Chalkable.Web.Models.ClassesViewData;
using Chalkable.Web.Models.DisciplinesViewData;
using Chalkable.Web.Models.Settings;

namespace Chalkable.Web.Models
{
    public class ClassPanoramaViewData : ClassViewData
    {
        public ClassProfilePanoramaSettingsViewData FilterSettings { get; set; }
        public IList<StandardizedTestViewData> StandardizedTests { get; set; }     
        public ClassDistributionSectionViewData ClassDistributionSection { get; set; } 
        public IList<StandardizedTestStatsViewData> StandardizedTestsStatsByClass { get; set; }
        public IList<StandardizedTestStatsViewData> SelectStandardizedTestsStats { get; set; }


        protected ClassPanoramaViewData(ClassDetails cClass) : base(cClass)
        {
        }
        
        public static ClassPanoramaViewData Create(ClassDetails cClass, ClassProfilePanoramaSetting filterSetting, IList<StandardizedTestDetails> standardizedTests, 
            ClassPanorama panorama, IList<GradingScaleRange> gradingScaleRanges, IList<int> selectedStudents)
        {
            var res = new ClassPanoramaViewData(cClass)
            {
                FilterSettings = filterSetting != null ? ClassProfilePanoramaSettingsViewData.Create(filterSetting) : null,
                StandardizedTests = standardizedTests.Select(x=>StandardizedTestViewData.Create(x, x.Components, x.ScoreTypes)).ToList(),
                ClassDistributionSection = ClassDistributionSectionViewData.Create(panorama.Grades, panorama.Absences, panorama.Infractions, gradingScaleRanges),
                StandardizedTestsStatsByClass = StandardizedTestStatsViewData.Create(panorama.StandardizedTests, standardizedTests)
            };

            var selected = panorama.StandardizedTests.Where(x => selectedStudents.Contains(x.StudentId));
            res.SelectStandardizedTestsStats = StandardizedTestStatsViewData.Create(selected.ToList(), standardizedTests);

            return res;
        }

    }

    public class StandardizedTestStatsViewData
    {
        public ShortStandardizedTestViewData StandardizedTest { get; set; }
        public StandardizedTestComponentViewData Component { get; set; }
        public StandardizedTestScoreTypeViewData ScoreType { get; set; }
        public IList<DailyStatsViewData> DailyStats { get; set; }

        public static IList<StandardizedTestStatsViewData> Create(IList<StudentStandardizedTestInfo> models, IList<StandardizedTestDetails> standardizedTests)
        {
            var res = new List<StandardizedTestStatsViewData>();

            if (models == null)
                return res;

            foreach (var standardizedTestInfo in models)
            {
                var test = res.FirstOrDefault(x => x.StandardizedTest.Id == standardizedTestInfo.StandardizedTestId
                                                   && x.Component.Id == standardizedTestInfo.StandardizedTestComponentId);

                if (test != null)
                    continue;

                var stTest = standardizedTests.First(x => x.Id == standardizedTestInfo.StandardizedTestId);
                var component = stTest.Components.First(x => x.Id == standardizedTestInfo.StandardizedTestComponentId);
                var scoreType = stTest.ScoreTypes.First(x => x.Id == standardizedTestInfo.StandardizedTestScoreTypeId);

                var viewData = new StandardizedTestStatsViewData
                {
                    StandardizedTest = ShortStandardizedTestViewData.Create(stTest),
                    Component = StandardizedTestComponentViewData.Create(component),
                    ScoreType = StandardizedTestScoreTypeViewData.Create(scoreType)
                };

                var studentStTestsInfos = models
                    .Where(x => x.StandardizedTestId == standardizedTestInfo.StandardizedTestId
                                && x.StandardizedTestComponentId == standardizedTestInfo.StandardizedTestComponentId)
                    .GroupBy(y => y.Date, x => x.Score);

                foreach (var studentStTestsInfo in studentStTestsInfos)
                    viewData.DailyStats.Add(DailyStatsViewData.Create(studentStTestsInfo.Key, studentStTestsInfo.Average(x => x), "MMM yyyy"));

                res.Add(viewData);
            }

            return res;
        }
    }


    public class ClassDistributionSectionViewData
    {
        public ClassDistributionStatsViewData GradeAverageDistribution { get; set; }
        public ClassDistributionStatsViewData AbsencesDistribution { get; set; }
        public ClassDistributionStatsViewData DisciplineDistribution { get; set; }

        private static ClassDistributionStatsViewData CreateGradeAvgViewData(IList<StudentAverageGradeInfo> models, IList<GradingScaleRange> gradingScaleRanges)
        {
            var res = new ClassDistributionStatsViewData
            {
                ClassAvg = models.Average(x => x.AverageGrade),
                DistributionStats = new List<DistributionItemViewData>()
            };

            gradingScaleRanges = gradingScaleRanges.OrderBy(x => x.LowValue).ToList();

            foreach (var gradingScaleRange in gradingScaleRanges)
            {
                var lo = (int) decimal.Round(gradingScaleRange.LowValue);
                var hi = (int) decimal.Round(gradingScaleRange.HighValue);
                res.DistributionStats.Add(new DistributionItemViewData
                {
                    Count = models.Count(x => x.AverageGrade <= hi && x.AverageGrade >= lo),
                    StartInterval = lo,
                    EndInterval = hi,
                    Summary = $"{lo}-{hi}"
                });
            }

            return res;
        }
        private static ClassDistributionStatsViewData CreateAbsencesViewData(IList<ShortStudentAbsenceInfo> models)
        {
            var res = new ClassDistributionStatsViewData();
            var absencePersents = models.Where(x => x.NumberOfDaysEnrolled != 0)
                .Select(x => (int) decimal.Round(x.NumberOfAbsences/x.NumberOfDaysEnrolled*100))
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

        public static ClassDistributionSectionViewData Create(IList<StudentAverageGradeInfo> avgInfos, IList<ShortStudentAbsenceInfo> absenceInfos,
            IList<ShortStudentInfractionsInfo> infractionInfos, IList<GradingScaleRange> gradingScaleRanges)
        {
            var res = new ClassDistributionSectionViewData();
            if (avgInfos != null)
                res.AbsencesDistribution = CreateGradeAvgViewData(avgInfos, gradingScaleRanges);
            if (absenceInfos != null)
                res.AbsencesDistribution = CreateAbsencesViewData(absenceInfos);
            if (infractionInfos != null)
                res.DisciplineDistribution = CreateInfractionViewData(infractionInfos);

            return res;
        }
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