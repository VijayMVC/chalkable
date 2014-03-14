using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chalkable.BusinessLogic.Model;
using Chalkable.Common;
using Chalkable.Data.School.DataAccess;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.School
{
    public interface  IGradingStandardService
    {
        IList<GradingStandardInfo> GetGradingStandards(int classId);
        GradingStandardInfo SetGrade(int studentId, int standardId, int classId, int? alphaGradeId, string note);
    }

    public class GradingStandardService : SisConnectedService, IGradingStandardService
    {
        public GradingStandardService(IServiceLocatorSchool serviceLocator) : base(serviceLocator)
        {
        }

        public IList<GradingStandardInfo> GetGradingStandards(int classId)
        {
            var standardScores = ConnectorLocator.StandardScoreConnector.GetStandardScores(classId, null, null);
            var students = ServiceLocator.PersonService.GetPaginatedPersons(new PersonQuery
                            {
                                ClassId = classId,
                                RoleId = CoreRoles.STUDENT_ROLE.Id
                            });
            var standards = ServiceLocator.StandardService.GetStandardes(classId, null, null);
            var res = new List<GradingStandardInfo>();
            foreach (var standardScore in standardScores)
            {
                var student = students.First(x => x.Id == standardScore.StudentId);
                var standard = standards.First(x => x.Id == standardScore.StandardId);
                res.Add(GradingStandardInfo.Create(standardScore, standard, student));
            }
            return res;
        }

        public GradingStandardInfo SetGrade(int studentId, int standardId, int classId, int? alphaGradeId, string note)
        {
            throw new NotImplementedException();
        }
    }



}
