using System.Collections.Generic;
using System.Linq;
using Chalkable.BusinessLogic.Model;
using Chalkable.BusinessLogic.Services.School;
using Chalkable.StiConnector.Connectors.Model;

namespace Chalkable.BusinessLogic.Services.DemoSchool
{
   
    public class DemoGradingStandardService : DemoSchoolServiceBase, IGradingStandardService
    {
        public DemoGradingStandardService(IServiceLocatorSchool serviceLocator)
            : base(serviceLocator)
        {
        }

        public IList<GradingStandardInfo> GetGradingStandards(int classId, int? gradingPeriodId, bool reCalculateStandards = true)
        {
            var standardScores = StorageLocator.StiStandardScoreStorage.GetStandardScores(classId, null, gradingPeriodId);
            var standards = ServiceLocator.StandardService.GetStandards(classId, null, null);
            var res = new List<GradingStandardInfo>();
            foreach (var standardScore in standardScores)
            {
                var standard = standards.First(x => x.Id == standardScore.StandardId);
                res.Add(GradingStandardInfo.Create(standardScore, standard));
            }
            return res;
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
            standardScore = StorageLocator.StiStandardScoreStorage.Update(classId, studentId, standardId, gradingPeriodId, standardScore);
            var standard = ServiceLocator.StandardService.GetStandardById(standardId);

            
            return GradingStandardInfo.Create(standardScore, standard);
        }
    }



}
