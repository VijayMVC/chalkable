using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chalkable.Common;
using Chalkable.Data.School.Model;
using Chalkable.StiConnector.Connectors.Model;

namespace Chalkable.StiConnector.Connectors
{
    public class ClassesDashboardConnector : ConnectorBase
    {
        public ClassesDashboardConnector(ConnectorLocator locator) : base(locator)
        {
        }

        public IList<SectionSummary> GetSectionsSummaries(int acadSessionId, DateTime tillDate, int start, int count)
        {
            var param = new NameValueCollection()
            {
                ["start"] = start.ToString(),
                ["count"] = count.ToString()

            };
            return
                Call<IList<SectionSummary>>(
                    $"{BaseUrl}chalkable/{acadSessionId}/classes/dashboard/sections/{tillDate.ToString(Constants.DATE_FORMAT, CultureInfo.InvariantCulture)}",
                    param);
        }

        public IList<SchoolSummary> GetSchoolsSummaries(DateTime tillDate, int start, int count, string filter)
        {
            var param = new NameValueCollection()
            {
                ["start"] = start.ToString(),
                ["count"] = count.ToString(),
            };

            if(!string.IsNullOrWhiteSpace(filter))
                param.Add("filter", filter);

            return Call<IList<SchoolSummary>>($"{BaseUrl}chalkable/classes/dashboard/schools/{tillDate.ToString(Constants.DATE_FORMAT, CultureInfo.InvariantCulture)}", param);
        }

        public IList<TeacherSummary> GetTeachersSummaries(int acadSessionId, DateTime tillDate, int start, int count)
        {
            var param = new NameValueCollection()
            {
                ["start"] = start.ToString(),
                ["count"] = count.ToString()

            };
            return
                Call<IList<TeacherSummary>>(
                    $"{BaseUrl}chalkable/{acadSessionId}/classes/dashboard/teachers/{tillDate.ToString(Constants.DATE_FORMAT, CultureInfo.InvariantCulture)}",
                    param);
        }
    }
}
