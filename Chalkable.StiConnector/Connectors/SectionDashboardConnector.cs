using System;
using System.Collections.Specialized;
using Chalkable.Common;
using Chalkable.StiConnector.Connectors.Model.Attendances;

namespace Chalkable.StiConnector.Connectors
{
    public class SectionDashboardConnector : ConnectorBase
    {
        private const string START_DATE_PARAM = "startDate";
        private const string END_DATE_PARAM = "endDate";
        private const string GRADING_PERIOD_ID_PARAM = "gradingPeriodId";

        public SectionDashboardConnector(ConnectorLocator locator) : base(locator)
        {
        }

        public StudentAttendanceDetailDashboard GetAttendanceDetailDashboard(int sectionId, DateTime startDate, DateTime endDate)
        {
            var url = string.Format(BaseUrl + "chalkable/sections/{0}/dashboard/attendance/datail", sectionId);
            var nvc = new NameValueCollection
                {
                    {START_DATE_PARAM, startDate.ToString(Constants.DATE_FORMAT)},
                    {END_DATE_PARAM, endDate.ToString(Constants.DATE_FORMAT)}
                };
            return Call<StudentAttendanceDetailDashboard>(url, nvc);
        }

        public SectionAttendanceSummaryDashboard GetAttendanceSummaryDashboard(int sectionId, int? gradingPeriodId)
        {
            var url = string.Format(BaseUrl + "chalkable/sections/{0}/dashboard/attendance/summary", sectionId);
            var nvc = new NameValueCollection();
            if(gradingPeriodId.HasValue)
                nvc.Add(GRADING_PERIOD_ID_PARAM, gradingPeriodId.Value.ToString());
            return Call<SectionAttendanceSummaryDashboard>(url, nvc);
        }
    }
}
