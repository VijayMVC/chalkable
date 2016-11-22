using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Chalkable.BusinessLogic.Model;
using Chalkable.Common.Exceptions;
using Chalkable.Data.Common;
using Chalkable.Data.Common.Orm;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.School
{
    public interface IPracticeGradeService
    {
        PracticeGrade Add(int standardId, int studentId, Guid applicationId, string score);
        IList<PracticeGrade> GetPracticeGrades(int studentId, int? standardId);
        Task<IList<PracticeGradesDetailedInfo>> GetPracticeGradesDetails(int classId, int studentId, int? standardId);
    }
    public class PracticeGradeService : SisConnectedService, IPracticeGradeService
    {
        public PracticeGradeService(IServiceLocatorSchool serviceLocator) : base(serviceLocator)
        {
        }

        public PracticeGrade Add(int standardId, int studentId, Guid applicationId, string score)
        {
            Trace.Assert(Context.SchoolYearId.HasValue);
            //TODO: add security 
            if(Context.PersonId != studentId)
                throw new ChalkableSecurityException();
            var classes = ServiceLocator.ClassService.GetStudentClasses(Context.SchoolYearId.Value, studentId);
            using (var uow = Update())
            {
                var classStandards = new DataAccessBase<ClassStandard, int>(uow)
                    .GetAll(new AndQueryCondition { {ClassStandard.STANDARD_REF_FIELD, standardId}});

                if(!classes.Any(c=> classStandards.Any(cs=>cs.ClassRef == c.Id || cs.ClassRef == c.CourseRef)))
                    throw new ChalkableSecurityException();

                var da = new DataAccessBase<PracticeGrade>(uow);
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
            return DoRead(uow => new DataAccessBase<PracticeGrade>(uow)
                .GetAll(new AndQueryCondition
                    {
                        {PracticeGrade.STUDENT_ID_FIELD, studentId}
                    }));
        }

        public async Task<IList<PracticeGradesDetailedInfo>> GetPracticeGradesDetails(int classId, int studentId, int? standardId)
        {
            var sy = ServiceLocator.SchoolYearService.GetCurrentSchoolYear();
            var studentExplorerTask = ConnectorLocator.StudentConnector.GetStudentExplorerDashboard(sy.Id, studentId, Context.NowSchoolTime);
            var standards = ServiceLocator.StandardService.GetStandards(classId, null, null);
            if (standardId.HasValue)
                standards = standards.Where(x => x.Id == standardId).ToList();
            var practiceGrades = GetPracticeGrades(studentId, standardId);
            var standardsScores =  (await studentExplorerTask).Standards.ToList();
            var res = new List<PracticeGradesDetailedInfo>();
            foreach (var standard in standards)
            {
                var standardScore = standardsScores.FirstOrDefault(x => x.StandardId == standard.Id && x.SectionId == classId);
                var pGrades = practiceGrades.Where(x => x.StandardId == standard.Id).OrderByDescending(x=>x.Date).ToList();
                res.Add(PracticeGradesDetailedInfo.Create(pGrades, standard, standardScore));
            }
            res = res.OrderBy(x => x)
                     .ThenBy(x=>x.Standard.Name)
                     .ToList();
            return res;
        }
    }
}
