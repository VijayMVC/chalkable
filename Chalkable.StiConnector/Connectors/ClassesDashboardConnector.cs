using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography.Xml;
using Chalkable.Common;
using Chalkable.StiConnector.Connectors.Model;

namespace Chalkable.StiConnector.Connectors
{
    public enum SectionSummarySortOption
    {
        SectionAscending = 0,  //Not available on teacher route
        SectionDescending = 1, //Not available on teacher route
        TeacherAscending = 2,
        TeacherDescending = 3,
        StudentsAscending = 4,
        StudentsDescending = 5,
        AttendanceAscending = 6,
        AttendanceDescending = 7,
        DisciplineAscending = 8,
        DisciplineDescending = 9,
        GradesAscending = 10,
        GradesDescending = 11
    }

    public class ClassesDashboardConnector : ConnectorBase
    {
        public ClassesDashboardConnector(ConnectorLocator locator) : base(locator)
        {
        }

        
        public IList<SectionSummary> GetSectionsSummaries(int acadSessionId, DateTime tillDate, int start, int end, string filter, SectionSummarySortOption sortType)
        {

            EnsureApiVersion("7.1.6.19573");
            
            var param = new NameValueCollection
            {
                ["orderBy"] = ((int)sortType).ToString(),
                ["start"] = start.ToString(),
                ["end"] = end.ToString()
            };

            if(!string.IsNullOrWhiteSpace(filter))
                param.Add("filter", filter);

            var res = Call<IList<SectionSummary>>($"{BaseUrl}chalkable/{acadSessionId}/classes/dashboard/sections/{tillDate.ToString(Constants.DATE_FORMAT, CultureInfo.InvariantCulture)}", param);
            return res;
        }


        public IList<SchoolSummary> GetSchoolsSummaries(DateTime tillDate)
        {
            EnsureApiVersion("7.1.6.19573");
            
            return Call<IList<SchoolSummary>>($"{BaseUrl}chalkable/classes/dashboard/schools/{tillDate.ToString(Constants.DATE_FORMAT, CultureInfo.InvariantCulture)}");
        }


        public IList<TeacherSummary> GetTeachersSummaries(int acadSessionId, DateTime tillDate, int start, int end, string filter, SectionSummarySortOption sortType)
        {
            EnsureApiVersion("7.1.6.19573");

            var param = new NameValueCollection()
            {
                ["orderBy"] = ((int)sortType).ToString(),
                ["start"] = start.ToString(),
                ["end"] = end.ToString(),
            };

            if (!string.IsNullOrWhiteSpace(filter))
                param.Add("filter", filter);

            return Call<IList<TeacherSummary>>($"{BaseUrl}chalkable/{acadSessionId}/classes/dashboard/teachers/{tillDate.ToString(Constants.DATE_FORMAT, CultureInfo.InvariantCulture)}", param);
        }

        public IList<SectionSummary> GetSectionSummariesByTeacher(int acadSessionId, int teacherId, DateTime tillDate, int start, int end, string filter, SectionSummarySortOption sortType)
        {
            EnsureApiVersion("7.1.0.0");

            var param = new NameValueCollection
            {
                ["orderBy"] = ((int) sortType).ToString(),
                ["start"] = start.ToString(),
                ["end"] = end.ToString()
            };

            if (!string.IsNullOrWhiteSpace(filter))
                param.Add("filter", filter);

            return Call<IList<SectionSummary>>($"{BaseUrl}chalkable/{acadSessionId}/classes/dashboard/teachers/{teacherId}/sections/{tillDate.ToString(Constants.DATE_FORMAT, CultureInfo.InvariantCulture)}", param);
        }

        public IList<SectionSummaryForStudent> GetSectionSummaryForStudent(int acadSessionId, int studentId, int gradingPeriodId)
        {
            EnsureApiVersion("7.3.11.21573");
            return Call<IList<SectionSummaryForStudent>>($"{BaseUrl}chalkable/{acadSessionId}/classes/dashboard/students/{studentId}/sections/{gradingPeriodId}");
        }
    }
}
