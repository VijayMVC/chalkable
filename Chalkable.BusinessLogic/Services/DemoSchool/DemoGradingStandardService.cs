using System;
using System.Collections.Generic;
using System.Linq;
using Chalkable.BusinessLogic.Model;
using Chalkable.BusinessLogic.Services.DemoSchool.Storage;
using Chalkable.BusinessLogic.Services.School;
using Chalkable.StiConnector.Connectors.Model;

namespace Chalkable.BusinessLogic.Services.DemoSchool
{
   
    public class DemoGradingStandardService : DemoSchoolServiceBase, IGradingStandardService
    {
        public DemoGradingStandardService(IServiceLocatorSchool serviceLocator, DemoStorage storage)
            : base(serviceLocator, storage)
        {
        }

        public IList<GradingStandardInfo> GetGradingStandards(int classId)
        {
            var standardScores = Storage.StiStandardScoreStorage.GetStandardScores(classId, null, null);
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
            var standardScore = new StandardScore
                {
                    SectionId = classId,
                    StudentId = studentId,
                    StandardId = standardId,
                    Note = note,
                    EnteredScoreAlphaGradeId = alphaGradeId,
                    GradingPeriodId = gradingPeriodId
                };
            standardScore = Storage.StiStandardScoreStorage.Update(classId, studentId, standardId, gradingPeriodId, standardScore);
            var standard = ServiceLocator.StandardService.GetStandardById(standardId);
            return GradingStandardInfo.Create(standardScore, standard);
        }
    }



}
