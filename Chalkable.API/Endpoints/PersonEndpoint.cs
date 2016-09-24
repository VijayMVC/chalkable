using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Chalkable.API.Models;

namespace Chalkable.API.Endpoints
{
    public class PersonEndpoint : Base
    {
        public PersonEndpoint(IConnector connector) : base(connector)
        {
        }

        public async Task<SchoolPerson> GetMe()
        {
            return await Connector.Get<SchoolPerson>("/Person/Me.json");
        }

        public async Task<StudentInfo> GetStudentInfo(int personId)
        {
            return await Connector.Get<StudentInfo>($"/Student/Info.json?personId={personId}");
        }

        public async Task<IList<StudentInfo>> GetStudents(string filter, bool? myStudentsOnly = null, int start = 0,
            int count = int.MaxValue,
            int? classId = null, bool? byLastName = null, int? markingPeriodId = null, bool? enrolledOnly = null)
        {
            var filters = new Dictionary<string, string>();

            if (!string.IsNullOrWhiteSpace(filter))
                filters.Add("filter", filter.Trim());

            if (myStudentsOnly.HasValue)
                filters.Add("myStudentsOnly", myStudentsOnly.Value ? "true" : "false");
            if (byLastName.HasValue)
                filters.Add("byLastName", byLastName.Value ? "true" : "false");
            if (enrolledOnly.HasValue)
                filters.Add("enrolledOnly", enrolledOnly.Value ? "true" : "false");

            if (classId.HasValue)
                filters.Add("classId", classId.Value.ToString());
            if (markingPeriodId.HasValue)
                filters.Add("markingPeriodId", markingPeriodId.Value.ToString());

            filters.Add("start", start.ToString());
            filters.Add("count", count.ToString());

            var f = filters.Select(x => Uri.EscapeDataString(x.Key) + '=' + Uri.EscapeDataString(x.Value))
                .Aggregate((a, x) => a != null ? a + '&' + x : x);

            return await Connector.Get<IList<StudentInfo>>($"/Student/GetStudents.json?{f}");
        }

        public async Task<IList<SchoolInfo>> GetDistrictSchools()
        {
            return await Connector.Get<IList<SchoolInfo>>($"/School/LocalSchools.json");
        }

        public async Task<IList<LimitedEnglish>> GetLimitedEnglishList()
        {
            return await Connector.Get<IList<LimitedEnglish>>($"/LimitedEnglish/List.json");
        } 
    }
}