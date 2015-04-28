using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using Chalkable.Common;
using Chalkable.StiConnector.Connectors.Model;
using Chalkable.StiConnector.Connectors.Model.Attendances;

namespace Chalkable.StiConnector.Connectors
{
    public class StudentConnector : ConnectorBase
    {
        public StudentConnector(ConnectorLocator locator) : base(locator)
        {
        }

        public IList<StudentCondition> GetStudentConditions(int studentId)
        {
            var url = string.Format("{0}chalkable/students/{1}/conditions", BaseUrl, studentId);
            return Call<IList<StudentCondition>>(url);
        }

        public NowDashboard GetStudentNowDashboard(int acadSessionId, int studentId)
        {
            var url = string.Format("{0}chalkable/{1}/students/{2}/dashboard/now", BaseUrl, acadSessionId, studentId);
            return Call<NowDashboard>(url);
        }

        public StudentExplorerDashboard GetStudentExplorerDashboard(int acadSessionId, int studentId, DateTime? date = null)
        {
            var nvc = new NameValueCollection();
            if(date.HasValue)
                nvc.Add("date", date.Value.ToString(Constants.DATE_FORMAT));
            var url = string.Format("{0}chalkable/{1}/students/{2}/dashboard/explorer", BaseUrl, acadSessionId, studentId);
            return Call<StudentExplorerDashboard>(url, nvc);
        }

        public StudentAttendanceSummaryDashboard GetStudentAttendanceSummary(int studentId, int acadSessionId, int? termId)
        {
            var nvc = new NameValueCollection();
            if (termId.HasValue)
                nvc.Add("termId", termId.Value.ToString());
            var url = string.Format("{0}chalkable/{1}/students/{2}/dashboard/attendance/summary", BaseUrl, acadSessionId, studentId);
            return Call<StudentAttendanceSummaryDashboard>(url, nvc);
        }

        public StudentAttendanceDetailDashboard GetStudentAttendanceDetailDashboard(int studentId, int acadSessionId, DateTime? startDate, DateTime? endDate)
        {
            var nvc = new NameValueCollection();
            if (startDate.HasValue)
                nvc.Add("startDate", startDate.Value.ToString(Constants.DATE_FORMAT));
            if (endDate.HasValue)
                nvc.Add("endDate", endDate.Value.ToString(Constants.DATE_FORMAT));
            var url = string.Format("{0}chalkable/{1}/students/{2}/dashboard/attendance/detail", BaseUrl, acadSessionId, studentId);
            return Call<StudentAttendanceDetailDashboard>(url, nvc);
        }

    }
}
