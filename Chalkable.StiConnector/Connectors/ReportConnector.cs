using System.Collections.Generic;
using System.Collections.Specialized;
using System.Threading.Tasks;
using Chalkable.StiConnector.Connectors.Model.Reports;
using Chalkable.StiConnector.Connectors.Model.Reports.ReportCards;

namespace Chalkable.StiConnector.Connectors
{
    public class ReportConnector : ConnectorBase
    {
        public ReportConnector(ConnectorLocator locator) : base(locator)
        {
        }

        public IList<StudentProgressReportComment> GetProgressReportComments(int sectionId, int? gradingPeriodId = null)
        {
            var url = string.Format(BaseUrl + "chalkable/sections/{0}/progressreportcomments", sectionId);
            var nvc = new NameValueCollection();
            if(gradingPeriodId.HasValue)
                nvc.Add("gradingPeriodId", gradingPeriodId.Value.ToString());
            return Call<IList<StudentProgressReportComment>>(url, nvc);
        }

        public void UpdateProgressReportComment(int sectionId, IList<StudentProgressReportComment> comments)
        {
            var url = string.Format(BaseUrl + "chalkable/sections/{0}/progressreportcomments", sectionId);
            Put(url, comments);
        }

        
        public byte[] ProgressReport(ProgressReportParams ps)
        {
            return Download(BaseUrl + "reports/progress", ps);
        }
        
        public byte[] GradebookReport(GradebookReportParams ps)
        {
            return Download(BaseUrl + "reports/gradebook", ps);
        }
        
        public byte[] WorksheetReport(WorksheetReportParams ps)
        {
            return Download(BaseUrl + "reports/worksheet", ps);
        }
        
        public byte[] ComprehensiveProgressReport(ComprehensiveProgressParams ps)
        {
            return Download(BaseUrl + "reports/ComprehensiveProgress", ps);
        }
        
        public byte[] StudentComprehensiveProgressReport(int studentId, StudentComprehensiveProgressParams ps)
        {
            var url = $"{BaseUrl}students/{studentId}/reports/comprehensiveprogress";
            return Download(url, ps);
        }
        
        public byte[] MissingAssignmentsReport(MissingAssignmentsParams ps)
        {
            return Download(BaseUrl + "reports/missingassignments", ps);
        }
        
        public byte[] BirthdayReport(BirthdayReportParams ps)
        {
            return Download(BaseUrl + "reports/birthday", ps);
        }
        
        public byte[] AttendnaceRegisterReport(AttendanceRegisterReportParams ps)
        {
            return Download(BaseUrl + "reports/attendanceregister", ps);
        }
        
        public byte[] AttendanceProfileReport(AttendanceProfileReportParams ps)
        {
            return Download(BaseUrl + "reports/attendanceprofile", ps);
        }
        
        public byte[] GradeVerificationReport(GradeVerificationReportParams ps)
        {
            return Download(BaseUrl + "reports/gradeverification", ps);
        }
        
        public byte[] LessonPlanReport(LessonPlanReportParams ps)
        {
            return Download(BaseUrl + "reports/lessonplan", ps);
        }
        
        public byte[] SeatingChartReport(SeatingChartReportPrams ps)
        {
            return Download(BaseUrl + "reports/seatingchart", ps);
        }

        public async Task<ReportCard> ReportCardData(ReportCardOptions options)
        {
            return await PostAsync<ReportCard, ReportCardOptions>($"{BaseUrl}reports/reportcard", options);
        }
    }
}