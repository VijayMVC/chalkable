using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chalkable.Data.Common;
using Chalkable.Data.Common.Orm;
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
            using (var uow = Update())
            {
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
