using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Chalkable.BusinessLogic.Mapping.ModelMappers;
using Chalkable.BusinessLogic.Model;
using Chalkable.BusinessLogic.Model.PanoramaSettings;
using Chalkable.BusinessLogic.Model.StudentPanorama;
using Chalkable.BusinessLogic.Security;
using Chalkable.Common;
using Chalkable.Common.Exceptions;
using Chalkable.Data.Common;
using Chalkable.Data.School.DataAccess;
using Chalkable.Data.School.DataAccess.AnnouncementsDataAccess;
using Chalkable.Data.School.Model;
using Chalkable.Data.School.Model.Announcements;
using Chalkable.StiConnector.Connectors.Model;

namespace Chalkable.BusinessLogic.Services.School
{
    public interface IStudentService
    {
        void AddStudents(IList<Student> students);
        void EditStudents(IList<Student> students);
        void DeleteStudents(IList<Student> students);

        void AddStudentSchools(IList<StudentSchool> studentSchools);
        void EditStudentSchools(IList<StudentSchool> studentSchools);
        void DeleteStudentSchools(IList<StudentSchool> studentSchools);

        IList<Student> GetTeacherStudents(int teacherId, int schoolYearId);
        bool IsTeacherStudent(int teacherId, int studentId, int schoolYearId);
        IList<Student> GetClassStudents(int classId, int? markingPeriodId, bool? isEnrolled = null);
        PaginatedList<Student> SearchStudents(int schoolYearId, int? classId, int? teacherId, int? classmatesToId, string filter, bool orderByFirstName, int start, int count, int? markingPeriodId, bool enrolledOnly = false);
        Student GetById(int id, int schoolYearId);
        int GetEnrolledStudentsCount();

        IList<StudentHealthCondition> GetStudentHealthConditions(int studentId);
        StudentSummaryInfo GetStudentSummaryInfo(int studentId, int schoolYearId);
        StudentExplorerInfo GetStudentExplorerInfo(int studentId, int schoolYearId);
        Task<StudentPanoramaInfo> Panorama(int studentId, IList<int> schoolYearIds, IList<StandardizedTestFilter> standardizedTestFilters);

        StudentDetailsInfo GetStudentDetailsInfo(int studentId, int syId);
        IList<StudentDetailsInfo> GetClassStudentsDetails(int classId, bool? isEnrolled = null);
        void PrepareToDelete(IList<Student> students);
        void PrepareToDeleteStudentSchools(IList<StudentSchool> studentSchools);
    }

    public class StudentService : SisConnectedService, IStudentService
    {
        public StudentService(IServiceLocatorSchool serviceLocator) : base(serviceLocator)
        {
        }

        public void AddStudents(IList<Student> students)
        {
            BaseSecurity.EnsureSysAdmin(Context);
            DoUpdate(u => new StudentDataAccess(u).Insert(students));
        }

        public void EditStudents(IList<Student> students)
        {
            BaseSecurity.EnsureSysAdmin(Context);
            DoUpdate(u => new StudentDataAccess(u).Update(students));
        }

        public void DeleteStudents(IList<Student> students)
        {
            BaseSecurity.EnsureSysAdmin(Context);
            DoUpdate(u => new StudentDataAccess(u).Delete(students));
        }

        public void AddStudentSchools(IList<StudentSchool> studentSchools)
        {
            BaseSecurity.EnsureSysAdmin(Context);
            DoUpdate(u => new DataAccessBase<StudentSchool>(u).Insert(studentSchools));
        }
        public void EditStudentSchools(IList<StudentSchool> studentSchools)
        {
            BaseSecurity.EnsureSysAdmin(Context);
            DoUpdate(u => new DataAccessBase<StudentSchool>(u).Update(studentSchools));
        }
        public void DeleteStudentSchools(IList<StudentSchool> studentSchools)
        {
            BaseSecurity.EnsureSysAdmin(Context);
            DoUpdate(u => new DataAccessBase<StudentSchool>(u).Delete(studentSchools));
        }

        public Student GetById(int id, int schoolYearId)
        {
            using (var uow = Read())
            {
                var da = new StudentDataAccess(uow);
                return da.GetById(id, schoolYearId);
            }
        }

        public IList<Student> GetTeacherStudents(int teacherId, int schoolYearId)
        {
            return DoRead(u => new StudentDataAccess(u).GetTeacherStudents(teacherId, schoolYearId));
        }

        public bool IsTeacherStudent(int teacherId, int studentId, int schoolYearId)
        {
            var students = GetTeacherStudents(teacherId, schoolYearId);
            return students.Any(s => s.Id == studentId);
        }

        public IList<Student> GetClassStudents(int classId, int? markingPeriodId, bool? isEnrolled = null)
        {
            return DoRead(u => new StudentDataAccess(u).GetStudents(classId, markingPeriodId, isEnrolled));
        }

        public PaginatedList<Student> SearchStudents(int schoolYearId, int? classId, int? teacherId, int? classmatesToId, string filter, bool orderByFirstName, int start, int count, int? markingPeriodId, bool enrolledOnly = false)
        {
            return DoRead( u => new StudentDataAccess(u).SearchStudents(schoolYearId, classId, teacherId, classmatesToId, filter,
                            orderByFirstName, start, count, markingPeriodId, enrolledOnly));
        }

        public StudentSummaryInfo GetStudentSummaryInfo(int studentId, int schoolYearId)
        {
            Trace.Assert(Context.SchoolLocalId.HasValue);
            Trace.Assert(Context.PersonId.HasValue);
            
            var nowDashboard = ConnectorLocator.StudentConnector.GetStudentNowDashboard(schoolYearId, studentId, Context.NowSchoolTime);
            var student = ServiceLocator.StudentService.GetById(studentId, schoolYearId);
            var infractions = ServiceLocator.InfractionService.GetInfractions();
            var activitiesIds = nowDashboard.Scores.GroupBy(x => x.ActivityId).Select(x => x.Key).ToList();
            var anns = DoRead(uow => new ClassAnnouncementForTeacherDataAccess(uow, schoolYearId).GetByActivitiesIds(activitiesIds, Context.PersonId.Value));
            anns = anns.Where(x => x.ClassAnnouncementData.VisibleForStudent).ToList();
            var res = StudentSummaryInfo.Create(student, nowDashboard, infractions, anns, MapperFactory.GetMapper<StudentAnnouncement, Score>());
            return res;
        }

        public IList<StudentHealthCondition> GetStudentHealthConditions(int studentId)
        {
            if (!CanGetHealthConditions())
                return new List<StudentHealthCondition>();

            var healthConditions = ConnectorLocator.StudentConnector.GetStudentConditions(studentId);

            if (healthConditions == null)
                return new List<StudentHealthCondition>();

            var result = (from studentCondition in healthConditions
                where studentCondition != null
                select new StudentHealthCondition
                {
                    Id = studentCondition.Id,
                    Name = studentCondition.Name,
                    Description = studentCondition.Description,
                    IsAlert = studentCondition.IsAlert,
                    MedicationType = studentCondition.MedicationType,
                    Treatment = studentCondition.Treatment
                }).ToList();
            return result;
        }

        private bool CanGetHealthConditions()
        {
            return Context.Role != CoreRoles.STUDENT_ROLE
                   && (Context.Claims.HasPermission(ClaimInfo.VIEW_HEALTH_CONDITION)
                       || Context.Claims.HasPermission(ClaimInfo.VIEW_MEDICAL));
        }


        public StudentExplorerInfo GetStudentExplorerInfo(int studentId, int schoolYearId)
        {
            Trace.Assert(Context.SchoolLocalId.HasValue);
            Trace.Assert(Context.PersonId.HasValue);
            Trace.Assert(Context.SchoolYearId.HasValue);
            var date = Context.NowSchoolYearTime;
            var student = GetById(studentId, schoolYearId);
            var classes = ServiceLocator.ClassService.GetStudentClasses(schoolYearId, studentId).ToList();
            var classPersons = ServiceLocator.ClassService.GetClassPersons(studentId, true);
            classes = classes.Where(c => classPersons.Any(cp => cp.ClassRef == c.Id)).ToList();
            if (Context.Role == CoreRoles.TEACHER_ROLE)
            {
                var classTeachers = ServiceLocator.ClassService.GetClassTeachers(null, Context.PersonId.Value);
                classes = classes.Where(c => classTeachers.Any(ct => ct.ClassRef == c.Id)).ToList();
            }
            var inowStExpolorer = ConnectorLocator.StudentConnector.GetStudentExplorerDashboard(schoolYearId, student.Id, date);
            var standards = ServiceLocator.StandardService.GetStandards(null, null, null);
            IList<int> importanActivitiesIds = new List<int>();
            IList<AnnouncementComplex> announcements = new List<AnnouncementComplex>();
            IList<StandardScore> standardScores = new List<StandardScore>();
            IList<StudentAverage> mostRecentAverages = new List<StudentAverage>();
            if (inowStExpolorer != null)
            {
                mostRecentAverages = inowStExpolorer.Averages.Where(x => x.IsGradingPeriodAverage && (x.HasGrade)).OrderBy(x => x.GradingPeriodId).ToList();
                standardScores = inowStExpolorer.Standards.ToList();
                standards = standards.Where(s => s.IsActive || standardScores.Any(ss => ss.StandardId == s.Id && ss.HasScore)).ToList();

                if (inowStExpolorer.Activities != null && inowStExpolorer.Activities.Any())
                {
                    foreach (var classDetailse in classes)
                    {
                        var activity = inowStExpolorer.Activities.Where(x => x.SectionId == classDetailse.Id)
                                                    .OrderByDescending(x => x.MaxWeight)
                                                    .FirstOrDefault();
                        if (activity == null) continue;
                        importanActivitiesIds.Add(activity.Id);
                    }
                    announcements = DoRead(uow => new ClassAnnouncementForTeacherDataAccess(uow, Context.SchoolYearId.Value).GetByActivitiesIds(importanActivitiesIds, Context.PersonId.Value));
                }
            }
            return StudentExplorerInfo.Create(student, classes, mostRecentAverages, standardScores, announcements, standards);
        }

        public int GetEnrolledStudentsCount()
        {
            return DoRead(u => new StudentDataAccess(u).GetEnrolledStudentsCount());
        }

        public async Task<StudentPanoramaInfo> Panorama(int studentId, IList<int> schoolYearIds, IList<StandardizedTestFilter> standardizedTestFilters)
        {
            BaseSecurity.EnsureAdminOrTeacher(Context);

            if (!Context.Claims.HasPermission(ClaimInfo.VIEW_PANORAMA))
                throw new ChalkableSecurityException("You are not allowed to view class panorama");

            if (schoolYearIds == null || schoolYearIds.Count == 0)
                throw new ChalkableException("School years is required parameter");

            standardizedTestFilters = standardizedTestFilters ?? new List<StandardizedTestFilter>();
            var componentIds = standardizedTestFilters.Select(x => x.ComponentId);
            var scoreTypeIds = standardizedTestFilters.Select(x => x.ScoreTypeId);

            var tasks = new List<Task<IList<Date>>>();
            foreach (var syId in schoolYearIds)
            {
                tasks.Add(Task.Run(() => ServiceLocator.CalendarDateService.GetLastDays(syId, true, null, Context.NowSchoolYearTime)));
            }
            var studentPanorama = ConnectorLocator.PanoramaConnector.GetStudentPanorama(studentId, schoolYearIds, componentIds.ToList(), scoreTypeIds.ToList());

            var days = (await Task.WhenAll(tasks)).SelectMany(x => x).OrderBy(x=>x.Day).ToList();
            return StudentPanoramaInfo.Create(studentPanorama, days);
        }

        public StudentDetailsInfo GetStudentDetailsInfo(int studentId, int syId)
        {
            var sy = ServiceLocator.SchoolYearService.GetSchoolYearById(syId);
            using (var uow = Read())
            {
                var studentDetails = new StudentDataAccess(uow).GetDetailsById(studentId, syId);
                Ethnicity ethnicity = null;
                Country country = null;
                Language language = null;
                Person counselor = null;

                if(studentDetails.PrimaryPersonEthnicity != null)
                    ethnicity = new DataAccessBase<Ethnicity, int>(uow).GetById(studentDetails.PrimaryPersonEthnicity.EthnicityRef);

                if(studentDetails.PrimaryPersonLanguage != null)
                    language = new DataAccessBase<Language, int>(uow).GetById(studentDetails.PrimaryPersonLanguage.LanguageRef);

                if (studentDetails.PrimaryPersonNationality != null)
                    country = new DataAccessBase<Country, int>(uow).GetById(studentDetails.PrimaryPersonNationality.CountryRef);

                var studentSchool = studentDetails.StudentSchools.FirstOrDefault(x => x.SchoolRef == sy.SchoolRef);
                if (studentSchool?.CounselorRef != null)
                    counselor = new PersonDataAccess(uow).GetById(studentSchool.CounselorRef.Value);
                
                return StudentDetailsInfo.Create(studentDetails, ethnicity, language, country, counselor, studentSchool);
            }
        }

        public IList<StudentDetailsInfo> GetClassStudentsDetails(int classId, bool? isEnrolled = null)
        {
            Trace.Assert(Context.SchoolLocalId.HasValue);

            using (var uow = Read())
            {
                var studentsDetails = new StudentDataAccess(uow).GetDetailsByClass(classId, isEnrolled);
                var ethnicitiesIds = studentsDetails
                    .Where(x => x.PrimaryPersonEthnicity != null)
                    .Select(x => x.PrimaryPersonEthnicity.EthnicityRef).ToList();
                var ethnicities = new DataAccessBase<Ethnicity, int>(uow).GetByIds(ethnicitiesIds);

                var languagesIds = studentsDetails
                    .Where(x => x.PrimaryPersonLanguage != null)
                    .Select(x => x.PrimaryPersonLanguage.LanguageRef).ToList();
                var languages = new DataAccessBase<Language, int>(uow).GetByIds(languagesIds);

                var countriesIds = studentsDetails
                    .Where(x => x.PrimaryPersonNationality != null)
                    .Select(x => x.PrimaryPersonNationality.CountryRef).ToList();
                var countries = new DataAccessBase<Country, int>(uow).GetByIds(countriesIds);

                IList<int> counselorsIds = new List<int>();

                foreach (var student in studentsDetails)
                {
                    var currentStudentSchool = student.StudentSchools.FirstOrDefault(x => x.SchoolRef == Context.SchoolLocalId);
                    if (currentStudentSchool?.CounselorRef != null)
                        counselorsIds.Add(currentStudentSchool.CounselorRef.Value);
                }
                var counselors = new PersonDataAccess(uow).GetByIds(counselorsIds);
                
                return StudentDetailsInfo.Create(studentsDetails, ethnicities, languages, countries, counselors, Context.SchoolLocalId.Value);
            }
        }

        public void PrepareToDelete(IList<Student> students)
        {
            DoUpdate(u => new StudentDataAccess(u).PrepareToDelete(students));
        }

        public void PrepareToDeleteStudentSchools(IList<StudentSchool> studentSchools)
        {
            DoUpdate(u => new DataAccessBase<StudentSchool>(u).PrepareToDelete(studentSchools));
        }
    }
}
