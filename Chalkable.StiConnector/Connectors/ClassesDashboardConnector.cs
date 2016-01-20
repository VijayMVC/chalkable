using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.Linq;
using Chalkable.Common;
using Chalkable.StiConnector.Connectors.Model;

namespace Chalkable.StiConnector.Connectors
{
    public class ClassesDashboardConnector : ConnectorBase
    {
        public ClassesDashboardConnector(ConnectorLocator locator) : base(locator)
        {
        }

        
        public IList<SectionSummary> GetSectionsSummaries(int acadSessionId, DateTime tillDate, int start, int end, string filter, int? teacherId)
        {

            EnsureApiVersion("7.1.6.19573");
            
            var param = new NameValueCollection
            {
                ["start"] = (teacherId.HasValue ? 1 : start).ToString(),
                ["end"] = (teacherId.HasValue ? int.MaxValue : end).ToString()
            };

            if(!string.IsNullOrWhiteSpace(filter))
                param.Add("filter", filter);

            var res = Call<IList<SectionSummary>>($"{BaseUrl}chalkable/{acadSessionId}/classes/dashboard/sections/{tillDate.ToString(Constants.DATE_FORMAT, CultureInfo.InvariantCulture)}", param);
            // remove this after providing teacherId to api call
            if (teacherId.HasValue)
                res = res.Where(x => x.TeacherId == teacherId).Skip(start).Take(end).ToList();

            return res;
        }


        public IList<SchoolSummary> GetSchoolsSummaries(DateTime tillDate)
        {
            EnsureApiVersion("7.1.6.19573");
            
            return Call<IList<SchoolSummary>>($"{BaseUrl}chalkable/classes/dashboard/schools/{tillDate.ToString(Constants.DATE_FORMAT, CultureInfo.InvariantCulture)}");
        }


        public IList<TeacherSummary> GetTeachersSummaries(int acadSessionId, DateTime tillDate, int start, int end, string filter)
        {
            EnsureApiVersion("7.1.6.19573");

            var param = new NameValueCollection()
            {
                ["start"] = start.ToString(),
                ["end"] = end.ToString(),
            };

            if (!string.IsNullOrWhiteSpace(filter))
                param.Add("filter", filter);

            return Call<IList<TeacherSummary>>($"{BaseUrl}chalkable/{acadSessionId}/classes/dashboard/teachers/{tillDate.ToString(Constants.DATE_FORMAT, CultureInfo.InvariantCulture)}", param);
        }
    }
}
