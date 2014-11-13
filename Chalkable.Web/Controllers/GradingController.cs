using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web.Mvc;
using Chalkable.BusinessLogic.Model;
using Chalkable.Common.Exceptions;
using Chalkable.Data.Common.Enums;
using Chalkable.Data.Master.Model;
using Chalkable.Data.School.Model;
using Chalkable.Web.ActionFilters;
using Chalkable.Web.Models;
using Chalkable.Web.Models.AnnouncementsViewData;

namespace Chalkable.Web.Controllers
{
    [RequireHttps, TraceControllerFilter]
    public class GradingController : ChalkableController
    {
        [AuthorizationFilter("Teacher")]
        public ActionResult TeacherSummary(int teacherId)
        {
            var schoolYearId = GetCurrentSchoolYearId();
            var gradingPeriod = SchoolLocator.GradingPeriodService.GetGradingPeriodDetails(schoolYearId, Context.NowSchoolYearTime.Date);
            var classesGradesSummary = SchoolLocator.GradingStatisticService.GetClassesGradesSummary(teacherId, gradingPeriod.Id);
            return Json(GradingTeacherClassSummaryViewData.Create(classesGradesSummary), 6);
        }

        [AuthorizationFilter("Teacher", Preference.API_DESCR_GRADING_CLASS_SUMMARY, true, CallType.Get, new[] { AppPermissionType.Grade, AppPermissionType.Class })]
        public ActionResult ClassSummary(int classId)
        {
            if (!SchoolLocator.Context.PersonId.HasValue)
                throw new UnassignedUserException();
            return Json(PrepareClassGradingBoxes(classId));
        }

        private ClassGradingBoxesViewData PrepareClassGradingBoxes(int classId)
        {
            var syId = GetCurrentSchoolYearId();
            var gradingPeriods = SchoolLocator.GradingPeriodService.GetGradingPeriodsDetails(syId);
            var currentGradingPeriod = SchoolLocator.GradingPeriodService.GetGradingPeriodDetails(syId, Context.NowSchoolYearTime.Date);
            TeacherClassGrading gradingSummary = null;
            if (currentGradingPeriod != null)
            {
                gradingSummary = SchoolLocator.GradingStatisticService.GetClassGradingSummary(classId, currentGradingPeriod);
            }
            return ClassGradingBoxesViewData.Create(gradingPeriods, gradingSummary);
        }

        [AuthorizationFilter("Teacher")]
        public ActionResult ClassGradingPeriodSummary(int classId, int gradingPeriodId)
        {
            var gradingPeriod = SchoolLocator.GradingPeriodService.GetGradingPeriodById(gradingPeriodId);
            var res = SchoolLocator.GradingStatisticService.GetClassGradingSummary(classId, gradingPeriod);
            return Json(GradingClassSummaryViewData.Create(res));
        }

        [AuthorizationFilter("AdminGrade, AdminEdit, AdminView, Teacher, Student", Preference.API_DESCR_GRADING_CLASS_SUMMARY, true, CallType.Get, new[] { AppPermissionType.Grade, AppPermissionType.Class })]
        public ActionResult ItemGradingStat(int announcementId)
        {
            //return FakeJson("~/fakeData/itemGradingStat.json");
            //var ann = SchoolLocator.AnnouncementService.GetAnnouncementById(announcementId);
            var studentAnns = SchoolLocator.StudentAnnouncementService.GetStudentAnnouncements(announcementId);
            return Json(ItemGradigStatViewData.Create(studentAnns, announcementId));
        }


        [AuthorizationFilter("Teacher", Preference.API_DESCR_GRADING_CLASS_SUMMARY_GRID, true, CallType.Get, new[] { AppPermissionType.Grade, AppPermissionType.Class })]
        public ActionResult ClassSummaryGrids(int classId)
        {
            Trace.WriteLine("GetCurrentSchoolYearId " + DateTime.Now.Ticks * 1.0 / TimeSpan.TicksPerSecond);
            var syId = GetCurrentSchoolYearId();
            Trace.WriteLine("GetGradingPeriodsDetails " + DateTime.Now.Ticks * 1.0 / TimeSpan.TicksPerSecond);
            var gradingPeriods = SchoolLocator.GradingPeriodService.GetGradingPeriodsDetails(syId);
            Trace.WriteLine("GetStandards " + DateTime.Now.Ticks * 100.0 / TimeSpan.TicksPerSecond);
            var standards = SchoolLocator.StandardService.GetStandards(classId, null, null);
            Trace.WriteLine("GetClassAnnouncementTypes " + DateTime.Now.Ticks * 1.0 / TimeSpan.TicksPerSecond);
            var classAnnouncementTypes = SchoolLocator.ClassAnnouncementTypeService.GetClassAnnouncementTypes(classId);
            var date = Context.NowSchoolYearTime.Date;
            Trace.WriteLine("gradingPeriods " + DateTime.Now.Ticks * 1.0 / TimeSpan.TicksPerSecond);
            var currentGradingPeriod = gradingPeriods.FirstOrDefault(x => x.StartDate.Date <= date && x.EndDate >= date);
            if (currentGradingPeriod == null)
                currentGradingPeriod = gradingPeriods.Where(x => x.StartDate.Date <= date)
                                  .OrderByDescending(x => x.StartDate).FirstOrDefault();
            Trace.WriteLine("GetGradeBook " + DateTime.Now.Ticks * 1.0 / TimeSpan.TicksPerSecond);
            ChalkableGradeBook gradeBook = null;
            if(currentGradingPeriod != null)
                gradeBook = SchoolLocator.GradingStatisticService.GetGradeBook(classId, currentGradingPeriod);
            Trace.WriteLine("GradingGridsViewData.Create " + DateTime.Now.Ticks * 1.0 / TimeSpan.TicksPerSecond);
            return Json(GradingGridsViewData.Create(gradeBook, gradingPeriods, standards, classAnnouncementTypes));
        }

        [AuthorizationFilter("Teacher")]
        public ActionResult ClassGradingGrid(int classId, int gradingPeriodId, int? standardId, int? classAnnouncementTypeId, bool? notCalculateGrid)
        {
            var gradingPeriod = SchoolLocator.GradingPeriodService.GetGradingPeriodById(gradingPeriodId);
            var gradeBook = SchoolLocator.GradingStatisticService.GetGradeBook(classId, gradingPeriod, standardId, classAnnouncementTypeId, !(notCalculateGrid ?? false));
            return Json(GradingGridViewData.Create(gradeBook));
        }
        
        [AuthorizationFilter("Teacher")]
        public ActionResult GetGridComments(int schoolYearId)
        {
            if (!Context.PersonId.HasValue)
                throw new UnassignedUserException();
            return Json(SchoolLocator.GradingStatisticService.GetGradeBookComments(schoolYearId, Context.PersonId.Value));
        }

        [AuthorizationFilter("Teacher")]
        public ActionResult PostGradebook(int classId, int gradingPeriodId)
        {
            SchoolLocator.GradingStatisticService.PostGradebook(classId, gradingPeriodId);
            return Json(true);
        }

        
        [AuthorizationFilter("Teacher")]
        public ActionResult GetStudentAverages(int classId, int gradingPeriodId, int averageId)
        {
            var studentAverages = SchoolLocator.GradingStatisticService.GetStudentAverages(classId, averageId, gradingPeriodId);
            return Json(studentAverages.Select(StudentAveragesViewData.Create).ToList());
        }

        [AuthorizationFilter("Teacher")]
        public ActionResult FinalGrade(int classId)
        {
            var syId = GetCurrentSchoolYearId();
            var gradingPeriods = SchoolLocator.GradingPeriodService.GetGradingPeriodsDetails(syId);
            var date = Context.NowSchoolYearTime.Date;
            var currentGradingPeriod = SchoolLocator.GradingPeriodService.GetGradingPeriodDetails(syId, date);
            GradingPeriodFinalGradeViewData gradingPeriodFinalGrade = null;
            if (currentGradingPeriod != null)
                gradingPeriodFinalGrade = GetGradingPeriodFinalGrade(classId, currentGradingPeriod, null);
            return Json(FinalGradesViewData.Create(gradingPeriods, gradingPeriodFinalGrade));
        }

        [AuthorizationFilter("Teacher")]
        public ActionResult GradingPeriodFinalGrade(int classId, int gradingPeriodId, int? averageId)
        {
            var gradingPeriod = SchoolLocator.GradingPeriodService.GetGradingPeriodById(gradingPeriodId);
            return Json(GetGradingPeriodFinalGrade(classId, gradingPeriod, averageId));
        }

        private GradingPeriodFinalGradeViewData GetGradingPeriodFinalGrade(int classId, GradingPeriodDetails gradingPeriod, int? averageId)
        {
            if(!Context.SchoolLocalId.HasValue)
                throw new UnassignedUserException();
            var finalGrade = SchoolLocator.GradingStatisticService.GetFinalGrade(classId, gradingPeriod);
            //var attendanceSummary = SchoolLocator.AttendanceService.GetAttendanceSummary(Context.SchoolLocalId.Value, gradingPeriod.Id);
            var average = finalGrade.Averages.FirstOrDefault(x => !averageId.HasValue || x.AverageId == averageId);
            return GradingPeriodFinalGradeViewData.Create(finalGrade, average);
        }

        [AuthorizationFilter("Teacher")]
        public ActionResult ClassStandardGrids(int classId)
        {
            var schoolYearId = GetCurrentSchoolYearId();
            var gradingPeriods = SchoolLocator.GradingPeriodService.GetGradingPeriodsDetails(schoolYearId);
            var date = Context.NowSchoolYearTime.Date;
            var currentGradingPeriod = SchoolLocator.GradingPeriodService.GetGradingPeriodDetails(schoolYearId, date);
            StandardGradingGridViewData currentStandardGrid = null;
            if (currentGradingPeriod != null)
            {
                var students = GetStudentsForGrid(classId, currentGradingPeriod.MarkingPeriodRef);
                var gradingStandards = SchoolLocator.GradingStandardService.GetGradingStandards(classId, currentGradingPeriod.Id);
                currentStandardGrid = StandardGradingGridViewData.Create(currentGradingPeriod, gradingStandards, students);
            }
            return Json(StandardGradingGridsViewData.Create(gradingPeriods, currentStandardGrid));
            //var res = new List<StandardGradingGridViewData>();
            //foreach (var gradingPeriod in gradingPeriods)
            //{
            //    var gs = gradingStandards.Where(x => gradingPeriod.Id == x.GradingPeriodId).ToList();
            //    var currentStudents = students.Where(x => gs.Any(y => y.StudentId == x.Id)).ToList();
            //    res.Add(StandardGradingGridViewData.Create(gradingPeriod, gs, currentStudents));
            //}
        }
        
        [AuthorizationFilter("Teacher")]
        public ActionResult ClassStandardGrid(int classId, int gradingPeriodId)
        {
            var gradingStandards = SchoolLocator.GradingStandardService.GetGradingStandards(classId, gradingPeriodId);
            var gradingPeriod = SchoolLocator.GradingPeriodService.GetGradingPeriodById(gradingPeriodId);
            var students = GetStudentsForGrid(classId, gradingPeriod.MarkingPeriodRef);
            return Json(StandardGradingGridViewData.Create(gradingPeriod, gradingStandards, students));
        }

        private IList<StudentDetails> GetStudentsForGrid(int classId, int markingPeriodId)
        {
            var classRoomOption = SchoolLocator.ClassroomOptionService.GetClassOption(classId);
            bool? enrolled = classRoomOption != null && !classRoomOption.IncludeWithdrawnStudents ? true : default(bool?);
            return SchoolLocator.PersonService.GetClassStudents(classId, markingPeriodId, enrolled)
                                               .OrderBy(x => x.LastName).ThenBy(x => x.FirstName).ToList();
        }

        [AuthorizationFilter("Teacher")]
        public ActionResult ClassStandardSummary(int classId)
        {
            var gradingStandards = SchoolLocator.GradingStandardService.GetGradingStandards(classId, null);
            var anns = SchoolLocator.AnnouncementService.GetAnnouncements(null, 0, int.MaxValue, classId);
            var schoolYearId = GetCurrentSchoolYearId();
            var gradingPeriods = SchoolLocator.GradingPeriodService.GetGradingPeriodsDetails(schoolYearId);
            var res = new List<GradingStandardClassSummaryViewData>();
            var annSts = SchoolLocator.AnnouncementService.GetAnnouncementStandards(classId);
            foreach (var gradingPeriod in gradingPeriods)
            {
                var gs = gradingStandards.Where(x => gradingPeriod.Id == x.GradingPeriodId).ToList();
                var announcements = anns.Where(x => x.Expires >= gradingPeriod.StartDate && x.Expires <= gradingPeriod.EndDate).ToList();
                res.Add(GradingStandardClassSummaryViewData.Create(gradingPeriod, gs, announcements, annSts));
            }
            return Json(res);
        }

        [AuthorizationFilter("Teacher")]
        public ActionResult UpdateStandardGrade(int classId, int gradingPeriodId
            , int studentId, int standardId, int? alphaGradeId, string note)
        {
            var res = SchoolLocator.GradingStandardService.SetGrade(studentId, standardId, classId
                , gradingPeriodId, alphaGradeId, note);
            return Json(StandardGradingItemViewData.Create(res));
        }

        
        [AuthorizationFilter("AdminGrade, AdminEdit, AdminView, Teacher, Student")]
        public ActionResult StudentSummary(int studentId, int? classId)
        {
            var res = new GradingStudentSummaryViewData {Announcements = GetGradedItems()};
            return Json(res);
        }
     
        [AuthorizationFilter("AdminGrade, AdminEdit, AdminView, Teacher, Student")]
        public ActionResult RecentlyGradedItems(int? start, int? count, int? classId)
        {
            return Json(GetGradedItems(start, count, classId));
        }

        private IList<AnnouncementViewData> GetGradedItems(int? start = null, int? count = null, int? classId = null)
        {
            return FeedController.GetAnnouncementForFeedList(SchoolLocator, start, count, null, classId, false, true);
        }

        [AuthorizationFilter("AdminGrade, AdminEdit, AdminView, Teacher, Student")]
        public ActionResult StudentClassSummary(int studentId, int classId)
        {
            //var gradingStats = SchoolLocator.GradingStatisticService.GetStudentClassGradeStats(mp.Id, classId, studentId);
           // var gradingPerMp = ClassLogic.GetGradingSummary(SchoolLocator, classId, mp.SchoolYearRef, null, studentId, false);
            //return Json(GradingStudentClassSummaryViewData.Create(gradingStats.FirstOrDefault(), mp, gradingPerMp));
            ClassGradingBoxesViewData classGarding = null;//PrepareClassGradingBoxes(classId);
            var mp = SchoolLocator.MarkingPeriodService.GetMarkingPeriodByDate(Context.NowSchoolYearTime.Date, true);
            return Json(GradingStudentClassSummaryViewData.Create(mp, GetGradedItems(null, null, classId), classGarding));
        }

        //TODO: duplicate part of announcement/read data. for API compatibility only
        /*[AuthorizationFilter("Teacher", Preference.API_DESCR_GRADE_LIST_ITEMS, true, CallType.Get, new[] { AppPermissionType.Grade, AppPermissionType.Announcement })]
        public ActionResult ItemGradesList(int announcementId)
        {
            var annView = SchoolLocator.AnnouncementService.GetAnnouncementDetails(announcementId);
            var attachmentsInfo = AttachmentLogic.PrepareAttachmentsInfo(annView.AnnouncementAttachments);
            var res = StudentAnnouncementLogic.ItemGradesList(SchoolLocator, annView, attachmentsInfo);
            return Json(res);
        }*/

        /*public ActionResult ApplyAutoGrade(int announcementId)
        {
            SchoolLocator.StudentAnnouncementService.ResolveAutoGrading(announcementId, true);
            return ItemGradesList(announcementId);
        }

        public ActionResult ApplyManualGrade(int announcementId)
        {
            SchoolLocator.StudentAnnouncementService.ResolveAutoGrading(announcementId, false);
            return ItemGradesList(announcementId);
        }*/

        //TODO: do we need this in API still?
        //[AuthorizationFilter("Teacher", Preference.API_DESCR_GRADE_UPDATE_ITEM, true, CallType.Get, new[] { AppPermissionType.Grade, AppPermissionType.Announcement })]
        [AuthorizationFilter("Teacher")]
        public ActionResult UpdateItem(int announcementId, int studentId, string gradeValue, string extraCredits
            , string comment, bool dropped, bool? exempt, bool? incomplete, bool? late)
        {
            var studentAnn = SchoolLocator.StudentAnnouncementService.SetGrade(announcementId, studentId, gradeValue, extraCredits
                , comment, dropped, late ?? false, exempt ?? false, incomplete ?? false
                , (int)GradingStyleEnum.Numeric100);
            MasterLocator.UserTrackingService.SetScore(Context.Login,
                studentAnn.AnnouncementId,
                studentAnn.StudentId,
                gradeValue,
                extraCredits);
            return Json(ShortStudentAnnouncementViewData.Create(studentAnn));
        }

        [AuthorizationFilter("Teacher")]
        public ActionResult UpdateStudentAverage(int classId, int gradingPeriodId, int studentId, int averageId, string averageValue, bool exempt
            , IList<StudentAverageCommentViewData> codes, string note)
        {
            IList<ChalkableStudentAverageComment> comments = null;
            if (codes != null)
                comments = codes.Select(x => new ChalkableStudentAverageComment
                    {
                        HeaderId = x.HeaderId,
                        AverageId = averageId,
                        StudentId = studentId,
                        HeaderText = x.HeaderName,
                        GradingComment = x.GradingComment == null ? null 
                                        : new GradingComment
                                            {
                                                Comment = x.GradingComment.Comment,
                                                Code = x.GradingComment.Code,
                                                Id = x.GradingComment.Id
                                            }
                    }).ToList(); 
            var res = SchoolLocator.GradingStatisticService.UpdateStudentAverage(classId, studentId, averageId, gradingPeriodId
                , averageValue, exempt, comments ?? new List<ChalkableStudentAverageComment>(), note);

            MasterLocator.UserTrackingService.SetFinalGrade(Context.Login, classId, studentId, gradingPeriodId,
                averageValue, exempt, note);

            return Json(StudentAveragesViewData.Create(res));
        }
    }
}