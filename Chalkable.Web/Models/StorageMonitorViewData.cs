using System;
using System.Collections.Generic;
using System.Linq;
using Chalkable.BusinessLogic.Model;

namespace Chalkable.Web.Models
{
    public class ContainerViewData
    {
        public string Name { get; set; }
        public string Uri { get; set; }

        protected ContainerViewData() { }

        public static ContainerViewData Create(BlobContainerInfo container)
        {
            var res = new ContainerViewData()
                          {
                              Name = container.Name,
                              Uri = container.Uri,                             
                          };
            return res;
        }

        public static IList<ContainerViewData> Create(IList<BlobContainerInfo> containers)
        {
            return containers.Select(Create).ToList();
        }

    }

    public class BlobViewData
    {
        public long Size { get; set; }
        public string Name { get; set; }
        public string Uri { get; set; }

        public static BlobViewData Create(BlobInfo blob)
        {
            var res = new BlobViewData
                          {
                              Size = blob.Size,
                              Name = blob.Name,
                              Uri = blob.Uri
                          };
            return res;
        }
        
        public static IList<BlobViewData> Create(IList<BlobInfo> blobs)
        {
            return blobs.Select(Create).ToList();
        } 
    }

    public class BackupViewData
    {
        public string Ticks { get; set; }
        public DateTime Created { get; set; }
        public bool HasMaster { get; set; }
        public bool HasSchoolTemplate { get; set; }
        public int SchoolCount { get; set; }
    }
 }