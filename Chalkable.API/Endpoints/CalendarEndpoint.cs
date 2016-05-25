using System;
using System.Threading.Tasks;

namespace Chalkable.API.Endpoints
{
    public class CalendarEndpoint : Base
    {
        public CalendarEndpoint(IConnector connector) : base(connector)
        {
        }

        public async Task<bool> IsSchoolDay(DateTime? dateTime)
        {
            var url = "/Calendar/IsSchoolDay.json";
            return await Connector.Get<bool>($"{url}?dateTime={dateTime}");
        }

        public async Task<int> SchoolDaysCount(DateTime fromDate, DateTime toDate)
        {
            var url = "/Calendar/SchoolDaysCount.json";
            return await Connector.Get<int>($"{url}?fromDate={fromDate}&toDate={toDate}");
        }
    } 
}
