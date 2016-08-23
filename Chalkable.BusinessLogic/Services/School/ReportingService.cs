﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using Chalkable.BusinessLogic.Model;
using Chalkable.BusinessLogic.Model.Reports;
using Chalkable.BusinessLogic.Security;
using Chalkable.BusinessLogic.Services.Reporting;
using Chalkable.Common;
using Chalkable.Common.Exceptions;
using Chalkable.Data.School.Model;
using Chalkable.Data.School.Model.Announcements;
using Chalkable.StiConnector.Connectors.Model.Reports;

namespace Chalkable.BusinessLogic.Services.School
{
    public interface IReportingService
    {
        IList<StudentCommentInfo> GetProgressReportComments(int classId, int gradingPeriodId);
        void SetProgressReportComment(int classId, int studentId, int gradingPeriodId, string comment);
        void SetProgressReportComments(int classId, int gradingPeriodId, IList<StudentCommentInputModel> studentComments);

        byte[] GetGradebookReport(GradebookReportInputModel gradebookReportInput);
        byte[] GetWorksheetReport(WorksheetReportInputModel worksheetReportInput);
        byte[] GetProgressReport(ProgressReportInputModel inputModel);
        byte[] GetComprehensiveProgressReport(ComprehensiveProgressInputModel comprehensiveProgressInput);
        byte[] GetMissingAssignmentsReport(MissingAssignmentsInputModel missingAssignmentsInput);
        byte[] GetBirthdayReport(BirthdayReportInputModel birthdayReportInput);
        byte[] GetAttendanceRegisterReport(AttendanceRegisterInputModel inputModel);
        byte[] GetAttendanceProfileReport(AttendanceProfileReportInputModel inputModel);
        byte[] GetSeatingChartReport(SeatingChartReportInputModel inputModel);
        byte[] GetGradeVerificationReport(GradeVerificationInputModel inputModel);
        byte[] GetLessonPlanReport(LessonPlanReportInputModel inputModel);
        byte[] GetStudentComprehensiveReport(int studentId, int gradingPeriodId);
        byte[] GetFeedReport(FeedReportInputModel inputModel, string path);
        byte[] GetReportCards(ReportCardsInputModel inputModel);
        FeedReportSettingsInfo GetFeedReportSettings();
        void SetFeedReportSettings(FeedReportSettingsInfo feedReportSettings);
    }

    public class ReportingService : SisConnectedService, IReportingService
    {
        public ReportingService(IServiceLocatorSchool serviceLocator) : base(serviceLocator)
        {
        }

        public IList<StudentCommentInfo> GetProgressReportComments(int classId, int gradingPeriodId)
        {
            var inowReportComments = ConnectorLocator.ReportConnector.GetProgressReportComments(classId, gradingPeriodId);
            var gp = ServiceLocator.GradingPeriodService.GetGradingPeriodById(gradingPeriodId);
            int markingPeriodId = gp.MarkingPeriodRef;
            var students = ServiceLocator.StudentService.GetClassStudents(classId, markingPeriodId);
            var res = new List<StudentCommentInfo>();
            foreach (var student in students)
            {
                var studentComment = inowReportComments.FirstOrDefault(sc => sc.StudentId == student.Id);
                if (studentComment == null) continue;
                res.Add(new StudentCommentInfo
                {
                    Student = student,
                    Comment = studentComment.Comment
                });
            }
            return res;
        }

        public void SetProgressReportComment(int classId, int studentId, int gradingPeriodId, string comment)
        {
            var stComment = new StudentCommentInputModel {Comment = comment, StudentId = studentId};
            SetProgressReportComments(classId, gradingPeriodId, new List<StudentCommentInputModel> {stComment});
        }
        
        public void SetProgressReportComments(int classId, int gradingPeriodId, IList<StudentCommentInputModel> studentComments)
        {
            if (studentComments == null || studentComments.Count <= 0) return;
            var inowStudentProgressReportComments =
                studentComments.Select(x => new StudentProgressReportComment
                    {
                        Comment = x.Comment,
                        StudentId = x.StudentId,
                        GradingPeriodId = gradingPeriodId,
                        SectionId = classId
                    }).ToList();
            ConnectorLocator.ReportConnector.UpdateProgressReportComment(classId, inowStudentProgressReportComments);
        }

        public byte[] GetGradebookReport(GradebookReportInputModel inputModel)
        {
            var gp = ServiceLocator.GradingPeriodService.GetGradingPeriodById(inputModel.GradingPeriodId);
            var stiModel = new GradebookReportParams
                {
                    AcadSessionId = gp.SchoolYearRef,
                    StartDate = inputModel.StartDate,
                    EndDate = inputModel.EndDate,
                    DisplayStudentAverage = inputModel.DisplayStudentAverage,
                    DisplayLetterGrade = inputModel.DisplayLetterGrade,
                    DisplayTotalPoints = inputModel.DisplayTotalPoints,
                    IncludeWithdrawnStudents = inputModel.IncludeWithdrawnStudents,
                    IncludeNonGradedActivities = inputModel.IncludeNonGradedActivities,
                    SuppressStudentName = inputModel.SuppressStudentName,
                    OrderBy = inputModel.OrderBy,
                    GroupBy = inputModel.GroupBy,
                    IdToPrint = inputModel.IdToPrint,
                    ReportType = inputModel.ReportType,
                    GradingPeriodId = inputModel.GradingPeriodId,
                    SectionId = inputModel.ClassId
                };
            if (inputModel.StudentIds == null)
            {
                var students = ServiceLocator.StudentService.GetClassStudents(inputModel.ClassId, gp.MarkingPeriodRef);
                stiModel.StudentIds = students.Select(x => x.Id).ToArray();
            }
            else stiModel.StudentIds = inputModel.StudentIds.ToArray();
            if (CoreRoles.TEACHER_ROLE == Context.Role)
                stiModel.StaffId = Context.PersonId;
            return ConnectorLocator.ReportConnector.GradebookReport(stiModel);
        }

        public byte[] GetWorksheetReport(WorksheetReportInputModel inputModel)
        {
            int[] activityIds = new int[10];
            if (inputModel.AnnouncementIds != null && inputModel.AnnouncementIds.Count > 0)
            {
                var anns = ServiceLocator.ClassAnnouncementService.GetClassAnnouncements(inputModel.StartDate, inputModel.EndDate, null, null, null); 
                anns = anns.Where(x => x.SisActivityId.HasValue && inputModel.AnnouncementIds.Contains(x.Id)).ToList();
                activityIds = anns.Select(x => x.SisActivityId.Value).ToArray();    
            }

            var gp = ServiceLocator.GradingPeriodService.GetGradingPeriodById(inputModel.GradingPeriodId);
            var stiModel = new WorksheetReportParams
                {
                    ActivityIds = activityIds,
                    StartDate = inputModel.StartDate,
                    EndDate = inputModel.EndDate,
                    Header = inputModel.Header,
                    IdToPrint = inputModel.IdToPrint,
                    PrintAverage = inputModel.PrintAverage,
                    PrintLetterGrade = inputModel.PrintLetterGrade,
                    PrintScores = inputModel.PrintScores,
                    PrintStudent = inputModel.PrintStudent,
                    Title1 = inputModel.Title1,
                    Title2 = inputModel.Title2,
                    Title3 = inputModel.Title3,
                    Title4 = inputModel.Title4,
                    Title5 = inputModel.Title5,
                    SectionId = inputModel.ClassId,
                    GradingPeriodId = inputModel.GradingPeriodId,
                };
            if (inputModel.StudentIds == null)
            {
                var students = ServiceLocator.StudentService.GetClassStudents(inputModel.ClassId, gp.MarkingPeriodRef);
                stiModel.StudentIds = students.Select(x => x.Id).ToArray();
            }
            else stiModel.StudentIds = inputModel.StudentIds.ToArray();

            if (CoreRoles.TEACHER_ROLE == Context.Role)
                stiModel.StaffId = Context.PersonId;
            stiModel.AcadSessionId = gp.SchoolYearRef;
            return ConnectorLocator.ReportConnector.WorksheetReport(stiModel);
        }

        public byte[] GetProgressReport(ProgressReportInputModel inputModel)
        {
            var gp = ServiceLocator.GradingPeriodService.GetGradingPeriodById(inputModel.GradingPeriodId);
            var stiModel = new ProgressReportParams
                {
                    AcadSessionId = gp.SchoolYearRef,
                    AbsenceReasonIds = inputModel.AbsenceReasonIds.ToArray(),
                    AdditionalMailings = inputModel.AdditionalMailings,
                    DailyAttendanceDisplayMethod = inputModel.DailyAttendanceDisplayMethod,
                    DisplayCategoryAverages = inputModel.DisplayCategoryAverages,
                    DisplayClassAverages = inputModel.DisplayClassAverages,
                    DisplayLetterGrade = inputModel.DisplayLetterGrade,
                    DisplayPeriodAttendance = inputModel.DisplayPeriodAttendance,
                    DisplaySignatureLine = inputModel.DisplaySignatureLine,
                    DisplayStudentComments = inputModel.DisplayStudentComments,
                    DisplayStudentMailingAddress = inputModel.DisplayStudentMailingAddress,
                    DisplayTotalPoints = inputModel.DisplayTotalPoints,
                    SectionId = inputModel.ClassId,
                    GoGreen = inputModel.GoGreen,
                    GradingPeriodId = inputModel.GradingPeriodId,
                    IdToPrint = inputModel.IdToPrint,
                    PrintFromHomePortal = inputModel.PrintFromHomePortal,
                    MaxCategoryClassAverage = inputModel.MaxCategoryClassAverage,
                    MinCategoryClassAverage = inputModel.MinCategoryClassAverage,
                    MaxStandardAverage = inputModel.MaxStandardAverage,
                    MinStandardAverage = inputModel.MinStandardAverage,
                    SectionComment = inputModel.ClassComment,
                };
            //TODO: maiby remove this later after inow fix
            if (inputModel.StudentIds == null)
            {
                var students = ServiceLocator.StudentService.GetClassStudents(inputModel.ClassId, gp.MarkingPeriodRef);
                stiModel.StudentIds = students.Select(x => x.Id).ToArray();
            }
            else stiModel.StudentIds = inputModel.StudentIds.ToArray();

            return ConnectorLocator.ReportConnector.ProgressReport(stiModel);
        }

        public byte[] GetComprehensiveProgressReport(ComprehensiveProgressInputModel comprehensiveProgressInput)
        {
            var defaultGp = ServiceLocator.GradingPeriodService.GetGradingPeriodById(comprehensiveProgressInput.GradingPeriodId);
            var stiModel = new ComprehensiveProgressParams
                {
                    EndDate = comprehensiveProgressInput.EndDate,
                    StartDate = comprehensiveProgressInput.StartDate,
                    AbsenceReasonIds = comprehensiveProgressInput.AbsenceReasonIds != null ? comprehensiveProgressInput.AbsenceReasonIds.ToArray() : null,
                    IdToPrint = comprehensiveProgressInput.IdToPrint,
                    AcadSessionId = defaultGp.SchoolYearRef,
                    GradingPeriodIds =  comprehensiveProgressInput.GradingPeriodIds.ToArray(),
                    AdditionalMailings = comprehensiveProgressInput.AdditionalMailings,
                    ClassAverageOnly = comprehensiveProgressInput.ClassAverageOnly,
                    DailyAttendanceDisplayMethod = comprehensiveProgressInput.DailyAttendanceDisplayMethod,
                    DisplayCategoryAverages = comprehensiveProgressInput.DisplayCategoryAverages,
                    DisplayClassAverage = comprehensiveProgressInput.DisplayClassAverage,
                    DisplayPeriodAttendance = comprehensiveProgressInput.DisplayPeriodAttendance,
                    DisplaySignatureLine = comprehensiveProgressInput.DisplaySignatureLine,
                    DisplayStudentComment = comprehensiveProgressInput.DisplayStudentComment,
                    DisplayStudentMailingAddress = comprehensiveProgressInput.DisplayStudentMailingAddress,
                    DisplayTotalPoints = comprehensiveProgressInput.DisplayTotalPoints,
                    MaxStandardAverage = comprehensiveProgressInput.MaxStandardAverage,
                    MinStandardAverage = comprehensiveProgressInput.MinStandardAverage,
                    GoGreen = comprehensiveProgressInput.GoGreen,
                    OrderBy = comprehensiveProgressInput.OrderBy,
                    SectionId = comprehensiveProgressInput.ClassId,
                    WindowEnvelope = comprehensiveProgressInput.WindowEnvelope,
                    StudentFilterId = comprehensiveProgressInput.StudentFilterId,
                    StudentIds = comprehensiveProgressInput.StudentIds != null ? comprehensiveProgressInput.StudentIds.ToArray() : null,
                    IncludePicture = comprehensiveProgressInput.IncludePicture,
                    IncludeWithdrawn = comprehensiveProgressInput.IncludeWithdrawn,
                   
                };
            return ConnectorLocator.ReportConnector.ComprehensiveProgressReport(stiModel);
        }

        public byte[] GetMissingAssignmentsReport(MissingAssignmentsInputModel missingAssignmentsInput)
        {
            bool? isEnrolled = missingAssignmentsInput.IncludeWithdrawn ? (bool?)null : true;
            var gradingPeriod = ServiceLocator.GradingPeriodService.GetGradingPeriodById(missingAssignmentsInput.GradingPeriodId);
            var stiModel = new MissingAssignmentsParams
                {
                    AcadSessionId = gradingPeriod.SchoolYearRef,
                    AlternateScoreIds = missingAssignmentsInput.AlternateScoreIds?.ToArray(),
                    AlternateScoresOnly = missingAssignmentsInput.AlternateScoresOnly,
                    EndDate = missingAssignmentsInput.EndDate,
                    ConsiderZerosAsMissingGrades = missingAssignmentsInput.ConsiderZerosAsMissingGrades,
                    IdToPrint = missingAssignmentsInput.IdToPrint,
                    IncludeWithdrawn = missingAssignmentsInput.IncludeWithdrawn,
                    OnePerPage = missingAssignmentsInput.OnePerPage,
                    OrderBy = missingAssignmentsInput.OrderBy,
                    SectionId = missingAssignmentsInput.ClassId,
                    StartDate = missingAssignmentsInput.StartDate,
                    SuppressStudentName = missingAssignmentsInput.SuppressStudentName, 
                };
            if (missingAssignmentsInput.StudentIds == null)
            {
                var students = ServiceLocator.StudentService.GetClassStudents(missingAssignmentsInput.ClassId, gradingPeriod.MarkingPeriodRef, isEnrolled);
                stiModel.StudentIds = students.Select(x => x.Id).ToArray();
            }
            else stiModel.StudentIds = missingAssignmentsInput.StudentIds.ToArray();
            return ConnectorLocator.ReportConnector.MissingAssignmentsReport(stiModel);
        }

        public byte[] GetBirthdayReport(BirthdayReportInputModel inputModel)
        {
            var gp = ServiceLocator.GradingPeriodService.GetGradingPeriodById(inputModel.GradingPeriodId);
            var ps = new BirthdayReportParams
                {
                    AcadSessionId = gp.SchoolYearRef,
                    EndDate = inputModel.EndDate,
                    StartDate = inputModel.StartDate,
                    EndMonth = inputModel.EndMonth,
                    StartMonth = inputModel.StartMonth,
                    GroupBy = inputModel.GroupBy,
                    IncludePhoto = inputModel.IncludePhoto,
                    IncludeWithdrawn = inputModel.IncludeWithdrawn,
                    SectionId = inputModel.ClassId
                };
            return ConnectorLocator.ReportConnector.BirthdayReport(ps);
        }

        public byte[] GetAttendanceRegisterReport(AttendanceRegisterInputModel inputModel)
        {
            var gp = ServiceLocator.GradingPeriodService.GetGradingPeriodById(inputModel.GradingPeriodId);
            var ps = new AttendanceRegisterReportParams
                {
                    AcadSessionId = gp.SchoolYearRef,
                    AbsenceReasonIds = inputModel.AbsenceReasonIds?.ToArray(),
                    IdToPrint = inputModel.IdToPrint,
                    IncludeTardies = inputModel.IncludeTardies,
                    MonthId = inputModel.MonthId,
                    ReportType = inputModel.ReportType,
                    SectionId = inputModel.ClassId,
                    ShowLocalReasonCode = inputModel.ShowLocalReasonCode
                };
            return ConnectorLocator.ReportConnector.AttendnaceRegisterReport(ps);
        }

        public byte[] GetAttendanceProfileReport(AttendanceProfileReportInputModel inputModel)
        {
            var gp = ServiceLocator.GradingPeriodService.GetGradingPeriodById(inputModel.GradingPeriodId);
            var ps = new AttendanceProfileReportParams
                {
                    AbsenceReasonIds = inputModel.AbsenceReasons?.ToArray(),
                    AcadSessionId = gp.SchoolYearRef,
                    IncludeNote = inputModel.DisplayNote,
                    IncludePeriodAbsences = inputModel.DisplayPeriodAbsences,
                    IncludeReasonTotals = inputModel.DisplayReasonTotals,
                    IncludeWithdrawnStudents = inputModel.DisplayWithdrawnStudents,
                    StartDate = inputModel.StartDate,
                    EndDate = inputModel.EndDate,
                    GroupBy = inputModel.GroupBy,
                    SectionId = inputModel.ClassId,
                    IdToPrint = inputModel.IdToPrint,
                    IncludeUnlisted = inputModel.IncludeUnlisted,
                    IncludeCheckInCheckOut = inputModel.IncludeCheckInCheckOut,
                    TermIds = inputModel.MarkingPeriodIds?.ToArray(),
                };
            if (inputModel.StudentIds == null)
            {
                var isEnrolled = inputModel.DisplayWithdrawnStudents ? (bool?) null : true;
                var students = ServiceLocator.StudentService.GetClassStudents(inputModel.ClassId, gp.MarkingPeriodRef, isEnrolled);
                ps.StudentIds = students.Select(x => x.Id).ToArray();
            }
            else 
                ps.StudentIds = inputModel.StudentIds.ToArray();
            return ConnectorLocator.ReportConnector.AttendanceProfileReport(ps);
        }


        public byte[] GetGradeVerificationReport(GradeVerificationInputModel inputModel)
        {
            var gp = ServiceLocator.GradingPeriodService.GetGradingPeriodById(inputModel.GradingPeriodId);
            var ps = new GradeVerificationReportParams
                {
                    AcadSessionId = gp.SchoolYearRef,
                    GradeType = inputModel.GradeType,
                    SectionId = inputModel.ClassId,
                    GradedItemIds = inputModel.GradedItemId?.ToArray(),
                    GradingPeriodIds = inputModel.GradingPeriodIds?.ToArray() ?? new []{ gp.Id },
                    IncludeComments = inputModel.IncludeCommentsAndLegend,
                    IncludeSignature = inputModel.IncludeSignature,
                    IncludeNotes = inputModel.IncludeNotes,
                    IncludeWithdrawn = inputModel.IncludeWithdrawn,
                    IdToPrint = inputModel.IdToPrint,
                    StudentOrder = inputModel.StudentOrder,
                    StudentIds = inputModel.StudentIds?.ToArray()
                };
            return ConnectorLocator.ReportConnector.GradeVerificationReport(ps);
        }


        public byte[] GetSeatingChartReport(SeatingChartReportInputModel inputModel)
        {
            var gp = ServiceLocator.GradingPeriodService.GetGradingPeriodById(inputModel.GradingPeriodId);
            var c = ServiceLocator.ClassService.GetById(inputModel.ClassId);
            var ps = new SeatingChartReportPrams
                {
                    AcadSessionId = gp.SchoolYearRef,
                    SectionId = c.Id,
                    TermId = gp.MarkingPeriodRef,
                    IncludeStudentPhoto = inputModel.DisplayStudentPhoto
                };
            return ConnectorLocator.ReportConnector.SeatingChartReport(ps);
        }
        
        public byte[] GetLessonPlanReport(LessonPlanReportInputModel inputModel)
        {
            var gp = ServiceLocator.GradingPeriodService.GetGradingPeriodById(inputModel.GradingPeriodId);
            var ps = new LessonPlanReportParams
                {
                    AcadSessionId = gp.SchoolYearRef,
                    StartDate = inputModel.StartDate,
                    EndDate = inputModel.EndDate,
                    IncludeActivities = inputModel.IncludeAnnouncements,
                    IncludeStandards = inputModel.IncludeStandards,
                    PublicPrivateText = inputModel.PublicPrivateText,
                    SectionId = inputModel.ClassId,
                    SortActivities = inputModel.SortItems,
                    SortSections = inputModel.SortClasses,
                    ActivityAttributeIds = inputModel.AnnouncementAttributes?.ToArray(),
                    ActivityCategoryIds = inputModel.AnnouncementTypes?.ToArray(),
                    MaxCount = inputModel.MaxCount
                };
            return ConnectorLocator.ReportConnector.LessonPlanReport(ps);
        }

        public byte[] GetStudentComprehensiveReport(int studentId, int gradingPeriodId)
        {
            var syId = ServiceLocator.SchoolYearService.GetCurrentSchoolYear().Id;
            var ps = new StudentComprehensiveProgressParams
                {
                    AcadSessionId = syId,
                    GradingPeriodIds = new[] { gradingPeriodId }
                };
            return ConnectorLocator.ReportConnector.StudentComprehensiveProgressReport(studentId, ps);
        }

        public byte[] GetFeedReport(FeedReportInputModel inputModel, string path)
        {
            if(inputModel.Settings == null) 
                throw new ChalkableException("Empty report settings parameter");

            ValidateDateRange(inputModel.Settings.StartDate, inputModel.Settings.EndDate);

            IReportHandler<FeedReportInputModel> handler;
            if (inputModel.Settings.IncludeDetails)
                handler = new FeedDetailsReportHandler();
            else
                handler = new ShortFeedReportHandler();

            var format = inputModel.FormatTyped ?? ReportingFormat.Pdf;
            var dataSet = handler.PrepareDataSource(inputModel, format, ServiceLocator, ServiceLocator.ServiceLocatorMaster);
            var definition = Path.Combine(path, handler.ReportDefinitionFile);
            if (!File.Exists(definition))
                throw new ChalkableException(string.Format(ChlkResources.ERR_REPORT_DEFINITION_FILE_NOT_FOUND, definition));

            return new DefaultRenderer().Render(dataSet, definition, format, null);
        }

        public byte[] GetReportCards(ReportCardsInputModel inputModel)
        {
            BaseSecurity.EnsureDistrictAdmin(Context);

            throw new NotImplementedException();
        }

        public FeedReportSettingsInfo GetFeedReportSettings()
        {
            Trace.Assert(Context.PersonId.HasValue);
            Trace.Assert(Context.SchoolYearId.HasValue);

            var settings = ServiceLocator.PersonSettingService.GetSettingsForPerson(Context.PersonId.Value,
                Context.SchoolYearId.Value, new List<string>
                {
                    PersonSetting.FEED_REPORT_START_DATE,
                    PersonSetting.FEED_REPORT_END_DATE,
                    PersonSetting.FEED_REPORT_INCLUDE_DETAILS,
                    PersonSetting.FEED_REPORT_INCLUDE_HIDDEN_ACTIVITIES,
                    PersonSetting.FEED_REPORT_INCLUDE_HIDDEN_ATTRIBUTES,
                    PersonSetting.FEED_REPORT_LP_ONLY,
                    PersonSetting.FEED_REPORT_INCLUDE_ATTACHMENTS
                });
            return new FeedReportSettingsInfo(settings);
        }

        public void SetFeedReportSettings(FeedReportSettingsInfo settings)
        {
            Trace.Assert(Context.PersonId.HasValue);
            Trace.Assert(Context.SchoolYearId.HasValue);
            ValidateDateRange(settings.StartDate, settings.EndDate);
            ServiceLocator.PersonSettingService.SetSettingsForPerson(Context.PersonId.Value, Context.SchoolYearId.Value, settings.ToDictionary());
        }

        private static void ValidateDateRange(DateTime? startDate, DateTime? endDate)
        {
            if (startDate.HasValue && endDate.HasValue && startDate > endDate)
                throw new ChalkableException("Invalid date range. Start date is greater than end date");
        }
    }

}
