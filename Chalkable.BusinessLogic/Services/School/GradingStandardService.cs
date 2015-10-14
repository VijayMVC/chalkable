using System.Collections.Generic;
using System.Threading.Tasks;
using Chalkable.BusinessLogic.Model;
using Chalkable.BusinessLogic.Security;
using Chalkable.StiConnector.Connectors.Model;

namespace Chalkable.BusinessLogic.Services.School
{
    public interface  IGradingStandardService
    {
        Task<IList<GradingStandardInfo>> GetGradingStandards(int classId, int? gradingPeriodId, bool reCalculateStandards = true); 
        GradingStandardInfo SetGrade(int studentId, int standardId, int classId, int gradingPeriodId, int? alphaGradeId, string note);  
    }

    public class GradingStandardService : SisConnectedService, IGradingStandardService
    {
        public GradingStandardService(IServiceLocatorSchool serviceLocator) : base(serviceLocator)
        {
        }
        public async Task<IList<GradingStandardInfo>> GetGradingStandards(int classId, int? gradingPeriodId, bool reCalculateStandards = true)
        {
            if (reCalculateStandards && GradebookSecurity.CanReCalculateGradebook(Context)) 
                await ConnectorLocator.GradebookConnector.Calculate(classId);
            var standardScores = ConnectorLocator.StandardScoreConnector.GetStandardScores(classId, null, gradingPeriodId);
            var standards = ServiceLocator.StandardService.GetStandards(classId, null, null);
            return GradingStandardInfo.Create(standardScores, standards);
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
