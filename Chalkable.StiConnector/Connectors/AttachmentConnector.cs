using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using Chalkable.StiConnector.Connectors.Model;

namespace Chalkable.StiConnector.Connectors
{
    public class AttachmentConnector : ConnectorBase
    {
        private string url;
        private string urlFormat;
        public AttachmentConnector(ConnectorLocator locator) : base(locator)
        {
            url = BaseUrl + "Chalkable/attachments";
            urlFormat = url + "/{0}";
        }
        
        public IEnumerable<StiAttachment> UploadAttachment(string name, byte[] content)
        {
            return PostWithFile<IList<StiAttachment>>(url, name, content, null); 
        } 

        public void DeleteAttachment(int id)
        {
            Delete(string.Format(urlFormat, id));
        }
        
        public void DeleteAttachment(int activityId, int id)
        {
            locator.ActivityAttachmentsConnector.Delete(activityId, id);
            DeleteAttachment(id);
        }

        public void UpdateAttachment(int id, string name, byte[] content)
        {
            PostWithFile<StiAttachment>(string.Format(urlFormat, id), name, content, null, HttpMethod.Put);
        }

        //Todo: refactor this 
        public byte[] GetAttachmentContent(int id)
        {
            try
            {
                return InitWebClient().DownloadData(string.Format(urlFormat, id));
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
