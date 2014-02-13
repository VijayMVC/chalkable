using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
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

        public IList<Score> GetSores(int activityId)
        {
            return Call<IList<Score>>(string.Format(urlFormat, activityId));
        } 
        public Score GetScore(int activityId, int studentId)
        {
            return Call<Score>(string.Format(urlFormat + "/{1}", activityId, studentId));
        }

        public Score UpdateScore(int activityId, int studentId, Score score)
        {
            return Post(string.Format(urlFormat + "/{1}", activityId, studentId), score, HttpMethod.Put);
        }
    }
}
