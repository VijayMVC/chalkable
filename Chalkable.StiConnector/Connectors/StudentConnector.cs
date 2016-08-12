using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.IO;
using System.Net;
using System.Runtime.Remoting.Contexts;
using System.Threading.Tasks;
using Chalkable.Common;
using Chalkable.StiConnector.Connectors.Model;
using Chalkable.StiConnector.Connectors.Model.Attendances;

namespace Chalkable.StiConnector.Connectors
{
    public class StudentConnector : ConnectorBase
    {
        private const string GRADING_PERIOD_ID_PARAM = "gradingPeriodId";
        private const string START_DATE_PARAM = "startDate";
        private const string END_DATE_PARAM = "endDate";
        private const string DATE_PARAM = "date";
        private const string DATE_TIME_FORMAT = "yyyy-MM-ddThh:mm:ss";

        public StudentConnector(ConnectorLocator locator) : base(locator)
        {
        }
        
        public async Task<IList<StudentCondition>> GetStudentConditions(int studentId)
        {
            var url = $"{BaseUrl}chalkable/students/{studentId}/conditions";
            return await CallAsync<IList<StudentCondition>>(url);
        }
        
        public async Task<NowDashboard> GetStudentNowDashboard(int acadSessionId, int studentId, DateTime nowSchoolTime)
        {
            var nvc = new NameValueCollection
            {
                [DATE_PARAM] = nowSchoolTime.ToString(DATE_TIME_FORMAT, CultureInfo.InvariantCulture)
            };
            var url = $"{BaseUrl}chalkable/{acadSessionId}/students/{studentId}/dashboard/now";
            return await CallAsync<NowDashboard>(url, nvc);
        }
        
        public async Task<StudentExplorerDashboard> GetStudentExplorerDashboard(int acadSessionId, int studentId, DateTime? date = null)
        {
            var nvc = new NameValueCollection();
            if(date.HasValue)
                nvc.Add(DATE_PARAM, date.Value.ToString(Constants.DATE_FORMAT));
            var url = $"{BaseUrl}chalkable/{acadSessionId}/students/{studentId}/dashboard/explorer";
            return await CallAsync<StudentExplorerDashboard>(url, nvc);
        }
        
        public StudentAttendanceSummaryDashboard GetStudentAttendanceSummary(int studentId, int acadSessionId, int? termId)
        {
            var nvc = new NameValueCollection();
            if (termId.HasValue)
                nvc.Add(GRADING_PERIOD_ID_PARAM, termId.Value.ToString());
            var url = $"{BaseUrl}chalkable/{acadSessionId}/students/{studentId}/dashboard/attendance/summary";
            return Call<StudentAttendanceSummaryDashboard>(url, nvc);
        }
        
        public StudentAttendanceDetailDashboard GetStudentAttendanceDetailDashboard(int studentId, int acadSessionId, DateTime? startDate, DateTime? endDate)
        {
            var nvc = new NameValueCollection();
            if (startDate.HasValue)
                nvc.Add(START_DATE_PARAM, startDate.Value.ToString(Constants.DATE_FORMAT));
            if (endDate.HasValue)
                nvc.Add(END_DATE_PARAM, endDate.Value.ToString(Constants.DATE_FORMAT));
            var url = $"{BaseUrl}chalkable/{acadSessionId}/students/{studentId}/dashboard/attendance/detail";
            return Call<StudentAttendanceDetailDashboard>(url, nvc);
        }
        
        public DisciplineSummaryDashboard GetStudentDisciplineSummary(int studentId, int acadSessionId, int? gradingPeriodId)
        {
            var nvc = new NameValueCollection();
            if (gradingPeriodId.HasValue)
                nvc.Add(GRADING_PERIOD_ID_PARAM, gradingPeriodId.Value.ToString());
            var url = $"{BaseUrl}chalkable/{acadSessionId}/students/{studentId}/dashboard/discipline/summary";
            return Call<DisciplineSummaryDashboard>(url, nvc);
        }
        
        public DisciplineDetailDashboard GetStudentDisciplineDetailDashboard(int studentId, int acadSessionId, DateTime? startDate, DateTime? endDate)
        {
            var nvc = new NameValueCollection();
            if (startDate.HasValue)
                nvc.Add(START_DATE_PARAM, startDate.Value.ToString(Constants.DATE_FORMAT));
            if (endDate.HasValue)
                nvc.Add(END_DATE_PARAM, endDate.Value.ToString(Constants.DATE_FORMAT));
            var url = $"{BaseUrl}chalkable/{acadSessionId}/students/{studentId}/dashboard/discipline/detail";
            return Call<DisciplineDetailDashboard>(url, nvc);
        }

        public async Task<IList<StudentHealthForm>> GetStudentHealthForms(int studentId)
        {
            return await CallAsync<IList<StudentHealthForm>>($"{BaseUrl}health/students/{studentId}/healthForms");
        }
        public async Task SetStudentHealthFormReadReceipts(int studentId, int studentHealthFormId, StudentHealthFormReadReceipt formReadReceipts)
        {
           await PostAsync<Object>($"{BaseUrl}health/students/{studentId}/healthForms/{studentHealthFormId}/readReceipts", formReadReceipts);
        }

        public async Task<bool> HasHealthLicenses()
        {
            try
            {
                await InitWebClient().DownloadDataTaskAsync($"{BaseUrl}moduleLicenses/Health");
                return true;
            }
            catch (WebException ex)
            {
                if (ex.Response is HttpWebResponse)
                {
                    HttpStatusCode status = (ex.Response as HttpWebResponse).StatusCode;
                    if (status == HttpStatusCode.NotFound)
                        return false;
                }
                var reader = new StreamReader(ex.Response.GetResponseStream());
                var msg = reader.ReadToEnd();
                throw new Exception(msg);
            }
        }

        public byte[] GetStudentHealthFormDocument(int studentId, int healthFormId)
        {
            return Download($"{BaseUrl}health/students/{studentId}/healthForms/{healthFormId}");
        }
    }
}
