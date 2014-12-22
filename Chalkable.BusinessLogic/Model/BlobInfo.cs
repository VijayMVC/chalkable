using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Blob;

namespace Chalkable.BusinessLogic.Model
{
    public class BlobInfo
    {
        public long Size { get; set; }
        public string Name { get; set; }
        public string Uri { get; set; }

        public static BlobInfo Create(ICloudBlob blob)
        {
            return new BlobInfo
                {
                    Name = blob.Name,
                    Size = blob.Properties.Length,
                    Uri = blob.Uri.ToString()
                };
        }
        public static IList<BlobInfo> Create(IList<ICloudBlob> blobs)
        {
            return blobs.Select(Create).ToList();
        } 
    }
}
