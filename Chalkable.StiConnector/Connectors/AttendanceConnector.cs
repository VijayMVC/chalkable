using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Threading.Tasks;
using Chalkable.Common;
using Chalkable.StiConnector.Connectors.Model;
using Chalkable.StiConnector.Connectors.Model.Attendances;

namespace Chalkable.StiConnector.Connectors
{
    public class AttendanceConnector : ConnectorBase
    {
        public AttendanceConnector(ConnectorLocator locator)
            : base(locator)
        {
        }
        
        public SectionAttendance GetSectionAttendance(DateTime date, int sectionId)
        {
            return Call<SectionAttendance>($"{BaseUrl}Chalkable/sections/{sectionId}/attendance/{date.ToString(Constants.DATE_FORMAT)}");
        }
        
        public void SetSectionAttendance(int acadSessionId, DateTime date, int sectionId, SectionAttendance sectionAttendance)
        {
            string url = $"{BaseUrl}Chalkable/sections/{sectionId}/attendance/{date.ToString(Constants.DATE_FORMAT)}";
            Post(url, sectionAttendance);
        }
        
        public async Task<IList<SectionAttendanceSummary>> GetSectionAttendanceSummary(IList<int> sectionIds, DateTime start, DateTime end)
        {
            var parmeters = new NameValueCollection
                {
                    {"start", start.ToString(Constants.DATE_FORMAT)},
                    {"end", end.ToString(Constants.DATE_FORMAT)}
                };
            for(int i = 0; i < sectionIds.Count; i++)
                parmeters.Add($"sectionIds[{i}]", sectionIds[i].ToString());
            string url = $"{BaseUrl}chalkable/attendance/summary";
            return await CallAsync<IList<SectionAttendanceSummary>>(url, parmeters);
        }
        
        public async Task<IList<PostedAttendance>> GetPostedAttendances(int acadSessionId, DateTime date)
        {
            string url = $"{BaseUrl}chalkable/{acadSessionId}/postedattendance/{date.ToString(Constants.DATE_FORMAT)}";
            return await CallAsync<IList<PostedAttendance>>(url);
        }
    }
}
