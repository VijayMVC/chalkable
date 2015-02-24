using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chalkable.BusinessLogic.Security;
using Chalkable.BusinessLogic.Services.Master;
using Chalkable.Common;
using Chalkable.Common.Exceptions;
using Chalkable.Data.Common;
using Chalkable.Data.Common.Orm;
using Chalkable.Data.Master.Model;
using Chalkable.Data.School.DataAccess;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.School
{
    public interface IPracticeGradeService
    {
        PracticeGrade Add(int standardId, int studentId, Guid applicationId, string score);
        IList<PracticeGrade> GetPracticeGrades(int studentId, int? standardId);
    }
    public class PracticeGradeService : SchoolServiceBase, IPracticeGradeService
    {
        public PracticeGradeService(IServiceLocatorSchool serviceLocator) : base(serviceLocator)
        {
        }

        public PracticeGrade Add(int standardId, int studentId, Guid applicationId, string score)
        {
            //TODO: add security 
            if(!(Context.Role == CoreRoles.TEACHER_ROLE || Context.PersonId == studentId))
                throw new ChalkableSecurityException();
            if (!HasInstalledApp(applicationId, studentId))
                throw new ChalkableSecurityException("Current studented has no installed app");

            var classes = ServiceLocator.ClassService.GetClasses(Context.SchoolYearId, null, studentId);
            using (var uow = Update())
            {
                var classStandards = new ClassStandardDataAccess(uow).GetAll(new AndQueryCondition
                    {
                        {ClassStandard.STANDARD_REF_FIELD, standardId}
                    });
                if(!classes.Any(c=> classStandards.Any(cs=>cs.ClassRef == c.Id || cs.ClassRef == c.CourseRef)))
                    throw new ChalkableSecurityException();

                var da = new PracticeGradeDataAccess(uow);
                var date = Context.NowSchoolYearTime;
                da.Insert(new PracticeGrade
                {
                    Score = score,
                    StandardId = standardId,
                    StudentId = studentId,
                    ApplicationRef = applicationId,
                    Date = date
                });
                uow.Commit();
                var res = da.GetAll(new AndQueryCondition
                    {
                        {PracticeGrade.STANDARD_ID_FIELD, standardId},
                        {PracticeGrade.STUDENT_ID_FIELD, studentId},
                        {PracticeGrade.APPLICATION_REF_FIELD, applicationId},
                        {PracticeGrade.DATE_FIELD, date},
                    });
                return res.Last();
            }
        }

        
        private bool HasInstalledApp(Guid applicationId, int studentId)
        {
            var practiceAppId = Guid.Parse(PreferenceService.Get(Preference.PRACTICE_APPLICATION_ID).Value);
            return practiceAppId == applicationId
                   || ServiceLocator.AppMarketService.GetInstallationForPerson(applicationId, studentId) != null;
        }

        public IList<PracticeGrade> GetPracticeGrades(int studentId, int? standardId)
        {
            return DoRead(uow => new PracticeGradeDataAccess(uow)
                .GetAll(new AndQueryCondition
                    {
                        {PracticeGrade.STUDENT_ID_FIELD, studentId}
                    }));
        }
    }
}
