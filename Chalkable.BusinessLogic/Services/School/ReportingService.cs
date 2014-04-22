using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chalkable.Common;
using Chalkable.StiConnector.Connectors.Model;

namespace Chalkable.BusinessLogic.Services.School
{
    public interface IReportingService
    {
        byte[] GetGradebookReport(GradebookReportInputModel gradebookReportInput);
        byte[] GetWorksheetReport(WorksheetReportInputModel worksheetReportInput);
    }

    public class ReportingService : SisConnectedService, IReportingService
    {
        public ReportingService(IServiceLocatorSchool serviceLocator) : base(serviceLocator)
        {
        }

        public byte[] GetGradebookReport(GradebookReportInputModel inputModel)
        {
            var stiModel = new GradebookReportParams
                {
                    StartDate = inputModel.StartDate,
                    EndDate = inputModel.EndDate,
                    DisplayStudentAverage = inputModel.DisplayLetterGrade,
                    DisplayLetterGrade = inputModel.DisplayLetterGrade,
                    DisplayTotalPoints = inputModel.DisplayTotalPoints,
                    IncludeWithdrawnStudents = inputModel.IncludeNonGradedActivities,
                    SuppressStudentName = inputModel.SuppressStudentName,
                    OrderBy = inputModel.OrderBy,
                    GroupBy = inputModel.GroupBy,
                    IdToPrint = inputModel.IdToPrint,
                    ReportType = inputModel.ReportType,
                    GradingPeriodId = inputModel.GradingPeriodId,
                    SectionId = inputModel.ClassId
                };
            if (CoreRoles.TEACHER_ROLE == Context.Role)
                stiModel.StaffId = Context.UserLocalId;
            return ConnectorLocator.ReportConnector.GradebookReport(stiModel);
        }

        public byte[] GetWorksheetReport(WorksheetReportInputModel inputModel)
        {
            
            var anns = ServiceLocator.AnnouncementService.GetAnnouncements(inputModel.StartDate, inputModel.EndDate);
            anns = anns.Where(x => x.SisActivityId.HasValue && inputModel.AnnouncementIds.Contains(x.Id)).ToList();
            var activityIds = anns.Select(x => x.SisActivityId.Value).ToArray();
            var students = ServiceLocator.ClassService.GetStudents(inputModel.ClassId);
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
                stiModel.StaffId = Context.UserLocalId;
            var gp = ServiceLocator.GradingPeriodService.GetGradingPeriodById(inputModel.GradingPeriodId);
            stiModel.AcadSessionId = gp.SchoolYearRef;
            return ConnectorLocator.ReportConnector.WorksheetReport(stiModel);
        }
    }

    public class BaseReportInputModel
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int IdToPrint { get; set; }
        public ReportingFormat FormatTyped
        {
            get { return (ReportingFormat)Format; }
            set { Format = (int) value; }
        }
        public int Format { get; set; }
        public int GradingPeriodId { get; set; }
        public int ClassId { get; set; }
    }

    public class GradebookReportInputModel : BaseReportInputModel
    {
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
