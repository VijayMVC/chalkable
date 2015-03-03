using System.Collections.Generic;
using System.Net.Http;
using Chalkable.StiConnector.Connectors.Model;

namespace Chalkable.StiConnector.Connectors
{
    public class ActivityAttachmentsConnector : ConnectorBase
    {
        private string urlFormat;
        public ActivityAttachmentsConnector(ConnectorLocator locator) : base(locator)
        {
            urlFormat = BaseUrl + "Chalkable/activities/{0}/attachments";
        }

        public ActivityAttachment CreateActivityAttachments(int activityId, ActivityAttachment activityAttachment)
        {
            return Post(string.Format(urlFormat, activityId), activityAttachment);
        }

        public void Delete(int activityId, int id)
        {
            Delete(string.Format(urlFormat + "/{1}", activityId, id));
        }

        public void Update(int activityId, int id, ActivityAttachment activityAttachment)
        {
            Put(string.Format(urlFormat + "/{1}", activityId, id), activityAttachment);
        }

        public IList<ActivityAttachment> GetAttachments(int activityId)
        {
            return Call<IList<ActivityAttachment>>(string.Format(urlFormat, activityId));
        } 
       
        public ActivityAttachment GetAttachment(int activityId, int id)
        {
            return Call<ActivityAttachment>(string.Format(urlFormat + "/{1}", activityId, id));
        }
    }
}
