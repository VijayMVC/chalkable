using System;
using System.Collections.Generic;
using System.Linq;
using Chalkable.BusinessLogic.Mapping.ModelMappers;
using Chalkable.BusinessLogic.Model;
using Chalkable.BusinessLogic.Services.DemoSchool.Storage;
using Chalkable.BusinessLogic.Services.School;
using Chalkable.Common;
using Chalkable.Data.School.DataAccess.AnnouncementsDataAccess;
using Chalkable.Data.School.Model;
using Chalkable.StiConnector.Connectors.Model;

namespace Chalkable.BusinessLogic.Services.DemoSchool
{
    public class DemoStudentService : DemoSchoolServiceBase, IStudentService
    {
        public DemoStudentService(IServiceLocatorSchool serviceLocator, DemoStorage demoStorage) : base(serviceLocator, demoStorage)
        {
        }

        public void AddStudents(IList<Student> students)
        {
            throw new NotImplementedException();
        }

        public void EditStudents(IList<Student> students)
        {
            throw new NotImplementedException();
        }

        public void DeleteStudents(IList<Student> students)
        {
            throw new NotImplementedException();
        }

        public void AddStudentSchools(IList<StudentSchool> studentSchools)
        {
            throw new NotImplementedException();
        }

        public void EditStudentSchools(IList<StudentSchool> studentSchools)
        {
            throw new NotImplementedException();
        }

        public void DeleteStudentSchools(IList<StudentSchool> studentSchools)
        {
            throw new NotImplementedException();
        }

        public StudentDetails GetById(int id, int schoolYearId)
        {
            return Storage.StudentStorage.GetStudentDeatils(id, schoolYearId);
        }

        public IList<StudentDetails> GetClassStudents(int classId, int markingPeriodId, bool? isEnrolled = null)
        {
            return Storage.StudentStorage.GetClassStudents(classId, markingPeriodId, isEnrolled);

            throw new NotImplementedException();
            /*IList<Person> res = ServiceLocator.PersonService.GetPaginatedPersons(new PersonQuery
            {
                ClassId = classId,
                CallerId = Context.PersonId,
                CallerRoleId = Context.Role.Id,
                Count = int.MaxValue,
                RoleId = CoreRoles.STUDENT_ROLE.Id
            });


            var sy = ServiceLocator.SchoolYearService.GetCurrentSchoolYear();
            var enrollmentStatus = isEnrolled.HasValue && isEnrolled.Value
                                      ? StudentEnrollmentStatusEnum.CurrentlyEnrolled
                                      : (StudentEnrollmentStatusEnum?)null;
            var studentSys = Storage.StudentSchoolYearStorage.GetList(sy.Id, enrollmentStatus);
            res = res.Where(x => studentSys.Any(y => y.StudentRef == x.Id)).ToList();
            if (isEnrolled.HasValue)
            {
                var classPersons = Storage.ClassPersonStorage.GetClassPersons(new ClassPersonQuery { ClassId = classId, IsEnrolled = isEnrolled, MarkingPeriodId = markingPeriodId });
                res = res.Where(x => classPersons.Any(y => y.PersonRef == x.Id)).ToList();
            }

            return res;*/
        }

        public PaginatedList<StudentDetails> SearchStudents(int schoolYearId, int? classId, int? teacherId, int? classmatesToId, string filter, bool orderByFirstName,
            int start, int count)
        {
            return Storage.StudentStorage.SearStudents(schoolYearId, classId, teacherId, filter, orderByFirstName, start, count);
        }
        
        public IList<StudentDetails> GetTeacherStudents(int teacherId, int schoolYearId)
        {
            return Storage.StudentStorage.GetTeacherStudents(teacherId, schoolYearId);
        }

        public IList<StudentHealthCondition> GetStudentHealthConditions(int studentId)
        {
            return Storage.StudentHealthConditionStorage.GetByStudentId(studentId);
        }

        public StudentSummaryInfo GetStudentSummaryInfo(int studentId)
        {


            /*
             * public ClassRank ClassRank { get; set; }
        public string CurrentAttendanceStatus { get; set; }
        public DailyAbsenceSummary DailyAttendance { get; set; }
        public IEnumerable<InfractionSummary> Infractions { get; set; }
        public IEnumerable<Score> Scores { get; set; }
        public IEnumerable<SectionAbsenceSummary> SectionAttendance { get; set; }
             */


            var classRank = new ClassRank
            {
                StudentId = studentId,
                ClassSize = 10
            };

            var syId = Context.SchoolYearId ?? ServiceLocator.SchoolYearService.GetCurrentSchoolYear().Id;

            var discipline = Storage.StiDisciplineStorage.GetList(DateTime.Today);

            var infractions = new List<Chalkable.StiConnector.Connectors.Model.Infraction>();

            foreach (var disciplineReferral in discipline)
            {
                infractions.AddRange(disciplineReferral.Infractions);
            }

            var chlkInfractions = ServiceLocator.InfractionService.GetInfractions();

            var infractionSummaries = from infr in infractions
                                      group infr by infr.Id
                                          into g
                                          select new { Id = g.Key, Count = g.Count() };

            var attendances = Storage.StiAttendanceStorage.GetSectionAttendanceSummary(studentId,
                DateTime.Today.AddDays(-1), DateTime.Today).Select(sectionAttendanceSummary => new SectionAbsenceSummary()
                {
                    SectionId = sectionAttendanceSummary.SectionId,
                    Tardies = sectionAttendanceSummary.Students.Select(x => x.Tardies).Sum(),
                    Absences = sectionAttendanceSummary.Students.Select(x => x.Absences).Sum(),

                }).ToList();

            var infractionSummary = infractionSummaries.Select(x => new InfractionSummary()
            {
                StudentId = studentId,
                InfractionId = x.Id,
                Occurrences = x.Count
            }).ToList();

            var nowDashboard = new NowDashboard
            {
                ClassRank = classRank,
                Infractions = infractionSummary,
                SectionAttendance = attendances,
                Scores = Storage.StiActivityScoreStorage.GetScores(studentId)
            };
            var student = ServiceLocator.StudentService.GetById(studentId, syId);
            var activitiesids = nowDashboard.Scores.GroupBy(x => x.ActivityId).Select(x => x.Key).ToList();
            var anns = Storage.AnnouncementStorage.GetAnnouncements(new AnnouncementsQuery { SisActivitiesIds = activitiesids }).Announcements;
            var res = StudentSummaryInfo.Create(student, nowDashboard, chlkInfractions, anns, MapperFactory.GetMapper<StudentAnnouncement, Score>());
            return res;
        }


        public StudentExplorerInfo GetStudentExplorerInfo(int studentId, int schoolYearId)
        {
            //var date = Context.NowSchoolYearTime;
            var student = GetById(studentId, schoolYearId);
            var classes = ServiceLocator.ClassService.GetClasses(schoolYearId, null, studentId).ToList();
            var classPersons = ServiceLocator.ClassService.GetClassPersons(studentId, true);
            classes = classes.Where(c => classPersons.Any(cp => cp.ClassRef == c.Id)).ToList();
            var inowStExpolorer = new StudentExplorerDashboard
                {
                    Averages = new List<StudentAverage>(),
                    Standards = new Stack<StandardScore>(),
                    Activities = new List<Activity>()
                };
            var standards = ServiceLocator.StandardService.GetStandards(null, null, null);
            IList<int> importanActivitiesIds = new List<int>();
            IList<AnnouncementComplex> announcements = new List<AnnouncementComplex>();
            if (inowStExpolorer.Activities.Any())
            {
                foreach (var classDetailse in classes)
                {
                    var activity = inowStExpolorer.Activities.Where(x => x.SectionId == classDetailse.Id)
                                                .OrderByDescending(x => x.MaxScore * x.WeightMultiplier + x.WeightAddition).FirstOrDefault();
                    if (activity == null) continue;
                    importanActivitiesIds.Add(activity.Id);
                }
                announcements = DoRead(uow => new AnnouncementForTeacherDataAccess(uow, Context.SchoolLocalId).GetByActivitiesIds(importanActivitiesIds));
            }
            return StudentExplorerInfo.Create(student, classes, inowStExpolorer, announcements, standards);
        }
    }
}
