using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Chalkable.API.Models;

namespace Chalkable.API.Endpoints
{
    public class AttachmentEndpoint : Base
    {
        public AttachmentEndpoint(IConnector connector) : base(connector)
        {
        }
       
        public async Task<Attachment> UploadAttachment(string fileName, Stream stream)
        {
            var url = $"/Attachment/Upload.json";
            return await Connector.Put<Attachment>($"{url}filename={fileName}", stream);
        }
    }
}