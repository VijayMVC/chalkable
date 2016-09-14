using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Chalkable.BusinessLogic.Mapping.ModelMappers;
using Chalkable.BusinessLogic.Model;
using Chalkable.BusinessLogic.Model.PanoramaSettings;
using Chalkable.BusinessLogic.Model.StudentPanorama;
using Chalkable.BusinessLogic.Services.School;
using Chalkable.Common;
using Chalkable.Data.School.Model;
using Chalkable.Data.School.Model.Announcements;
using Chalkable.StiConnector.Connectors.Model;
using Chalkable.StiConnector.Connectors.Model.Attendances;

namespace Chalkable.BusinessLogic.Services.DemoSchool
{
    public class DemoStudentStorage : BaseDemoIntStorage<Student>
    {
        public DemoStudentStorage()
            : base(x => x.Id, false)
        {
        }
    }
    public class DemoStudentSchoolStorage : BaseDemoIntStorage<StudentSchool>
    {
        public DemoStudentSchoolStorage()
            : base(null, true)
        {
        }
    }

    public class DemoStudentHealthConditionStorage : BaseDemoIntStorage<StudentHealthCondition>
    {
        public DemoStudentHealthConditionStorage()
            : base(x => x.Id)
        {
        }

        public IList<StudentHealthCondition> GetStudentHealthConditions(int studentId)
        {
            return data.Select(x => x.Value).ToList();
        }
    }

    public class DemoStudentService : DemoSchoolServiceBase, IStudentService
    {
        private DemoStudentStorage StudentStorage { get; set; }
        private DemoStudentHealthConditionStorage StudentHealthConditionStorage { get; set; }
        private DemoStudentSchoolStorage StudentSchoolStorage { get; set; }
        
        public DemoStudentService(IServiceLocatorSchool serviceLocator) : base(serviceLocator)
        {
            StudentStorage = new DemoStudentStorage();
            StudentHealthConditionStorage = new DemoStudentHealthConditionStorage();
            StudentSchoolStorage = new DemoStudentSchoolStorage();

        }

        public void AddStudents(IList<Student> students)
        {
            StudentStorage.Add(students);
        }

        public void EditStudents(IList<Student> students)
        {
            StudentStorage.Update(students);
        }

        public void DeleteStudents(IList<Student> students)
        {
            StudentStorage.Delete(students);
        }

        public void AddStudentSchools(IList<StudentSchool> studentSchools)
        {
            StudentSchoolStorage.Add(studentSchools);
        }

        public void EditStudentSchools(IList<StudentSchool> studentSchools)
        {
            StudentSchoolStorage.Update(studentSchools);
        }

        public void DeleteStudentSchools(IList<StudentSchool> studentSchools)
        {
            StudentSchoolStorage.Delete(studentSchools);
        }

        public PaginatedList<Student> SearchStudents(int schoolYearId, int? classId, int? schoolId, int? gradeLevel, int? programId,
            int? teacherId, int? classmatesToId, string filter, bool orderByFirstName, int start, int count,
            int? markingPeriodId, bool enrolledOnly = false)
        {
            throw new NotImplementedException();
        }

        public Student GetById(int id, int schoolYearId)
        {
            var student = StudentStorage.GetById(id);
            var isEnrolled = ServiceLocator.SchoolYearService.GetStudentAssignments().Any(x => x.StudentRef == id
                                          && x.SchoolYearRef == schoolYearId && x.IsEnrolled);
            return BuildStudentDetailsData(student, !isEnrolled);
        }

        private IList<Student> PrepareStudentListDetailsData(IEnumerable<Student> students, IDictionary<int, bool> stWithDrawDic)
        {
            return students.Select(s => BuildStudentDetailsData(s, stWithDrawDic.ContainsKey(s.Id) ? stWithDrawDic[s.Id] : default(bool?))).ToList();
        }

        public static Student BuildStudentDetailsData(Student student, bool? isWithdrawn)
        {
            return new Student
            {
                Id = student.Id,
                FirstName = student.FirstName,
                LastName = student.LastName,
                BirthDate = student.BirthDate,
                Gender = student.Gender,
                HasMedicalAlert = student.HasMedicalAlert,
                IsAllowedInetAccess = student.IsAllowedInetAccess,
                PhotoModifiedDate = student.PhotoModifiedDate,
                SpEdStatus = student.SpEdStatus,
                SpecialInstructions = student.SpecialInstructions,
                IsWithdrawn = isWithdrawn
            };
        }

        public bool IsTeacherStudent(int teacherId, int studentId, int schoolYearId)
        {
            throw new NotImplementedException();
        }

        public IList<Student> GetClassStudents(int classId, int? markingPeriodId, bool? isEnrolled = null)
        {
            var students = StudentStorage.GetAll();
            var stWithDrawDictionary = ServiceLocator.ClassService.GetClassPersons(null, classId, isEnrolled, markingPeriodId)
                .GroupBy(x => x.PersonRef).ToDictionary(x => x.Key, x => !x.First().IsEnrolled);
            students = students.Where(s => stWithDrawDictionary.ContainsKey(s.Id)).ToList();
            return PrepareStudentListDetailsData(students, stWithDrawDictionary);
        }

        public PaginatedList<Student> SearchStudents(int schoolYearId, int? classId, int? teacherId, int? classmatesToId, string filter, bool orderByFirstName, int start, int count, int? markingPeriod, bool enrolledOnly)
        {
            var students = StudentStorage.GetAll().AsEnumerable();
            if (!string.IsNullOrEmpty(filter))
            {
                var words = filter.ToLowerInvariant().Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                students = students.Where(s => words.Any(w => s.FirstName.ToLower().Contains(w) || s.LastName.ToLower().Contains(w)));
            }
            var classTeachers = ServiceLocator.ClassService.GetClassTeachers(classId, teacherId);
            var markingPeriods = ServiceLocator.MarkingPeriodService.GetMarkingPeriods(schoolYearId);
            var stSchoolYears = ((DemoSchoolYearService)ServiceLocator.SchoolYearService).GetStudentAssignments(schoolYearId, null);
            var classPersons = ((DemoClassService)ServiceLocator.ClassService).GetClassPersons(classId);
            classPersons = classPersons.Where(x => classTeachers.Any(y => y.ClassRef == x.ClassRef)).ToList();
            classPersons = classPersons.Where(x => markingPeriods.Any(y => y.Id == x.MarkingPeriodRef)).ToList();

            students = students.Where(x => classPersons.Any(y => y.PersonRef == x.Id));
            students = students.Where(x => stSchoolYears.Any(y => y.StudentRef == x.Id));

            students = orderByFirstName ? students.OrderBy(s => s.FirstName) : students.OrderBy(s => s.LastName);
            var res = students.ToList();
            IDictionary<int, bool> stWithDrawDic = new Dictionary<int, bool>();
            foreach (var student in res)
            {
                var isEnrolled = stSchoolYears.Any(x => x.StudentRef == student.Id && x.IsEnrolled)
                                && classPersons.Any(x => x.PersonRef == student.Id && x.IsEnrolled);
                if (!stWithDrawDic.ContainsKey(student.Id))
                    stWithDrawDic.Add(student.Id, !isEnrolled);
                else stWithDrawDic[student.Id] = !isEnrolled;
            }
            return new PaginatedList<Student>(PrepareStudentListDetailsData(res, stWithDrawDic), start / count, count);
        }
        
        public IList<Student> GetTeacherStudents(int teacherId, int schoolYearId)
        {
            var students = StudentStorage.GetAll();
            var markingPeriods = ServiceLocator.MarkingPeriodService.GetMarkingPeriods(schoolYearId);
            var classPersons = ((DemoClassService)ServiceLocator.ClassService).GetClassPersonsByMarkingPeriods(markingPeriods);

            students = students.Where(s => classPersons.Any(cp => cp.PersonRef == s.Id)).ToList();
            var stWithDrawDic = ((DemoSchoolYearService)ServiceLocator.SchoolYearService).GetStudentAssignments(schoolYearId, null)
                                            .GroupBy(x => x.StudentRef).ToDictionary(x => x.Key, x => !x.First().IsEnrolled);

            return PrepareStudentListDetailsData(students, stWithDrawDic);
        }

        public async Task<IList<StudentHealthCondition>> GetStudentHealthConditions(int studentId)
        {
            await Task.Delay(0);
            return StudentHealthConditionStorage.GetStudentHealthConditions(studentId);
        }

        public Task<IList<StudentHealthFormInfo>> GetStudentHealthForms(int studentId, int schoolYearId)
        {
            throw new NotImplementedException();
        }

        public byte[] DownloadStudentHealthFormDocument(int studentId, int healthFormId)
        {
            throw new NotImplementedException();
        }

        public Task VerifyStudentHealthForm(int studentId, int healthFormId)
        {
            throw new NotImplementedException();
        }

        public async Task<StudentSummaryInfo> GetStudentSummaryInfo(int studentId, int schoolYearId)
        {
            await Task.Delay(0);
            throw new NotImplementedException();
        }

        public Student GetStudentDetails(int studentId, int schoolYearId)
        {
            var student = StudentStorage.GetById(studentId);
            var isEnrolled = ((DemoSchoolYearService)ServiceLocator.SchoolYearService).IsStudentEnrolled(studentId, schoolYearId);
            return BuildStudentDetailsData(student, !isEnrolled);
        }

        public StudentSummaryInfo GetStudentSummaryInfo(int studentId)
        {
            var classRank = new ClassRank
            {
                StudentId = studentId,
                ClassSize = 10
            };

            var syId = Context.SchoolYearId ?? ServiceLocator.SchoolYearService.GetCurrentSchoolYear().Id;

            var discipline = ((DemoDisciplineService)ServiceLocator.DisciplineService).GetList(DateTime.Today);

            var infractions = new List<StiConnector.Connectors.Model.Infraction>();

            foreach (var disciplineReferral in discipline.Where(disciplineReferral => disciplineReferral.Infractions != null))
            {
                infractions.AddRange(disciplineReferral.Infractions);
            }

            var chlkInfractions = ServiceLocator.InfractionService.GetInfractions();

            var infractionSummaries = from infr in infractions
                                      group infr by infr.Id
                                          into g
                                          select new { Id = g.Key, Count = g.Count() };

            var attendances =
                ((DemoAttendanceService) ServiceLocator.AttendanceService).GetStudentAbsenceSummary(studentId);

            var infractionSummary = infractionSummaries.Select(x => new InfractionSummary
            {
                StudentId = studentId,
                InfractionId = x.Id,
                Occurrences = x.Count
            }).ToList();

            var nowDashboard = new NowDashboard
            {
                ClassRank = classRank,
                Infractions = infractionSummary,
                SectionAttendance = new List<StudentSectionAbsenceSummary>(), //attendances,
                Scores = ((DemoStudentAnnouncementService)ServiceLocator.StudentAnnouncementService).GetActivityScoresForStudent(studentId)
            };
            var student = GetStudentDetails(studentId, syId);
            var activitiesids = nowDashboard.Scores.GroupBy(x => x.ActivityId).Select(x => x.Key).ToList();

            //TODO: impl
            var anns = new List<AnnouncementComplex>(); //((DemoAnnouncementService) ServiceLocator.ClassAnnouncementService).GetByActivitiesIds(activitiesids);
            var res = StudentSummaryInfo.Create(student, nowDashboard, chlkInfractions, anns, MapperFactory.GetMapper<StudentAnnouncement, Score>());
            return res;
        }

        public async Task<StudentExplorerInfo> GetStudentExplorerInfo(int studentId, int schoolYearId)
        {
            await Task.Delay(0);
            Trace.Assert(Context.SchoolLocalId.HasValue);
            var student = GetById(studentId, schoolYearId);
            var classes = ServiceLocator.ClassService.GetStudentClasses(schoolYearId, studentId).ToList();
            var classPersons = ServiceLocator.ClassService.GetClassPersons(studentId, true);
            classes = classes.Where(c => classPersons.Any(cp => cp.ClassRef == c.Id)).ToList();
            var inowStExpolorer = new StudentExplorerDashboard
            {
                Averages = new List<StudentAverage>(),
                Standards = new Stack<StandardScore>(),
                Activities = new List<Activity>()
            };
            var standards = ServiceLocator.StandardService.GetStandards(null, null, null);
            IList<int> importantActivitiesIds = new List<int>();
            IList<AnnouncementComplex> announcements = new List<AnnouncementComplex>();
            if (inowStExpolorer.Activities.Any())
            {
                foreach (var details in classes)
                {
                    var activity = inowStExpolorer.Activities.Where(x => x.SectionId == details.Id)
                                                .OrderByDescending(x => x.MaxScore * x.WeightMultiplier + x.WeightAddition).FirstOrDefault();
                    if (activity == null) continue;
                    importantActivitiesIds.Add(activity.Id);
                }
                //TODO : impl later
                //announcements = ((DemoAnnouncementService)ServiceLocator.AnnouncementService).GetTeacherAnnouncementService()
                //    .GetByActivitiesIds(importantActivitiesIds);
            }
            return StudentExplorerInfo.Create(student, classes, inowStExpolorer.Averages.ToList()
                , inowStExpolorer.Standards.ToList(), announcements, standards);
            
        }

        public int GetEnrolledStudentsCount()
        {
            throw new NotImplementedException();
        }

        public Task<StudentPanoramaInfo> Panorama(int classId, IList<int> schoolYearIds, IList<StandardizedTestFilter> standardizedTestFilters)
        {
            throw new NotImplementedException();
        }

        public StudentDetailsInfo GetStudentDetailsInfo(int studentId, int syId)
        {
            throw new NotImplementedException();
        }

        public IList<StudentDetailsInfo> GetClassStudentsDetails(int classId, bool? isEnrolled = null)
        {
            throw new NotImplementedException();
        }

        public void PrepareToDelete(IList<Student> students)
        {
            throw new NotImplementedException();
        }

        public void PrepareToDeleteStudentSchools(IList<StudentSchool> studentSchools)
        {
            throw new NotImplementedException();
        }
    }
}
