﻿using System;
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


        [RequiredVersion("7.1.0.0")]
        public IList<SectionSummary> GetSectionsSummaries(int acadSessionId, DateTime tillDate, int start, int count, string filter)
        {
            var param = new NameValueCollection()
            {
                ["start"] = start.ToString(),
                ["end"] = (start+count).ToString(),
                ["search"] = string.IsNullOrWhiteSpace(filter) ? "" : filter
            };
            return Call<IList<SectionSummary>>(
                    $"{BaseUrl}chalkable/{acadSessionId}/classes/dashboard/sections/{tillDate.ToString(Constants.DATE_FORMAT, CultureInfo.InvariantCulture)}",
                    param);
        }


        [RequiredVersion("7.1.0.0")]
        public IList<SchoolSummary> GetSchoolsSummaries(DateTime tillDate, string filter)
        {
            var param = new NameValueCollection
            {
                ["search"] = string.IsNullOrWhiteSpace(filter) ? "" : filter
            };

            if(!string.IsNullOrWhiteSpace(filter))
                param.Add("filter", filter);

            return Call<IList<SchoolSummary>>($"{BaseUrl}chalkable/classes/dashboard/schools/{tillDate.ToString(Constants.DATE_FORMAT, CultureInfo.InvariantCulture)}", param);
        }


        [RequiredVersion("7.1.0.0")]
        public IList<TeacherSummary> GetTeachersSummaries(int acadSessionId, DateTime tillDate, int start, int count, string filter)
        {
            var param = new NameValueCollection()
            {
                ["start"] = start.ToString(),
                ["end"] = (start + count).ToString(),
                ["search"] = string.IsNullOrWhiteSpace(filter) ? "" : filter

            };
            return Call<IList<TeacherSummary>>($"{BaseUrl}chalkable/{acadSessionId}/classes/dashboard/teachers/{tillDate.ToString(Constants.DATE_FORMAT, CultureInfo.InvariantCulture)}", param);
        }
    }
}
