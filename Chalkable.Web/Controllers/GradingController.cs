using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Chalkable.BusinessLogic.Model;
using Chalkable.BusinessLogic.Services.School.Announcements;
using Chalkable.Common.Exceptions;
using Chalkable.Data.Common.Enums;
using Chalkable.Data.Master.Model;
using Chalkable.Data.School.Model;
using Chalkable.Web.ActionFilters;
using Chalkable.Web.Models;
using Chalkable.Web.Models.AnnouncementsViewData;
using Chalkable.Web.Models.ClassesViewData;
using Chalkable.Web.Models.GradingViewData;

namespace Chalkable.Web.Controllers
{
    [RequireHttps, TraceControllerFilter]
    public class GradingController : ChalkableController
    {
        [AuthorizationFilter("Teacher")]
        public async Task<ActionResult> TeacherSummary(int teacherId)
        {
            var schoolYearId = GetCurrentSchoolYearId();
            var gradingPeriod = SchoolLocator.GradingPeriodService.GetGradingPeriodDetails(schoolYearId, Context.NowSchoolYearTime.Date);
            var classesGradesSummary = await SchoolLocator.GradingStatisticService.GetClassesGradesSummary(teacherId, gradingPeriod);
            return Json(GradingTeacherClassSummaryViewData.Create(classesGradesSummary), 6);
        }

        [AuthorizationFilter("DistrictAdmin, Teacher", true, new[] { AppPermissionType.Grade, AppPermissionType.Class })]
        public async Task<ActionResult> ClassSummary(int classId)
        {
            if (!SchoolLocator.Context.PersonId.HasValue)
                throw new UnassignedUserException();
            return await Json(PrepareClassGradingBoxes(classId));
        }

        private async Task<ClassGradingBoxesViewData> PrepareClassGradingBoxes(int classId)
        {
            var syId = GetCurrentSchoolYearId();
            var gradingPeriods = SchoolLocator.GradingPeriodService.GetGradingPeriodsDetailsByClassId(classId);
            var currentGradingPeriod = CurrentGradingPeriodFromList(gradingPeriods, Context.NowSchoolYearTime.Date);
            TeacherClassGrading gradingSummary = null;
            if (currentGradingPeriod != null)
            {
                gradingSummary = await SchoolLocator.GradingStatisticService.GetClassGradingSummary(classId, currentGradingPeriod);
            }
            return ClassGradingBoxesViewData.Create(gradingPeriods, gradingSummary);
        }

        [AuthorizationFilter("DistrictAdmin, Teacher")]
        public async Task<ActionResult> ClassGradingPeriodSummary(int classId, int gradingPeriodId)
        {
            var gradingPeriod = SchoolLocator.GradingPeriodService.GetGradingPeriodById(gradingPeriodId);
            var res = await SchoolLocator.GradingStatisticService.GetClassGradingSummary(classId, gradingPeriod);
            return Json(GradingClassSummaryViewData.Create(res));
        }

        [AuthorizationFilter("DistrictAdmin, Teacher, Student", true, new[] { AppPermissionType.Grade, AppPermissionType.Class })]
        public ActionResult ItemGradingStat(int announcementId)
        {
            var studentAnns = SchoolLocator.StudentAnnouncementService.GetStudentAnnouncements(announcementId);
            return Json(ItemGradigStatViewData.Create(studentAnns, announcementId));
        }


        [AuthorizationFilter("DistrictAdmin, Teacher", true, new[] { AppPermissionType.Grade, AppPermissionType.Class })]
        public async Task<ActionResult> ClassSummaryGrids(int classId)
        {
            var gradingPeriods = SchoolLocator.GradingPeriodService.GetGradingPeriodsDetailsByClassId(classId);
            var date = Context.NowSchoolYearTime.Date;
            var currentGradingPeriod = CurrentGradingPeriodFromList(gradingPeriods, date);
            Task<ChalkableGradeBook> gradeBookTask = null;
            if(currentGradingPeriod != null)
                gradeBookTask = SchoolLocator.GradingStatisticService.GetGradeBook(classId, currentGradingPeriod);
            var standards = SchoolLocator.StandardService.GetStandards(classId, null, null);
            var classAnnouncementTypes = SchoolLocator.ClassAnnouncementTypeService.GetClassAnnouncementTypes(classId);
            return Json(GradingGridsViewData.Create(gradeBookTask != null ? await gradeBookTask : null, gradingPeriods, standards, classAnnouncementTypes));
        }

        private static GradingPeriod CurrentGradingPeriodFromList(IList<GradingPeriod> gradingPeriods, DateTime date)
        {
            var currentGradingPeriod = gradingPeriods.FirstOrDefault(x => x.StartDate.Date <= date && x.EndDate >= date);
            if (currentGradingPeriod == null)
                currentGradingPeriod = gradingPeriods.Where(x => x.StartDate.Date <= date)
                    .OrderByDescending(x => x.StartDate).FirstOrDefault();
            return currentGradingPeriod;
        }

        [AuthorizationFilter("DistrictAdmin, Teacher")]
        public async Task<ActionResult> ClassGradingGrid(int classId, int gradingPeriodId, int? standardId, int? classAnnouncementTypeId, bool? notCalculateGrid)
        {
            var gradingPeriod = SchoolLocator.GradingPeriodService.GetGradingPeriodById(gradingPeriodId);
            var gradeBookTask = SchoolLocator.GradingStatisticService.GetGradeBook(classId, gradingPeriod, standardId, classAnnouncementTypeId, !(notCalculateGrid ?? false));
            return Json(GradingGridViewData.Create(await gradeBookTask));
        }
        
        [AuthorizationFilter("DistrictAdmin, Teacher")]
        public ActionResult GetGridComments(int schoolYearId)
        {
            if (!Context.PersonId.HasValue)
                throw new UnassignedUserException();
            return Json(SchoolLocator.GradingStatisticService.GetGradeBookComments(schoolYearId, Context.PersonId.Value));
        }

        [AuthorizationFilter("DistrictAdmin, Teacher")]
        public ActionResult PostGradebook(int classId, int gradingPeriodId)
        {
            SchoolLocator.GradingStatisticService.PostGradebook(classId, gradingPeriodId);
            MasterLocator.UserTrackingService.PostedGrades(Context.Login, classId, gradingPeriodId);
            return Json(true);
        }

        [AuthorizationFilter("DistrictAdmin, Teacher")]
        public ActionResult PostStandards(int classId, int gradingPeriodId)
        {
            SchoolLocator.GradingStatisticService.PostStandards(classId, gradingPeriodId);
            MasterLocator.UserTrackingService.PostedGrades(Context.Login, classId, gradingPeriodId);
            return Json(true);
        }

        [AuthorizationFilter("DistrictAdmin, Teacher")]
        public ActionResult GetStudentAverages(int classId, int gradingPeriodId, int averageId)
        {
            var studentAverages = SchoolLocator.GradingStatisticService.GetStudentAverages(classId, averageId, gradingPeriodId);
            return Json(studentAverages.Select(StudentAveragesViewData.Create).ToList());
        }

        [AuthorizationFilter("DistrictAdmin, Teacher")]
        public async Task<ActionResult> FinalGrade(int classId)
        {
            var gradingPeriods = SchoolLocator.GradingPeriodService.GetGradingPeriodsDetailsByClassId(classId);
            var currentGradingPeriod = CurrentGradingPeriodFromList(gradingPeriods, Context.NowSchoolYearTime.Date);
            GradingPeriodFinalGradeViewData gradingPeriodFinalGrade = null;
            if (currentGradingPeriod != null)
                gradingPeriodFinalGrade = await GetGradingPeriodFinalGrade(classId, currentGradingPeriod, null);
            return Json(FinalGradesViewData.Create(gradingPeriods, gradingPeriodFinalGrade));
        }

        [AuthorizationFilter("DistrictAdmin, Teacher")]
        public async Task<ActionResult> GradingPeriodFinalGrade(int classId, int gradingPeriodId, int? averageId)
        {
            var gradingPeriod = SchoolLocator.GradingPeriodService.GetGradingPeriodById(gradingPeriodId);
            return await Json(GetGradingPeriodFinalGrade(classId, gradingPeriod, averageId));
        }

        private async Task<GradingPeriodFinalGradeViewData> GetGradingPeriodFinalGrade(int classId, GradingPeriod gradingPeriod, int? averageId)
        {
            if(!Context.SchoolLocalId.HasValue)
                throw new UnassignedUserException();
            var finalGrade = await SchoolLocator.GradingStatisticService.GetFinalGrade(classId, gradingPeriod);
            var average = finalGrade.Averages.FirstOrDefault(x => !averageId.HasValue || x.AverageId == averageId);
            return GradingPeriodFinalGradeViewData.Create(finalGrade, average);
        }

        [AuthorizationFilter("DistrictAdmin, Teacher")]
        public async Task<ActionResult> ClassStandardGrids(int classId)
        {
            var gradingPeriods = SchoolLocator.GradingPeriodService.GetGradingPeriodsDetailsByClassId(classId);
            var currentGradingPeriod = CurrentGradingPeriodFromList(gradingPeriods, Context.NowSchoolYearTime.Date);
            StandardGradingGridViewData currentStandardGrid = null;
            if (currentGradingPeriod != null)
            {
                var students = GetStudentsForGrid(classId, currentGradingPeriod.MarkingPeriodRef);
                var gradingStandardsTask = SchoolLocator.GradingStandardService.GetGradingStandards(classId, currentGradingPeriod.Id);
                currentStandardGrid = StandardGradingGridViewData.Create(currentGradingPeriod, await gradingStandardsTask, students);
            }
            return Json(StandardGradingGridsViewData.Create(gradingPeriods, currentStandardGrid));
        }
        
        [AuthorizationFilter("DistrictAdmin, Teacher")]
        public async Task<ActionResult> ClassStandardGrid(int classId, int gradingPeriodId)
        {
            var gradingStandardsTask = SchoolLocator.GradingStandardService.GetGradingStandards(classId, gradingPeriodId);
            var gradingPeriod = SchoolLocator.GradingPeriodService.GetGradingPeriodById(gradingPeriodId);
            var students = GetStudentsForGrid(classId, gradingPeriod.MarkingPeriodRef);
            var gradingStandards = await gradingStandardsTask;
            return Json(StandardGradingGridViewData.Create(gradingPeriod, gradingStandards, students));
        }

        private IList<StudentDetails> GetStudentsForGrid(int classId, int markingPeriodId)
        {
            var classRoomOption = SchoolLocator.ClassroomOptionService.GetClassOption(classId);
            bool? enrolled = classRoomOption != null && !classRoomOption.IncludeWithdrawnStudents ? true : default(bool?);
            return SchoolLocator.StudentService.GetClassStudents(classId, markingPeriodId, enrolled)
                                               .OrderBy(x => x.LastName).ThenBy(x => x.FirstName).ToList();
        }

        [AuthorizationFilter("DistrictAdmin, Teacher")]
        public async Task<ActionResult> ClassStandardSummary(int classId)
        {
            var gradingStandardsTask = SchoolLocator.GradingStandardService.GetGradingStandards(classId, null);
            var anns = SchoolLocator.ClassAnnouncementService.GetClassAnnouncements(null, null, classId, null, null);
            var gradingPeriods = SchoolLocator.GradingPeriodService.GetGradingPeriodsDetailsByClassId(classId);
            var res = new List<GradingStandardClassSummaryViewData>();
            var annSts = SchoolLocator.ClassAnnouncementService.GetAnnouncementStandards(classId);
            var gradingStandards = await gradingStandardsTask;
            foreach (var gradingPeriod in gradingPeriods)
            {
                var gs = gradingStandards.Where(x => gradingPeriod.Id == x.GradingPeriodId).ToList();
                var announcements = anns.Where(x => x.Expires >= gradingPeriod.StartDate && x.Expires <= gradingPeriod.EndDate).ToList();
                res.Add(GradingStandardClassSummaryViewData.Create(gradingPeriod, gs, announcements, annSts));
            }
            return Json(res);
        }

        [AuthorizationFilter("DistrictAdmin, Teacher")]
        public ActionResult UpdateStandardGrade(int classId, int gradingPeriodId
            , int studentId, int standardId, int? alphaGradeId, string note)
        {
            var res = SchoolLocator.GradingStandardService.SetGrade(studentId, standardId, classId
                , gradingPeriodId, alphaGradeId, note);
            return Json(StandardGradingItemViewData.Create(res));
        }

        
        [AuthorizationFilter("DistrictAdmin, Teacher, Student")]
        public ActionResult StudentSummary(int studentId, int? classId)
        {
            var res = new GradingStudentSummaryViewData {Announcements = GetGradedItems()};
            return Json(res);
        }
     
        [AuthorizationFilter("DistrictAdmin, Teacher, Student")]
        public ActionResult RecentlyGradedItems(int? start, int? count, int? classId)
        {
            return Json(GetGradedItems(start, count, classId));
        }

        private IList<AnnouncementViewData> GetGradedItems(int? start = null, int? count = null, int? classId = null)
        {
            var st = start ?? 0;
            var cn = count ?? 10;
            var anns = SchoolLocator.ClassAnnouncementService.GetClassAnnouncementsForFeed(null, null, classId, null, true, st, cn);
            return FeedController.PrepareAnnouncementsComplexViewData(SchoolLocator, anns);
        }

        [AuthorizationFilter("DistrictAdmin, Teacher, Student")]
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

        //TODO: do we need this in API still?
        //[AuthorizationFilter("Teacher", Preference.API_DESCR_GRADE_UPDATE_ITEM, true, CallType.Get, new[] { AppPermissionType.Grade, AppPermissionType.Announcement })]
        [AuthorizationFilter("DistrictAdmin, Teacher")]
        public ActionResult UpdateItem(int announcementId, int studentId, string gradeValue, string extraCredits
            , string comment, bool dropped, bool? exempt, bool? incomplete, bool? late, bool? callFromGradeBook, bool? commentWasChanged)
        {
            var studentAnn = SchoolLocator.StudentAnnouncementService.SetGrade(announcementId, studentId, gradeValue, extraCredits
                , comment, dropped, late ?? false, exempt ?? false, incomplete ?? false, commentWasChanged ?? false
                , (int)GradingStyleEnum.Numeric100);
            return Json(ShortStudentAnnouncementViewData.Create(studentAnn));
        }

        [AuthorizationFilter("DistrictAdmin, Teacher")]
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

        [AuthorizationFilter("Teacher, Student", true, new[] { AppPermissionType.Grade, AppPermissionType.Announcement })]
        public ActionResult SetAutoGrade(int announcementApplicationId, int? studentId, string gradeValue)
        {
            if (string.IsNullOrWhiteSpace(gradeValue))
                throw new ChalkableException("Param gradeValue is required");

            var autoGrade = SchoolLocator.StudentAnnouncementService.SetAutoGrade(announcementApplicationId, studentId, gradeValue);
            MasterLocator.UserTrackingService.AutoGradedItem(Context.Login, autoGrade.AnnouncementApplication.AnnouncementRef, autoGrade.StudentRef, autoGrade.Grade);
            return Json(true);
        }

        [AuthorizationFilter("DistrictAdmin, Teacher, Student")]
        public ActionResult GradedItemsList(int gradingPeriodId)
        {
            var res = SchoolLocator.GradedItemService.GetGradedItems(gradingPeriodId);
            return Json(GradedItemViewData.Create(res), 3);
        }

        [AuthorizationFilter("Teacher, Student")]
        public ActionResult GradingScalesList()
        {
            var res = SchoolLocator.GradingScaleService.GetGradingScales();
            return Json(GradingScaleViewData.Create(res));
        }
        
    }
}