using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chalkable.BusinessLogic.Model;
using Chalkable.BusinessLogic.Security;
using Chalkable.BusinessLogic.Services.Master;
using Chalkable.Common;
using Chalkable.Common.Exceptions;
using Chalkable.Data.Common;
using Chalkable.Data.Common.Orm;
using Chalkable.Data.Master.Model;
using Chalkable.Data.School.DataAccess;
using Chalkable.Data.School.Model;
using Chalkable.StiConnector.Connectors;

namespace Chalkable.BusinessLogic.Services.School
{
    public interface IPracticeGradeService
    {
        PracticeGrade Add(int standardId, int studentId, Guid applicationId, string score);
        IList<PracticeGrade> GetPracticeGrades(int studentId, int? standardId);
        IList<PracticeGradeDetailedInfo> GetPracticeGradesDetails(int classId, int studentId, int? standardId);
    }
    public class PracticeGradeService : SisConnectedService, IPracticeGradeService
    {
        public PracticeGradeService(IServiceLocatorSchool serviceLocator) : base(serviceLocator)
        {
        }

        public PracticeGrade Add(int standardId, int studentId, Guid applicationId, string score)
        {
            //TODO: add security 
            if(Context.PersonId != studentId)
                throw new ChalkableSecurityException();
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
    
        public IList<PracticeGrade> GetPracticeGrades(int studentId, int? standardId)
        {
            return DoRead(uow => new PracticeGradeDataAccess(uow)
                .GetAll(new AndQueryCondition
                    {
                        {PracticeGrade.STUDENT_ID_FIELD, studentId}
                    }));
        }

        public IList<PracticeGradeDetailedInfo> GetPracticeGradesDetails(int classId, int studentId, int? standardId)
        {
            var standards = ServiceLocator.StandardService.GetStandardsDetails(classId, null, null);
            if (standardId.HasValue)
                standards = standards.Where(x => x.Standard.Id == standardId).ToList();
            var practiceGrades = GetPracticeGrades(studentId, standardId);
            var sy = ServiceLocator.SchoolYearService.GetCurrentSchoolYear();
            var standardsScores = ConnectorLocator.StudentConnector.GetStudentExplorerDashboard(sy.Id, studentId, Context.NowSchoolTime)
                .Standards.ToList();
            var res = new List<PracticeGradeDetailedInfo>();
            foreach (var standard in standards)
            {
                var standardScore = standardsScores.FirstOrDefault(x => x.StandardId == standard.Standard.Id && x.SectionId == classId);
                var pGrades = practiceGrades.Where(x => x.StandardId == standard.Standard.Id).ToList();
                if (pGrades.Count > 0)
                {
                    res.AddRange(pGrades.Select(pg => PracticeGradeDetailedInfo.Create(pg, standard, standardScore)));
                }
                else res.Add(PracticeGradeDetailedInfo.Create(null, standard, standardScore));

            }
            return res;
        }
    }
}
