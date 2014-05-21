using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chalkable.Common;
using Chalkable.StiConnector.Connectors.Model;

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
            return Call<SectionAttendance>(string.Format("{0}Chalkable/sections/{1}/attendance/{2}", BaseUrl, sectionId, date.ToString(Constants.DATE_FORMAT)));
        }

        public void SetSectionAttendance(int acadSessionId, DateTime date, int sectionId, SectionAttendance sectionAttendance)
        {
            string url = string.Format("{0}Chalkable/sections/{1}/attendance/{2}", BaseUrl, sectionId, date.ToString(Constants.DATE_FORMAT));
            Post(url, sectionAttendance);
        }

        public IList<SectionAttendanceSummary> GetSectionAttendanceSummary(IList<int> sectionIds, DateTime start, DateTime end)
        {
            var parmeters = new NameValueCollection
                {
                    {"start", start.ToString(Constants.DATE_FORMAT)},
                    {"end", end.ToString(Constants.DATE_FORMAT)}
                };
            for(int i = 0; i < sectionIds.Count; i++)
                parmeters.Add(string.Format("sectionIds[{0}]", i.ToString()), sectionIds[i].ToString());
            string url = string.Format("{0}chalkable/attendance/summary");
            return Call<IList<SectionAttendanceSummary>>(url, parmeters);
        } 
    }
}
