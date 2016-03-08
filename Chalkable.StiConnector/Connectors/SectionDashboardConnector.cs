using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Threading.Tasks;
using Chalkable.Common;
using Chalkable.StiConnector.Connectors.Model;
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

        
        public SectionAttendanceDetailDashboard GetAttendanceDetailDashboard(int sectionId, DateTime startDate, DateTime endDate)
        {
            var url = $"{BaseUrl}chalkable/sections/{sectionId}/dashboard/attendance/detail";
            var nvc = new NameValueCollection
                {
                    {START_DATE_PARAM, startDate.ToString(Constants.DATE_FORMAT)},
                    {END_DATE_PARAM, endDate.ToString(Constants.DATE_FORMAT)}
                };
            return Call<SectionAttendanceDetailDashboard>(url, nvc);
        }
        
        public SectionAttendanceSummaryDashboard GetAttendanceSummaryDashboard(int sectionId, int? gradingPeriodId)
        {
            var url = $"{BaseUrl}chalkable/sections/{sectionId}/dashboard/attendance/summary";
            var nvc = new NameValueCollection();
            if(gradingPeriodId.HasValue)
                nvc.Add(GRADING_PERIOD_ID_PARAM, gradingPeriodId.Value.ToString());
            return Call<SectionAttendanceSummaryDashboard>(url, nvc);
        }

        public async Task<IList<DisciplineDailySummary>> GetDisciplineSummaryDashboard(int sectionId, DateTime? startDate, DateTime? endDate)
        {
            EnsureApiVersion("7.1.6.19573");

            var url = $"{BaseUrl}chalkable/sections/{sectionId}/dashboard/discipline/summary";
            var nvc = new NameValueCollection();
            if(startDate.HasValue) 
                nvc.Add(START_DATE_PARAM, startDate.Value.ToString(Constants.DATE_FORMAT));
            if(endDate.HasValue)
                nvc.Add(END_DATE_PARAM, endDate.Value.ToString(Constants.DATE_FORMAT));
            return await CallAsync<IList<DisciplineDailySummary>>(url, nvc);
        }
        
        public async Task<IList<AttendanceDailySummary>> GetAttendanceDailySummaries(int sectionId, DateTime? startDate, DateTime? endDate)
        {
            EnsureApiVersion("7.1.6.19573");

            var nvc = new NameValueCollection();
            if(startDate.HasValue)
                nvc.Add(START_DATE_PARAM, startDate.Value.ToString(Constants.DATE_FORMAT));
            if(endDate.HasValue)
                nvc.Add(END_DATE_PARAM, endDate.Value.ToString(Constants.DATE_FORMAT));

            return await CallAsync<IList<AttendanceDailySummary>>($"{BaseUrl}chalkable/sections/{sectionId}/dashboard/attendance/summary",nvc);
        }
    }
}
