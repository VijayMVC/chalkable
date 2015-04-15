using System.Collections.Generic;
using System.Linq;
using Chalkable.BusinessLogic.Model;
using Chalkable.BusinessLogic.Model.Reports;
using Chalkable.Common;
using Chalkable.StiConnector.Connectors.Model.Reports;

namespace Chalkable.BusinessLogic.Services.School
{
    public interface IReportingService
    {
        IList<StudentCommentInfo> GetProgressReportComments(int classId, int gradingPeriodId);
        void SetProgressReportComment(int classId, int studentId, int gradingPeriodId, string comment);

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
            var inowStudentReportComments = new List<StudentProgressReportComment>
                {
                    new StudentProgressReportComment
                        {
                            Comment = comment,
                            GradingPeriodId = gradingPeriodId,
                            SectionId = classId,
                            StudentId = studentId
                        }
                };
            ConnectorLocator.ReportConnector.UpdateProgressReportComment(classId, inowStudentReportComments);
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
                var anns = ServiceLocator.AnnouncementService.GetAnnouncements(inputModel.StartDate, inputModel.EndDate);
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
            if (inputModel.StudentComments != null && inputModel.StudentComments.Count > 0)
            {
                var inowStudentProgressReportComments =
                    inputModel.StudentComments.Select(x => new StudentProgressReportComment
                        {
                            Comment = x.Comment,
                            StudentId = x.StudentId,
                            GradingPeriodId = inputModel.GradingPeriodId,
                            SectionId = inputModel.ClassId
                        }).ToList();
                ConnectorLocator.ReportConnector.UpdateProgressReportComment(inputModel.ClassId, inowStudentProgressReportComments);
            }
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
                    StudentIds = inputModel.StudentIds != null ? inputModel.StudentIds.ToArray() : null,
                };
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
                    AlternateScoreIds = missingAssignmentsInput.AlternateScoreIds != null ? missingAssignmentsInput.AlternateScoreIds.ToArray() : null,
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
                    Header = inputModel.Header,
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
                    AbsenceReasonIds = inputModel.AbsenceReasonIds != null ? inputModel.AbsenceReasonIds.ToArray() : null,
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
                    AbsenceReasons = inputModel.AbsenceReasons != null ? inputModel.AbsenceReasons.ToArray() : null,
                    AcadSessionId = gp.SchoolYearRef,
                    DisplayNote = inputModel.DisplayNote,
                    DisplayPeriodAbsences = inputModel.DisplayPeriodAbsences,
                    DisplayReasonTotals = inputModel.DisplayReasonTotals,
                    DisplayWithdrawnStudents = inputModel.DisplayWithdrawnStudents,
                    StartDate = inputModel.StartDate,
                    EndDate = inputModel.EndDate,
                    GroupBy = inputModel.GroupBy,
                    SectionId = inputModel.ClassId,
                    IdToPrint = inputModel.IdToPrint,
                    IncludeUnlisted = inputModel.IncludeUnlisted,
                    Terms = inputModel.MarkingPeriodIds != null ? inputModel.MarkingPeriodIds.ToArray() : null
                };
            if (inputModel.StudentIds == null)
            {
                var isEnrolled = inputModel.DisplayWithdrawnStudents ? (bool?)null : true;
                var students = ServiceLocator.StudentService.GetClassStudents(inputModel.ClassId, gp.MarkingPeriodRef, isEnrolled);
                ps.StudentIds = students.Select(x => x.Id).ToArray();
            }
            return ConnectorLocator.ReportConnector.AttendanceProfileReport(ps);
        }


        public byte[] GetGradeVerificationReport(GradeVerificationInputModel inputModel)
        {
            var gp = ServiceLocator.GradingPeriodService.GetGradingPeriodById(inputModel.GradingPeriodId);
            var ps = new GradeVerificationReportParams
                {
                    AcadSessionId = gp.SchoolYearRef,
                    GradeType = inputModel.GradeType,
                    DistrictName = Context.SisUrl, //TODO: ask Jonathan about this field 
                    SectionId = inputModel.ClassId,
                    SectionOrder = inputModel.ClassOrder,
                    StartSectionNumber = inputModel.StartClassNumber,
                    EndSectionNumber = inputModel.EndClassNumber,
                    GradedItemId = inputModel.GradedItemId != null ? inputModel.GradedItemId.ToArray() : null,
                    GradingPeriodId = new []{gp.Id},
                    IncludeComments = inputModel.IncludeComments,
                    IncludeLegend = inputModel.IncludeLegend,
                    IncludeSignature = inputModel.IncludeSignature,
                    IncludeNotes = inputModel.IncludeNotes,
                    IncludeWithdrawn = inputModel.IncludeWithdrawn,
                    IdToPrint = inputModel.IdToPrint,
                    NumberToDisplay = inputModel.NumberToDisplay,
                    StudentOrder = inputModel.StudentOrder,
                    StudentIds = inputModel.StudentIds != null ? inputModel.StudentIds.ToArray() : null
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
                    CourseId = c.CourseRef.HasValue ? c.CourseRef.Value : c.Id,
                    TermId = gp.MarkingPeriodRef,
                    DisplayStudentPhoto = inputModel.DisplayStudentPhoto
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
                    XMLActivityAttribute = inputModel.AnnouncementAttributes != null ? inputModel.AnnouncementAttributes.ToArray() : null,
                    XMLActivityCategory = inputModel.AnnouncementTypes != null ? inputModel.AnnouncementTypes.ToArray() : null
                };
            return ConnectorLocator.ReportConnector.LessonPlanReport(ps);
        }
    }

}
