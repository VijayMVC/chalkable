using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Chalkable.StiConnector.Connectors.Model;

namespace Chalkable.StiConnector.Connectors
{
    public class ActivityAttachmentsConnector : ConnectorBase
    {
        private string url_format;
        public ActivityAttachmentsConnector(ConnectorLocator locator) : base(locator)
        {
            url_format = BaseUrl + "Chalkable/activities/{0}/attachments";
        }

        public IList<ActivityAttachment> CreateAttachments(int activityId, string fileName, byte[] fileContent)
        {
            return PostWithFile<IList<ActivityAttachment>>(string.Format(url_format, activityId), fileName, fileContent, null);
        }

        public void Delete(int activityId, int id)
        {
            Post<ActivityAttachment>(string.Format(url_format + "/{1}", activityId, id), null);
        }

        public void Update(int activityId, int id, string fileName, byte[] fileContent)
        {
            PostWithFile<ActivityAttachment>(string.Format(url_format + "/{1}", activityId, id), fileName, fileContent, null, PUT);
        }

        public IList<ActivityAttachment> GetAttachments(int activityId)
        {
            return Call<IList<ActivityAttachment>>(string.Format(url_format, activityId));
        } 

        public byte[] GetAttachmentContent(int activityId, int id)
        {
            var url = string.Format(url_format + "/{1}", activityId, id);
            //return Call<byte[]>(url);

            try
            {
                return InitWebClient().DownloadData(url);
            }
            catch (WebException ex)
            {
                var reader = new StreamReader(ex.Response.GetResponseStream());
                var msg = reader.ReadToEnd();
                throw new Exception(msg);
            }

        }
        
    }
}
