using System.Collections.Generic;
using System.Net.Http;
using Chalkable.StiConnector.Connectors.Model;

namespace Chalkable.StiConnector.Connectors
{
    public class ActivityAssignedAttributeConnector : ConnectorBase
    {
        private string urlFormat;
        public ActivityAssignedAttributeConnector(ConnectorLocator locator)
            : base(locator)
        {
            urlFormat = BaseUrl + "Chalkable/activities/{0}/attributes";
        }

        public ActivityAssignedAttribute CreateActivityAttribute(int activityId, ActivityAssignedAttribute activityAssignedAttribute)
        {
            return Post(string.Format(urlFormat, activityId), activityAssignedAttribute);
        }

        public void Delete(int activityId, int id)
        {
            Delete(string.Format(urlFormat + "/{1}", activityId, id));
        }

        public void Update(int activityId, int id, ActivityAssignedAttribute activityAssignedAttribute)
        {
            Put(string.Format(urlFormat + "/{1}", activityId, id), activityAssignedAttribute);
        }

        public IList<ActivityAssignedAttribute> GetAttributes(int activityId)
        {
            return Call<IList<ActivityAssignedAttribute>>(string.Format(urlFormat, activityId));
        }

        public ActivityAssignedAttribute GetAttribute(int activityId, int id)
        {
            return Call<ActivityAssignedAttribute>(string.Format(urlFormat + "/{1}", activityId, id));
        }
    }
}
