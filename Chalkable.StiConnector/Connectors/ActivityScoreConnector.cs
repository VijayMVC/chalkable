using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Chalkable.StiConnector.Connectors.Model;

namespace Chalkable.StiConnector.Connectors
{
    public class ActivityScoreConnector : ConnectorBase
    {
        private string urlFormat;
        public ActivityScoreConnector(ConnectorLocator locator) : base(locator)
        {
            urlFormat = BaseUrl + "chalkable/activities/{0}/scores";
        }

        
        public async Task<IList<Score>> GetScores(int activityId)
        {
            return await CallAsync<IList<Score>>(string.Format(urlFormat, activityId));
        }
        
        public async Task<Score> GetScore(int activityId, int studentId)
        {
            return await CallAsync<Score>(string.Format(urlFormat + "/{1}", activityId, studentId));
        }

        
        public Score UpdateScore(int activityId, int studentId, Score score)
        {
            return Post(string.Format(urlFormat + "/{1}", activityId, studentId), score, null, HttpMethod.Put);
        }
    }
}
