using System;
using System.Collections.Generic;
using System.Linq;
using Chalkable.BusinessLogic.Model;
using Chalkable.Common;
using Chalkable.StiConnector.Connectors.Model;

namespace Chalkable.BusinessLogic.Services.School
{
    public interface IReportingService
    {
        byte[] GetGradebookReport(GradebookReportInputModel gradebookReportInput);
        byte[] GetWorksheetReport(WorksheetReportInputModel worksheetReportInput);
        byte[] GetProgressReport(ProgressReportInputModel inputModel);

        byte[] GetComprehensiveProgressReport(ComprehensiveProgressInputModel comprehensiveProgressInput);
        byte[] GetMissingAssignmentsReport(MissingAssignmentsInputModel missingAssignmentsInput);
        
        IList<StudentCommentInfo> GetProgressReportComments(int classId, int gradingPeriodId);
        void SetProgressReportComment(int classId, int studentId, int gradingPeriodId, string comment);
    }

    public class ReportingService : SisConnectedService, IReportingService
    {
        public ReportingService(IServiceLocatorSchool serviceLocator) : base(serviceLocator)
        {
        }

        public byte[] GetGradebookReport(GradebookReportInputModel inputModel)
        {
            var gp = ServiceLocator.GradingPeriodService.GetGradingPeriodById(inputModel.GradingPeriodId);
            var students = ServiceLocator.StudentService.GetClassStudents(inputModel.ClassId, gp.MarkingPeriodRef);
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
                    SectionId = inputModel.ClassId,
                    StudentIds = students.Select(x=>x.Id).ToArray()
                };
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
            var students = ServiceLocator.StudentService.GetClassStudents(inputModel.ClassId, gp.MarkingPeriodRef);
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
                    StudentIds = students.Select(x=>x.Id).ToArray()
                };
            if (CoreRoles.TEACHER_ROLE == Context.Role)
                stiModel.StaffId = Context.PersonId;
            stiModel.AcadSessionId = gp.SchoolYearRef;
            return ConnectorLocator.ReportConnector.WorksheetReport(stiModel);
        }


        public byte[] GetProgressReport(ProgressReportInputModel inputModel)
        {
            var gp = ServiceLocator.GradingPeriodService.GetGradingPeriodById(inputModel.GradingPeriodId);
            int[] studentIds = new int[0];
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
                studentIds = inputModel.StudentComments.Select(x => x.StudentId).ToArray();
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
                    StudentIds = studentIds
                };
            return ConnectorLocator.ReportConnector.ProgressReport(stiModel);
        }


        public IList<StudentCommentInfo> GetProgressReportComments(int classId, int gradingPeriodId)
        {
            var inowReportComments = ConnectorLocator.ReportConnector.GetProgressReportComments(classId, gradingPeriodId);
            var gp = ServiceLocator.GradingPeriodService.GetGradingPeriodById(gradingPeriodId);
            int markingPeriodId = gp.MarkingPeriodRef;
            var students = ServiceLocator.StudentService.GetClassStudents(classId, markingPeriodId);
            var res = new List<StudentCommentInfo>();
            foreach (var inowStudentComment in inowReportComments)
            {
                var student = students.FirstOrDefault(x => x.Id == inowStudentComment.StudentId);
                if(student == null) continue;
                res.Add(new StudentCommentInfo
                    {
                        Student = student,
                        Comment = inowStudentComment.Comment
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


        public byte[] GetComprehensiveProgressReport(ComprehensiveProgressInputModel comprehensiveProgressInput)
        {
            bool includeWithdrawn = comprehensiveProgressInput.IncludeWithdrawn;
            var classPersons = ServiceLocator.ClassService.GetClassPersons(null, comprehensiveProgressInput.ClassId, includeWithdrawn ? (bool?)null : true, null).ToList();
            var defaultGp = ServiceLocator.GradingPeriodService.GetGradingPeriodById(comprehensiveProgressInput.GradingPeriodId);
            var stiModel = new ComprehensiveProgressParams
                {
                    EndDate = comprehensiveProgressInput.EndDate,
                    StartDate = comprehensiveProgressInput.StartDate,
                    AbsenceReasonIds = comprehensiveProgressInput.AbsenceReasonIds.ToArray(),
                    IdToPrint = comprehensiveProgressInput.IdToPrint,
                    AcadSessionId = defaultGp.SchoolYearRef,
                    GradingPeriodIds = comprehensiveProgressInput.GradingPeriodIds.ToArray(),
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
                    StudentIds = classPersons.Select(cp=>cp.PersonRef).ToArray(),
                    StaffId = Context.PersonId,
                    IncludePicture = comprehensiveProgressInput.IncludePicture,
                    IncludeWithdrawn = comprehensiveProgressInput.IncludeWithdrawn
                };
            return ConnectorLocator.ReportConnector.ComprehensiveProgressReport(stiModel);
        }

        public byte[] GetMissingAssignmentsReport(MissingAssignmentsInputModel missingAssignmentsInput)
        {
            var gradingPeriod = ServiceLocator.GradingPeriodService.GetGradingPeriodById(missingAssignmentsInput.GradingPeriodId);
            var stiModel = new MissingAssignmentsParams
                {
                    AcadSessionId = gradingPeriod.SchoolYearRef,
                    AlternateScoreIds = missingAssignmentsInput.AlternateScoreIds.ToArray(),
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
            return ConnectorLocator.ReportConnector.MissingAssignmentsReport(stiModel);
        }
    }

    public class BaseReportInputModel
    {
        public virtual int IdToPrint { get; set; }
        public ReportingFormat FormatTyped
        {
            get { return (ReportingFormat)Format; }
            set { Format = (int) value; }
        }
        public virtual int Format { get; set; }
        public virtual int GradingPeriodId { get; set; }
        public virtual int ClassId { get; set; }
    }

    public class GradebookReportInputModel : BaseReportInputModel
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public int ReportType { get; set; }
        public int OrderBy { get; set; }
        public int GroupBy { get; set; }

        public bool IncludeNonGradedActivities { get; set; }
        public bool IncludeWithdrawnStudents { get; set; }

        public bool DisplayLetterGrade { get; set; }
        public bool DisplayStudentAverage { get; set; }
        public bool DisplayTotalPoints { get; set; }
        public bool SuppressStudentName { get; set; }
    }

    public class WorksheetReportInputModel : BaseReportInputModel
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public IntList AnnouncementIds { get; set; }
        public string Title1 { get; set; }
        public string Title2 { get; set; }
        public string Title3 { get; set; }
        public string Title4 { get; set; }
        public string Title5 { get; set; }
        public string Header { get; set; }
        public bool PrintAverage { get; set; }
        public bool PrintLetterGrade { get; set; }
        public bool PrintScores { get; set; }
        public bool PrintStudent { get; set; }
    }

    public class ProgressReportInputModel : BaseReportInputModel
    {
        public IntList AbsenceReasonIds { get; set; }
        public bool AdditionalMailings { get; set; }
        
        public int DailyAttendanceDisplayMethod { get; set; }
        public bool DisplayCategoryAverages { get; set; }
        public bool DisplayClassAverages { get; set; }
        public bool DisplayLetterGrade { get; set; }
        public bool DisplayPeriodAttendance { get; set; }
        public bool DisplaySignatureLine { get; set; }
        public bool DisplayStudentComments { get; set; }
        public bool DisplayStudentMailingAddress { get; set; }
        public bool DisplayTotalPoints { get; set; }
        public bool GoGreen { get; set; }
        
        public decimal? MaxCategoryClassAverage { get; set; }
        public decimal? MaxStandardAverage { get; set; }
        public decimal? MinCategoryClassAverage { get; set; }
        public decimal? MinStandardAverage { get; set; }
        public bool PrintFromHomePortal { get; set; }

        public string ClassComment { get; set; }
        public IList<StudentCommentInputModel> StudentComments { get; set; }
    }

    public class MissingAssignmentsInputModel: BaseReportInputModel
    {
        public IntList AlternateScoreIds { get; set; }
        public bool AlternateScoresOnly { get; set; }
        public bool ConsiderZerosAsMissingGrades { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime StartDate { get; set; }
      
        public bool IncludeWithdrawn { get; set; }
        public bool OnePerPage { get; set; }
        public int OrderBy { get; set; }
        public bool SuppressStudentName { get; set; }
    }

    public class ComprehensiveProgressInputModel : BaseReportInputModel
    {
        public IntList GradingPeriodIds { get; set; }
        public IntList AbsenceReasonIds { get; set; }

        public DateTime? EndDate { get; set; }
        public DateTime? StartDate { get; set; }
        
        public decimal? MaxStandardAverage { get; set; }
        public decimal? MinStandardAverage { get; set; }

        public bool AdditionalMailings { get; set; }
        public bool ClassAverageOnly { get; set; }
        public bool DisplayCategoryAverages { get; set; }
        public bool DisplayClassAverage { get; set; }
        public int DailyAttendanceDisplayMethod { get; set; }
        public bool DisplayPeriodAttendance { get; set; }
        public bool DisplaySignatureLine { get; set; }
        public bool DisplayStudentComment { get; set; }
        public bool DisplayStudentMailingAddress { get; set; }
        public bool DisplayTotalPoints { get; set; }
        public bool IncludePicture { get; set; }
        public bool IncludeWithdrawn { get; set; }
        
        
        public int? StudentFilterId { get; set; }
        public bool GoGreen { get; set; }
        public int OrderBy { get; set; }
        public bool WindowEnvelope { get; set; }


        public override int GradingPeriodId
        {
            get
            {
                if (GradingPeriodIds != null && GradingPeriodIds.Count > 0)
                    return GradingPeriodIds.First();
                return base.GradingPeriodId;
            }
            set
            {
                base.GradingPeriodId = value;
            }
        }
    }

    public class StudentCommentInputModel
    {
        public int StudentId { get; set; }
        public string Comment { get; set; }
    }

    public enum ReportingFormat
    {
        Pdf = 0,
        Csv = 1,
        Excel = 2,
        Html = 3,
        Tiff = 4,
        Xml = 5
    }
    public static class ReportingFormatExtension
    {
        private const string EXT_XLS = "xls";
        private const string EXT_PDF = "pdf";
        private const string EXT_TIFF = "tiff";
        private const string EXT_CSV = "csv";
        private const string EXT_XML = "xml";
        private const string EXT_HTML = "html";
        
        public static string AsFileExtension(this ReportingFormat format)
        {
            switch (format)
            {
                case ReportingFormat.Pdf: return EXT_PDF;
                case ReportingFormat.Csv: return EXT_CSV;
                case ReportingFormat.Excel: return EXT_XLS;
                case ReportingFormat.Tiff: return EXT_TIFF;
                case ReportingFormat.Html: return EXT_HTML;
                case ReportingFormat.Xml: return EXT_XML;
                default:
                    throw new Exception(ChlkResources.ERR_INVALID_REPORT_FORMAT);
            }
        }
    }
}
