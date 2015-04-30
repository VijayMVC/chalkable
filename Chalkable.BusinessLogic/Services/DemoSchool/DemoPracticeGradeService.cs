using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Chalkable.BusinessLogic.Model;
using Chalkable.BusinessLogic.Services.School;
using Chalkable.Common.Exceptions;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.DemoSchool
{
    public class DemoPracticeGradeStorage : BaseDemoIntStorage<PracticeGrade>
    {
        public DemoPracticeGradeStorage()
            : base(x => x.Id, true)
        {

        }
    }

    public class DemoPracticeGradeService : DemoSchoolServiceBase, IPracticeGradeService
    {
        private DemoPracticeGradeStorage PracticeGradeStorage { get; set; }
        public DemoPracticeGradeService(IServiceLocatorSchool serviceLocator) : base(serviceLocator)
        {
            PracticeGradeStorage = new DemoPracticeGradeStorage();
        }

        public PracticeGrade Add(int standardId, int studentId, Guid applicationId, string score)
        {
            Trace.Assert(Context.SchoolYearId.HasValue);
            if (Context.PersonId != studentId)
                throw new ChalkableSecurityException();
            var classes = ServiceLocator.ClassService.GetStudentClasses(Context.SchoolYearId.Value, studentId);

            var classStandards = ((DemoStandardService) ServiceLocator.StandardService).GetClassStandards(standardId);

            if (!classes.Any(c => classStandards.Any(cs => cs.ClassRef == c.Id || cs.ClassRef == c.CourseRef)))
                throw new ChalkableSecurityException();

            PracticeGradeStorage.Add(new PracticeGrade
            {
                Score = score,
                StandardId = standardId,
                StudentId = studentId,
                ApplicationRef = applicationId,
                Date = Context.NowSchoolYearTime
            });
            var res =
                PracticeGradeStorage.GetAll()
                    .Where(
                        x => x.StudentId == studentId && x.ApplicationRef == applicationId && x.StandardId == standardId);
            return res.Last();
        }

        public IList<PracticeGrade> GetPracticeGrades(int studentId, int? standardId)
        {
            var practiceGrades = PracticeGradeStorage.GetAll().Where(x => x.StudentId == studentId).ToList();
            if (standardId.HasValue)
                practiceGrades = practiceGrades.Where(x => x.StudentId == standardId.Value).ToList();

            return practiceGrades;
        }


        public IList<PracticeGradesDetailedInfo> GetPracticeGradesDetails(int classId, int studentId, int? standardId)
        {

            var standards = ServiceLocator.StandardService.GetStandards(classId, null, null);
            if (standardId.HasValue)
                standards = standards.Where(x => x.Id == standardId).ToList();
            var practiceGrades = GetPracticeGrades(studentId, standardId);
            var standardsScores = ((DemoGradingStandardService)ServiceLocator.GradingStandardService).GetStandardScores(classId, null, null);
            var res = new List<PracticeGradesDetailedInfo>();
            foreach (var standard in standards)
            {
                var standardScore = standardsScores.FirstOrDefault(x => x.StandardId == standard.Id && x.SectionId == classId);
                var pGrades = practiceGrades.Where(x => x.StandardId == standard.Id).OrderByDescending(x=>x.Date).ToList();
                res.Add(PracticeGradesDetailedInfo.Create(pGrades, standard, standardScore));
            }
            return res;
        }
    }
}
