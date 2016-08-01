using System.Net.Http;
using Chalkable.StiConnector.Connectors.Model;

namespace Chalkable.StiConnector.Connectors
{
    public class SeatingChartConnector : ConnectorBase
    {
        private string urlFormat;
        public SeatingChartConnector(ConnectorLocator locator) : base(locator)
        {
            urlFormat = BaseUrl + "chalkable/sections/{0}/seatingchart/{1}";
        }
        
        public SeatingChart GetChart(int sectionId, int termId)
        {
            return Call<SeatingChart>(string.Format(urlFormat, sectionId, termId));
        }
        
        public void UpdateChart(int sectionId, int termId, SeatingChart chart)
        {
            Post(string.Format(urlFormat, sectionId, termId), chart, null, HttpMethod.Put);
        }
    }
}
