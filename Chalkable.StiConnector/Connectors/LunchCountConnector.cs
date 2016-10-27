using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using Chalkable.Common;
using Chalkable.StiConnector.Connectors.Model;

namespace Chalkable.StiConnector.Connectors
{
    public class LunchCountConnector : ConnectorBase
    {
        public LunchCountConnector(ConnectorLocator locator) : base(locator)
        {
        }

        public IList<LunchCount> GetLunchCount(int sectionId, DateTime date)
        {
            var url = $"{BaseUrl}chalkable/sections/{sectionId}/lunchcount/{date.ToString(Constants.DATE_FORMAT)}";
            return Call<IList<LunchCount>>(url);
        }


        public void UpdateLunchCount(int sectionId, DateTime date, List<LunchCount> lunchCounts)
        {
            Put($"{BaseUrl}chalkable/sections/{sectionId}/lunchcount/{date}", lunchCounts);
        }
    }
}
