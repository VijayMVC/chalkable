using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
            var url = "/Calendar/IsSchoolDay";
            return await Connector.Get<bool>($"{url}?dateTime={dateTime}");
        }
    } 
}
