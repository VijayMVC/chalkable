using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Chalkable.BusinessLogic.Model;
using Chalkable.BusinessLogic.Security;
using Chalkable.Data.School.DataAccess;
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
            var isTeacherClass = DoRead(u => new ClassTeacherDataAccess(u).Exists(classId, Context.PersonId));
            Task<Gradebook> calculateTask = null;
            if (reCalculateStandards && GradebookSecurity.CanReCalculateGradebook(Context, isTeacherClass))
                calculateTask = ConnectorLocator.GradebookConnector.Calculate(classId);
            
            var standards = ServiceLocator.StandardService.GetGridStandardsByPacing(classId, null, null, gradingPeriodId);
            if (calculateTask != null)
                await calculateTask;
            var standardScores = ConnectorLocator.StandardScoreConnector.GetStandardScores(classId, null, gradingPeriodId);
            standards = standards.Where(s => s.IsActive || standardScores.Any(ss => ss.StandardId == s.Id && ss.HasScore)).ToList();
            var res = GradingStandardInfo.Create(standardScores, standards);
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
