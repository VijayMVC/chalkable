using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using Chalkable.Common;
using Chalkable.StiConnector.Connectors.Model;

namespace Chalkable.StiConnector.Connectors
{
    public class ClassesDashboardConnector : ConnectorBase
    {
        public ClassesDashboardConnector(ConnectorLocator locator) : base(locator)
        {
        }

        
        public IList<SectionSummary> GetSectionsSummaries(int acadSessionId, DateTime tillDate, int start, int count, string filter)
        {

            EnsureApiVersion("7.1.6.19573");

            var param = new NameValueCollection()
            {
                ["start"] = start.ToString(),
                ["end"] = (start+count).ToString(),
                ["filter"] = string.IsNullOrWhiteSpace(filter) ? "" : filter
            };
            return Call<IList<SectionSummary>>(
                    $"{BaseUrl}chalkable/{acadSessionId}/classes/dashboard/sections/{tillDate.ToString(Constants.DATE_FORMAT, CultureInfo.InvariantCulture)}",
                    param);
        }


        public IList<SchoolSummary> GetSchoolsSummaries(DateTime tillDate, string filter)
        {
            EnsureApiVersion("7.1.6.19573");

            var param = new NameValueCollection
            {
                ["filter"] = string.IsNullOrWhiteSpace(filter) ? "" : filter
            };

            if(!string.IsNullOrWhiteSpace(filter))
                param.Add("filter", filter);

            return Call<IList<SchoolSummary>>($"{BaseUrl}chalkable/classes/dashboard/schools/{tillDate.ToString(Constants.DATE_FORMAT, CultureInfo.InvariantCulture)}", param);
        }


        public IList<TeacherSummary> GetTeachersSummaries(int acadSessionId, DateTime tillDate, int start, int count, string filter)
        {
            EnsureApiVersion("7.1.6.19573");

            var param = new NameValueCollection()
            {
                ["start"] = start.ToString(),
                ["end"] = (start + count).ToString(),
                ["filter"] = string.IsNullOrWhiteSpace(filter) ? "" : filter

            };
            return Call<IList<TeacherSummary>>($"{BaseUrl}chalkable/{acadSessionId}/classes/dashboard/teachers/{tillDate.ToString(Constants.DATE_FORMAT, CultureInfo.InvariantCulture)}", param);
        }
    }
}
