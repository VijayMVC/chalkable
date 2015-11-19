using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.Runtime.Remoting.Contexts;
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

        public IList<StudentCondition> GetStudentConditions(int studentId)
        {
            var url = string.Format("{0}chalkable/students/{1}/conditions", BaseUrl, studentId);
            return Call<IList<StudentCondition>>(url);
        }

        public NowDashboard GetStudentNowDashboard(int acadSessionId, int studentId, DateTime nowSchoolTime)
        {
            var nvc = new NameValueCollection
            {
                [DATE_PARAM] = nowSchoolTime.ToString(DATE_TIME_FORMAT, CultureInfo.InvariantCulture)
            };
            var url = string.Format("{0}chalkable/{1}/students/{2}/dashboard/now", BaseUrl, acadSessionId, studentId);
            return Call<NowDashboard>(url, nvc);
        }

        public StudentExplorerDashboard GetStudentExplorerDashboard(int acadSessionId, int studentId, DateTime? date = null)
        {
            var nvc = new NameValueCollection();
            if(date.HasValue)
                nvc.Add(DATE_PARAM, date.Value.ToString(Constants.DATE_FORMAT));
            var url = string.Format("{0}chalkable/{1}/students/{2}/dashboard/explorer", BaseUrl, acadSessionId, studentId);
            return Call<StudentExplorerDashboard>(url, nvc);
        }

        public StudentAttendanceSummaryDashboard GetStudentAttendanceSummary(int studentId, int acadSessionId, int? termId)
        {
            var nvc = new NameValueCollection();
            if (termId.HasValue)
                nvc.Add(GRADING_PERIOD_ID_PARAM, termId.Value.ToString());
            var url = string.Format("{0}chalkable/{1}/students/{2}/dashboard/attendance/summary", BaseUrl, acadSessionId, studentId);
            return Call<StudentAttendanceSummaryDashboard>(url, nvc);
        }

        public StudentAttendanceDetailDashboard GetStudentAttendanceDetailDashboard(int studentId, int acadSessionId, DateTime? startDate, DateTime? endDate)
        {
            var nvc = new NameValueCollection();
            if (startDate.HasValue)
                nvc.Add(START_DATE_PARAM, startDate.Value.ToString(Constants.DATE_FORMAT));
            if (endDate.HasValue)
                nvc.Add(END_DATE_PARAM, endDate.Value.ToString(Constants.DATE_FORMAT));
            var url = string.Format("{0}chalkable/{1}/students/{2}/dashboard/attendance/detail", BaseUrl, acadSessionId, studentId);
            return Call<StudentAttendanceDetailDashboard>(url, nvc);
        }

        public DisciplineSummaryDashboard GetStudentDisciplineSummary(int studentId, int acadSessionId, int? gradingPeriodId)
        {
            var nvc = new NameValueCollection();
            if (gradingPeriodId.HasValue)
                nvc.Add(GRADING_PERIOD_ID_PARAM, gradingPeriodId.Value.ToString());
            var url = string.Format("{0}chalkable/{1}/students/{2}/dashboard/discipline/summary", BaseUrl, acadSessionId, studentId);
            return Call<DisciplineSummaryDashboard>(url, nvc);
        }

        public DisciplineDetailDashboard GetStudentDisciplineDetailDashboard(int studentId, int acadSessionId, DateTime? startDate, DateTime? endDate)
        {
            var nvc = new NameValueCollection();
            if (startDate.HasValue)
                nvc.Add(START_DATE_PARAM, startDate.Value.ToString(Constants.DATE_FORMAT));
            if (endDate.HasValue)
                nvc.Add(END_DATE_PARAM, endDate.Value.ToString(Constants.DATE_FORMAT));
            var url = string.Format("{0}chalkable/{1}/students/{2}/dashboard/discipline/detail", BaseUrl, acadSessionId, studentId);
            return Call<DisciplineDetailDashboard>(url, nvc);
        }
    }
}
