using System;
using System.Collections.Generic;
using System.Linq;
using Chalkable.BusinessLogic.Common;
using Chalkable.BusinessLogic.Model;
using Chalkable.BusinessLogic.Model.ClassPanorama;
using Chalkable.Data.School.Model;
using Chalkable.Web.Models.ClassesViewData;
using Chalkable.Web.Models.DisciplinesViewData;
using Chalkable.Web.Models.PersonViewDatas;
using Chalkable.Web.Models.Settings;

namespace Chalkable.Web.Models.PanoramaViewDatas
{
    public class ClassPanoramaViewData : ClassViewData
    {
        public ClassProfilePanoramaSettingsViewData FilterSettings { get; set; }
        public IList<StandardizedTestViewData> StandardizedTests { get; set; }     
        public ClassDistributionSectionViewData ClassDistributionSection { get; set; } 
        public IList<StandardizedTestStatsViewData> StandardizedTestsStatsByClass { get; set; }
        public IList<StandardizedTestStatsViewData> SelectStandardizedTestsStats { get; set; }
        public IList<StudentStandardizedTestStats> Students { get; set; }

        protected ClassPanoramaViewData(ClassDetails cClass) : base(cClass)
        {
        }
        
        public static ClassPanoramaViewData Create(ClassDetails cClass, IList<StandardizedTestDetails> standardizedTests, ClassPanorama panorama, 
            IList<GradingScaleRange> gradingScaleRanges, IList<StudentDetailsInfo> classStudents, IList<int> selectedStudents, DateTime today)
        {
            var res = new ClassPanoramaViewData(cClass)
            {
                StandardizedTests = standardizedTests.Select(x => StandardizedTestViewData.Create(x, x.Components, x.ScoreTypes)).ToList(),
                ClassDistributionSection = ClassDistributionSectionViewData.Create(panorama.Grades, panorama.Absences, panorama.Infractions, gradingScaleRanges),
                StandardizedTestsStatsByClass = StandardizedTestStatsViewData.CreateForClass(panorama.StandardizedTests, standardizedTests),
                Students = new List<StudentStandardizedTestStats>()
            };

            classStudents = classStudents.OrderBy(x => x.FullName()).ToList();

            //Preparing students
            foreach (var student in classStudents)
            {
                var studentStats = StandardizedTestStatsViewData.CreateForStudent(student.Id, panorama.StandardizedTests, standardizedTests);
                var gradeAvg = panorama.Grades?.FirstOrDefault(x => x.StudentId == student.Id)?.AverageGrade;
                var infractions = panorama.Infractions?.FirstOrDefault(x => x.StudentId == student.Id)?.NumberOfInfractions;
                var absences = panorama.Absences?.FirstOrDefault(x => x.StudentId == student.Id);

                res.Students.Add(StudentStandardizedTestStats.Create(student, gradeAvg, absences, infractions, today, studentStats));
            }

            if (selectedStudents == null || selectedStudents.Count == 0)
                return res;

            var selected = panorama.StandardizedTests?.Where(x => selectedStudents.Contains(x.StudentId));
            res.SelectStandardizedTestsStats = StandardizedTestStatsViewData.CreateForClass(selected?.ToList(), standardizedTests);

            return res;
        }
    }

    public class StandardizedTestStatsViewData
    {
        public ShortStandardizedTestViewData StandardizedTest { get; set; }
        public StandardizedTestComponentViewData Component { get; set; }
        public StandardizedTestScoreTypeViewData ScoreType { get; set; }
        public IList<DailyStatsViewData> DailyStats { get; set; }

        public static IList<StandardizedTestStatsViewData> CreateForClass(IList<StudentStandardizedTestInfo> models, IList<StandardizedTestDetails> standardizedTests)
        {
            var res = new List<StandardizedTestStatsViewData>();

            if (models == null)
                return res;

            models = models.Where(x =>
            {
                decimal a;
                return decimal.TryParse(x.Score, out a);
            }).OrderBy(x => x.Date).ToList();

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
                    ScoreType = StandardizedTestScoreTypeViewData.Create(scoreType),
                    DailyStats = new List<DailyStatsViewData>()
                };

                var studentStTestsInfos = models
                    .Where(x => x.StandardizedTestId == standardizedTestInfo.StandardizedTestId
                                && x.StandardizedTestComponentId == standardizedTestInfo.StandardizedTestComponentId)
                    .GroupBy(y => y.Date, x => decimal.Parse(x.Score));

                foreach (var studentStTestsInfo in studentStTestsInfos)
                {
                    var avg = !studentStTestsInfo.Any() ? 0 : decimal.Round(studentStTestsInfo.Average(), 2);
                    viewData.DailyStats.Add(DailyStatsViewData.Create(studentStTestsInfo.Key, avg, "MMM yyyy"));
                }
                    

                res.Add(viewData);
            }

            return res;
        }

        public static IList<StandardizedTestStatsViewData> CreateForStudent(int studentId, IList<StudentStandardizedTestInfo> models,
            IList<StandardizedTestDetails> standardizedTests)
        {
            var res = new List<StandardizedTestStatsViewData>();
            if (models == null || models.Count == 0)
                return res;

            var studentTests = models.Where(x => x.StudentId == studentId).ToList();
            //We can draw plots only when we have numeric scores
            studentTests = studentTests.Where(x =>
            {
                decimal a;
                return decimal.TryParse(x.Score, out a);
            }).OrderBy(x => x.Date).ToList();

            if (studentTests.Count == 0)
                return res;

            studentTests = studentTests.OrderBy(x => x.Date).ToList();

            foreach (var test in studentTests)
            {
                var currentTest = res.FirstOrDefault(x => x.StandardizedTest.Id == test.StandardizedTestId
                                                       && x.Component.Id == test.StandardizedTestComponentId);

                if (currentTest != null)
                    continue;

                var standardizedTest = standardizedTests.FirstOrDefault(x => x.Id == test.StandardizedTestId);
                var component = standardizedTest?.Components?.FirstOrDefault(x => x.Id == test.StandardizedTestComponentId);
                var scoreType = standardizedTest?.ScoreTypes?.FirstOrDefault(x => x.Id == test.StandardizedTestScoreTypeId);

                var stats = new StandardizedTestStatsViewData
                {
                    Component = component == null ? null : StandardizedTestComponentViewData.Create(component),
                    StandardizedTest = standardizedTest == null ? null : ShortStandardizedTestViewData.Create(standardizedTest),
                    ScoreType = scoreType == null ? null : StandardizedTestScoreTypeViewData.Create(scoreType),
                    DailyStats = new List<DailyStatsViewData>()
                };

                var studentStTestsInfos = models
                    .Where(x => x.StandardizedTestId == test.StandardizedTestId
                                && x.StandardizedTestComponentId == test.StandardizedTestComponentId)
                    .GroupBy(y => y.Date, x => decimal.Parse(x.Score));

                foreach (var studentStTestsInfo in studentStTestsInfos)
                {
                    var avg = !studentStTestsInfo.Any() ? 0 : decimal.Round(studentStTestsInfo.Average(), 2);
                    stats.DailyStats.Add(DailyStatsViewData.Create(studentStTestsInfo.Key, avg, "MMM yyyy"));
                }

                res.Add(stats);
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
                ClassAvg = models.Count == 0 ? 0 : models.Average(x => x.AverageGrade),
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
                    Summary = $"{lo}-{hi}",
                    StudentIds = models.Where(x => x.AverageGrade <= hi && x.AverageGrade >= lo)
                                       .Select(x => x.StudentId)
                                       .ToList()
                });
            }

            return res;
        }
        private static ClassDistributionStatsViewData CreateAbsencesViewData(IList<ShortStudentAbsenceInfo> models)
        {
            var res = new ClassDistributionStatsViewData();
            var absencePersents = models.Where(x => x.NumberOfDaysEnrolled != 0)
                .Select(x => new { Persent = (int) decimal.Round(x.NumberOfAbsences/x.NumberOfDaysEnrolled*100), x.StudentId})
                .OrderBy(x => x.Persent).ToList();
            
            var maxPersent = absencePersents.Max(x => x.Persent);

            res.DistributionStats = new List<DistributionItemViewData>();
            for (var currentPersent = 0; currentPersent <= maxPersent; ++currentPersent)
            {
                res.DistributionStats.Add(new DistributionItemViewData
                {
                    StartInterval = currentPersent,
                    EndInterval = currentPersent,
                    Count = absencePersents.Count(x => x.Persent == currentPersent),
                    Summary = $"{currentPersent}%",
                    StudentIds = absencePersents.Where( x => x.Persent == currentPersent)
                                                .Select(x => x.StudentId)
                                                .ToList()
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
                Summary = $"{0}",
                StudentIds = models.Where(x => x.NumberOfInfractions == 0).Select(x => x.StudentId).ToList()
            });
            for (var i = 1; i <= maxInfractionCount; i += 3)
            {
                res.DistributionStats.Add(new DistributionItemViewData
                {
                    Count = models.Count(x => x.NumberOfInfractions >= i && x.NumberOfInfractions < i+3),
                    StartInterval = i,
                    EndInterval = i + 2,
                    Summary = $"{i}-{i+2}",
                    StudentIds = models.Where(x => x.NumberOfInfractions >= i && x.NumberOfInfractions < i + 3)
                                       .Select(x => x.StudentId)
                                       .ToList()
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
                res.GradeAverageDistribution = CreateGradeAvgViewData(avgInfos, gradingScaleRanges);
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
        public IList<int> StudentIds { get; set; }
        public decimal Count { get; set; }
        public string Summary { get; set; }
        public decimal StartInterval { get; set; }
        public decimal EndInterval { get; set; }
    }

    public class StudentStandardizedTestStats
    {
        public StudentDetailsViewData Student { get; set; }
        public IList<StandardizedTestStatsViewData> StandardizedTestsStats { get; set; }

        public static StudentStandardizedTestStats Create(StudentDetailsInfo student, decimal? gradeAvg, ShortStudentAbsenceInfo absences, int? infractions, 
            DateTime currentSchoolTime, IList<StandardizedTestStatsViewData> standardizedTestStats)
        {
            return new StudentStandardizedTestStats
            {
                Student = StudentDetailsViewData.Create(student, gradeAvg, absences, infractions, currentSchoolTime),
                StandardizedTestsStats = standardizedTestStats
            };
        }
    }
}