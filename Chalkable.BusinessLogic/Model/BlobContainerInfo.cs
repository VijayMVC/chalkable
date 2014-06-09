using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Blob;

namespace Chalkable.BusinessLogic.Model
{
    public class BlobContainerInfo
    {
        public string Name { get; set; }
        public string Uri { get; set; }

        public static BlobContainerInfo Create(CloudBlobContainer container)
        {
            return new BlobContainerInfo
                {
                    Name = container.Name,
                    Uri = container.Uri.ToString()
                };
        }
        public static IList<BlobContainerInfo> Create(IList<CloudBlobContainer> containers)
        {
            return containers.Select(Create).ToList();
        } 
    }
}
