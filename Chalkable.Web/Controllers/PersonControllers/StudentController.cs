using System.Diagnostics;
using System.Linq;
using System.Web.Mvc;
using Chalkable.Common;
using Chalkable.Data.Common.Enums;
using Chalkable.Data.School.Model;
using Chalkable.Web.ActionFilters;
using Chalkable.Web.Models.AttendancesViewData;
using Chalkable.Web.Models.DisciplinesViewData;
using Chalkable.Web.Models.PersonViewDatas;

namespace Chalkable.Web.Controllers.PersonControllers
{
    [RequireHttps, TraceControllerFilter]
    public class StudentController : PersonController
    {
        //[AuthorizationFilter("AdminGrade, AdminEdit, AdminView, Teacher, Student", Preference.API_DESCR_STUDENT_SUMMARY, true, CallType.Get, new[] { AppPermissionType.User, AppPermissionType.Attendance, AppPermissionType.Discipline, AppPermissionType.Grade })]
        [AuthorizationFilter("AdminGrade, AdminEdit, AdminView, Teacher, Student")]
        public ActionResult Summary(int schoolPersonId)
        {
            Trace.Assert(Context.PersonId.HasValue);
            Trace.Assert(Context.SchoolYearId.HasValue);

            if (SchoolLocator.Context.PersonId != schoolPersonId &&
                SchoolLocator.Context.Role == CoreRoles.STUDENT_ROLE)
            {
                var student = SchoolLocator.PersonService.GetPerson(schoolPersonId);
                return Json(ShortPersonViewData.Create(student));
            }
            var studentSummaryInfo = SchoolLocator.StudentService.GetStudentSummaryInfo(schoolPersonId);
            var classes = SchoolLocator.ClassService.GetStudentClasses(Context.SchoolYearId.Value, schoolPersonId).ToList();
            var classPersons = SchoolLocator.ClassService.GetClassPersons(schoolPersonId, true);
            classes = classes.Where(x => classPersons.Any(y => y.ClassRef == x.Id)).ToList();
            var schedule = SchoolLocator.ClassPeriodService.GetSchedule(null, studentSummaryInfo.StudentInfo.Id, null,
                Context.NowSchoolYearTime.Date, Context.NowSchoolYearTime.Date);
            var classIdsSortedBySchedule = schedule.OrderBy(si => si.PeriodOrder).Select(si => si.ClassId).Distinct().ToList();

            var classList = classIdsSortedBySchedule.Where(x=>x.HasValue && classes.Any(y=>y.Id == x.Value))
                .Select(sortedClassRef => classes.First(cls => cls.Id == sortedClassRef)).ToList();
            classList.AddRange(classes.Where(cls => !classIdsSortedBySchedule.Contains(cls.Id)));

            Room currentRoom = null;
            ClassDetails currentClass = null;
            if (studentSummaryInfo.CurrentSectionId.HasValue)
            {
                currentClass = classes.FirstOrDefault(x => x.Id == studentSummaryInfo.CurrentSectionId.Value);
                if (currentClass != null && currentClass.RoomRef.HasValue)
                    currentRoom = SchoolLocator.RoomService.GetRoomById(currentClass.RoomRef.Value);

            }
            var stHealsConditions = SchoolLocator.StudentService.GetStudentHealthConditions(schoolPersonId);
            var res = StudentSummaryViewData.Create(studentSummaryInfo, currentRoom, currentClass, classList);
            res.HealthConditions = StudentHealthConditionViewData.Create(stHealsConditions);
            return Json(res);

        }
        
        [AuthorizationFilter("AdminGrade, AdminEdit, AdminView, Teacher, Student", true, new[] { AppPermissionType.User })]
        public ActionResult Info(int personId)
        {
            var syId = GetCurrentSchoolYearId();
            var res = (StudentInfoViewData)GetInfo(personId, personInfo=> StudentInfoViewData.Create(personInfo, syId));
            var stHealsConditions = SchoolLocator.StudentService.GetStudentHealthConditions(personId);
            res.HealthConditions = StudentHealthConditionViewData.Create(stHealsConditions);
            var studentDetailsInfo = SchoolLocator.StudentService.GetById(personId, syId);
            res.HasMedicalAlert = studentDetailsInfo.HasMedicalAlert;
            res.IsAllowedInetAccess = studentDetailsInfo.IsAllowedInetAccess;
            res.SpecialInstructions = studentDetailsInfo.SpecialInstructions;
            res.SpEdStatus = studentDetailsInfo.SpEdStatus;

            //todo : clarify in Zoli ... do we need all contacts or just family members
            var studentContacts = SchoolLocator.ContactService.GetStudentContactDetails(personId);
            res.StudentContacts = StudentContactViewData.Create(studentContacts);
            return Json(res, 6);
        }

        [AuthorizationFilter("AdminGrade, AdminEdit, AdminView, Teacher, Student")]
        public ActionResult Schedule(int personId)
        {
            var student = SchoolLocator.StudentService.GetById(personId, GetCurrentSchoolYearId());
            return Json(PrepareScheduleData(StudentViewData.Create(student)));
        }

        [AuthorizationFilter("AdminGrade, AdminEdit, AdminView, Teacher, Student", true, new[] { AppPermissionType.User })]
        public ActionResult GetStudents(string filter, bool? myStudentsOnly, int? start, int? count, int? classId, bool? byLastName)
        {
            Trace.Assert(Context.SchoolYearId.HasValue);
            int? teacherId = null;
            if (myStudentsOnly == true && CoreRoles.TEACHER_ROLE == SchoolLocator.Context.Role)
                teacherId = SchoolLocator.Context.PersonId;
            int? classMatesToId = null;
            if (CoreRoles.STUDENT_ROLE == SchoolLocator.Context.Role)
                classMatesToId = Context.PersonId;
            var res = SchoolLocator.StudentService.SearchStudents(Context.SchoolYearId.Value, classId, teacherId, classMatesToId, filter, byLastName != true, start ?? 0, count ?? 10);
            return Json(res.Transform(StudentViewData.Create));
        }

        [AuthorizationFilter("AdminGrade, AdminEdit, AdminView, Teacher, Student")]
        public ActionResult Explorer(int personId)
        {
            var syId = GetCurrentSchoolYearId();
            var studentExplorerInfo = SchoolLocator.StudentService.GetStudentExplorerInfo(personId, syId);
            return Json(StudentExplorerViewData.Create(studentExplorerInfo));
        }

        [AuthorizationFilter("AdminGrade, AdminEdit, AdminView, Teacher, Student")]
        public ActionResult AttendanceSummary(int studentId, int? gradingPeriodId)
        {
            var syid = GetCurrentSchoolYearId();
            var gradingPeriods = SchoolLocator.GradingPeriodService.GetGradingPeriodsDetails(syid);
            var gp = gradingPeriodId.HasValue
                         ? SchoolLocator.GradingPeriodService.GetGradingPeriodById(gradingPeriodId.Value)
                         : SchoolLocator.GradingPeriodService.GetGradingPeriodDetails(syid, Context.NowSchoolYearTime.Date);
            var studentSummary = SchoolLocator.AttendanceService.GetStudentAttendanceSummary(studentId, gradingPeriodId);
            return Json(StudentAttendanceSummaryViewData.Create(studentSummary, gp, gradingPeriods));
        }
        [AuthorizationFilter("AdminGrade, AdminEdit, AdminView, Teacher, Student")]
        public ActionResult Apps(int studentId, int? start, int? count)
        {
            var syId = GetCurrentSchoolYearId();
            var student = SchoolLocator.StudentService.GetById(studentId, syId);
            var currentBalance = FundController.GetPersonBalance(MasterLocator, studentId);
            var apps = AppMarketController.GetListInstalledApps(SchoolLocator, MasterLocator, studentId, null, start, count);
            return Json(StudentAppsViewData.Create(student, currentBalance, apps));
        }

        [AuthorizationFilter("AdminGrade, AdminEdit, AdminView, Teacher, Student")]
        public ActionResult DisciplineSummary(int studentId, int? gradingPeriodId)
        {
            var syId = GetCurrentSchoolYearId();
            var student = SchoolLocator.StudentService.GetById(studentId, syId);
            var gradingPeriods = SchoolLocator.GradingPeriodService.GetGradingPeriodsDetails(syId);
            var gp = gradingPeriodId.HasValue
                         ? SchoolLocator.GradingPeriodService.GetGradingPeriodById(gradingPeriodId.Value)
                         : SchoolLocator.GradingPeriodService.GetGradingPeriodDetails(syId, Context.NowSchoolYearTime.Date);
            var infractionSummaries = SchoolLocator.DisciplineService.GetStudentInfractionSummary(studentId, gradingPeriodId);
            var res = StudentDisciplineSummaryViewData.Create(student, infractionSummaries, gp, gradingPeriods);
            return Json(res);
        }
    }
}