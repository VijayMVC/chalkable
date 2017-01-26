using System;
using System.Globalization;
using System.Threading.Tasks;
using Chalkable.API.Helpers;

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
            var date = dateTime?.ToString(Constants.DATE_FORMAT, CultureInfo.InvariantCulture);
            return await Connector.Get<bool>($"{url}?dateTime={date}");
        }

        public async Task<int> SchoolDaysCount(DateTime fromDate, DateTime toDate)
        {
            var url = "/Calendar/SchoolDaysCount.json";
            var fromDateParam = fromDate.ToString(Constants.DATE_FORMAT, CultureInfo.InvariantCulture);
            var toDateParam = toDate.ToString(Constants.DATE_FORMAT, CultureInfo.InvariantCulture);
            return await Connector.Get<int>($"{url}?fromDate={fromDateParam}&toDate={toDateParam}");
        }
    } 
}
