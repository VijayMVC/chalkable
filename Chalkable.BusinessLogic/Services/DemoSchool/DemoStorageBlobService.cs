using System;
using System.Collections.Generic;
using Chalkable.BusinessLogic.Model;
using Chalkable.BusinessLogic.Services.DemoSchool.Storage;
using Chalkable.BusinessLogic.Services.School;
using Chalkable.Common;
using Chalkable.Data.Common.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace Chalkable.BusinessLogic.Services.DemoSchool
{
    public class DemoStorageBlobService : DemoSchoolServiceBase, IStorageBlobService
    {
        public DemoStorageBlobService(IServiceLocatorSchool serviceLocator, DemoStorage demoStorage) : base(serviceLocator, demoStorage)
        {
        }

        public PaginatedList<BlobContainerInfo> GetBlobContainers(int start = 0, int count = int.MaxValue)
        {
            throw new NotImplementedException();
        }

        public PaginatedList<BlobInfo> GetBlobs(string containeraddress, string keyPrefix = null, int start = 0, int count = int.MaxValue)
        {
            throw new NotImplementedException();
        }

        public IList<IListBlobItem> GetBlobNames(string containeraddress, string keyPrefix = null)
        {
            throw new NotImplementedException();
        }

        public void AddBlob(string containerAddress, string key, byte[] content)
        {
            Storage.BlobStorage.Add(containerAddress, key, content);
        }

        public byte[] GetBlobContent(string containerAddress, string key)
        {
            return Storage.BlobStorage.GetBlob(containerAddress, key);
        }

        public void DeleteBlob(string blobAddress)
        {
            throw new NotImplementedException();
        }

        public void DeleteBlob(string containerName, string key)
        {
            Storage.BlobStorage.DeleteBlob(containerName, key);
        }
    }
}
