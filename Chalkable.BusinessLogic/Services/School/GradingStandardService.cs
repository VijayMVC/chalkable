using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chalkable.BusinessLogic.Model;
using Chalkable.Common;
using Chalkable.Data.School.DataAccess;
using Chalkable.Data.School.Model;
using Chalkable.StiConnector.Connectors.Model;

namespace Chalkable.BusinessLogic.Services.School
{
    public interface  IGradingStandardService
    {
        IList<GradingStandardInfo> GetGradingStandards(int classId);
        GradingStandardInfo SetGrade(int studentId, int standardId, int classId, int gradingPeriodId, int? alphaGradeId, string note);
        
    }

    public class GradingStandardService : SisConnectedService, IGradingStandardService
    {
        public GradingStandardService(IServiceLocatorSchool serviceLocator) : base(serviceLocator)
        {
        }

        public IList<GradingStandardInfo> GetGradingStandards(int classId)
        {
            var standardScores = ConnectorLocator.StandardScoreConnector.GetStandardScores(classId, null, null);
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
            var res = ConnectorLocator.StandardScoreConnector.Update(classId, studentId, standardId, gradingPeriodId, standardScore);
            var standard = ServiceLocator.StandardService.GetStandardById(standardId);
            return GradingStandardInfo.Create(res, standard);
        }
    }



}
