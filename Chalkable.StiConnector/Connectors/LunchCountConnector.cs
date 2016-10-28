using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Chalkable.Common;
using Chalkable.StiConnector.Connectors.Model;

namespace Chalkable.StiConnector.Connectors
{
    public class LunchCountConnector : ConnectorBase
    {
        public LunchCountConnector(ConnectorLocator locator) : base(locator)
        {
        }

        public async Task<IList<LunchCount>> GetLunchCount(int sectionId, DateTime date)
        {
            var url = $"{BaseUrl}chalkable/sections/{sectionId}/lunchcount/{date.ToString(Constants.DATE_FORMAT)}";
            return await CallAsync<IList<LunchCount>>(url);
        }


        public void UpdateLunchCount(int sectionId, DateTime date, List<LunchCount> lunchCounts)
        {
            Post($"{BaseUrl}chalkable/sections/{sectionId}/lunchcount/{date.ToString(Constants.DATE_FORMAT)}", lunchCounts);
        }
    }
}
