using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Chalkable.BusinessLogic.Model;
using Chalkable.BusinessLogic.Services.School;
using Chalkable.StiConnector.Connectors.Model;

namespace Chalkable.BusinessLogic.Services.DemoSchool
{

    public class DemoStandardScoreStorage : BaseDemoIntStorage<StandardScore>
    {
        public DemoStandardScoreStorage()
            : base(null, true)
        {
        }

        public StandardScore Update(int classId, int studentId, int standardId, int gradingPeriodId, StandardScore standardScore)
        {
            if (data.Any(x => x.Value.StudentId == studentId && x.Value.SectionId == classId &&
                              x.Value.StandardId == standardId && x.Value.GradingPeriodId == gradingPeriodId))
            {
                var item = data.First(x => x.Value.StudentId == studentId && x.Value.SectionId == classId &&
                                           x.Value.StandardId == standardId &&
                                           x.Value.GradingPeriodId == gradingPeriodId).Key;
                data[item] = standardScore;
            }
            return standardScore;
        }
    }

    public class DemoGradingStandardService : DemoSchoolServiceBase, IGradingStandardService
    {
        private DemoStandardScoreStorage StandardScoreStorage { get; set; }
        public DemoGradingStandardService(IServiceLocatorSchool serviceLocator)
            : base(serviceLocator)
        {
            StandardScoreStorage = new DemoStandardScoreStorage();
        }

        public Task<IList<GradingStandardInfo>> GetGradingStandards(int classId, int? gradingPeriodId, bool reCalculateStandards = true)
        {
            var standardScores = GetStandardScores(classId, null, gradingPeriodId);
            var standards = ServiceLocator.StandardService.GetStandards(classId, null, null);
            var res = new List<GradingStandardInfo>();
            foreach (var standardScore in standardScores)
            {
                var standard = standards.FirstOrDefault(x => x.Id == standardScore.StandardId);
                if (standard != null)
                    res.Add(GradingStandardInfo.Create(standardScore, standard));
            }
            return new Task<IList<GradingStandardInfo>>(() => res);
        }

        public GradingStandardInfo SetGrade(int studentId, int standardId, int classId, int gradingPeriodId, int? alphaGradeId, string note)
        {
            var alphaGradeName = alphaGradeId.HasValue ? ((DemoAlphaGradeService)ServiceLocator.AlphaGradeService).GetAlphaGradeById(alphaGradeId.Value).Name : "";
            var standardScore = new StandardScore
                {
                    SectionId = classId,
                    StudentId = studentId,
                    StandardId = standardId,
                    Note = note,
                    EnteredScoreAlphaGradeId = alphaGradeId,
                    ComputedScoreAlphaGradeId = alphaGradeId,
                    EnteredScoreAlphaGradeName = alphaGradeName,
                    ComputedScoreAlphaGradeName = alphaGradeName,
                    GradingPeriodId = gradingPeriodId
                };
            standardScore = StandardScoreStorage.Update(classId, studentId, standardId, gradingPeriodId, standardScore);
            var standard = ServiceLocator.StandardService.GetStandardById(standardId);
            return GradingStandardInfo.Create(standardScore, standard);
        }

        public IEnumerable<StandardScore> GetStandardScores(int sectionId, int? standardId, int? gradingPeriodId)
        {
            var scores = StandardScoreStorage.GetData().Select(x => x.Value);

            scores = scores.Where(x => x.SectionId == sectionId);

            if (standardId.HasValue)
                scores = scores.Where(x => x.StandardId == sectionId);
            if (gradingPeriodId.HasValue)
                scores = scores.Where(x => x.GradingPeriodId == gradingPeriodId);

            return scores.ToList();
        }

        public void AddStandardScore(StandardScore standardScore)
        {
            StandardScoreStorage.Add(standardScore);
        }
    }



}
