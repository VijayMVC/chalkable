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
    public class DemoStorageBlobService :  DemoSchoolService, IStorageBlobService
    {
        public DemoStorageBlobService(IServiceLocatorSchool serviceLocator, DemoStorage demoStorage) : base(serviceLocator, demoStorage)
        {
        }

        public PaginatedList<BlobContainerInfo> GetBlobContainers(int start = 0, int count = int.MaxValue)
        {
            throw new NotImplementedException();
            var res = BlobContainerInfo.Create(new BlobHelper().GetBlobContainers());
            return new PaginatedList<BlobContainerInfo>(res, start/count, count);
        }

        public PaginatedList<BlobInfo> GetBlobs(string containeraddress, string keyPrefix = null, int start = 0, int count = int.MaxValue)
        {
            throw new NotImplementedException();
            return new BlobHelper().GetBlobs(containeraddress, keyPrefix, start, count).Transform(BlobInfo.Create);
        }

        public IList<IListBlobItem> GetBlobNames(string containeraddress, string keyPrefix = null)
        {
            throw new NotImplementedException();
            return new BlobHelper().GetBlobNames(containeraddress, keyPrefix);
        }

        public void AddBlob(string containerAddress, string key, byte[] content)
        {
            throw new NotImplementedException();
            new BlobHelper().AddBlob(containerAddress, key, content);
        }

        public byte[] GetBlobContent(string containerAddress, string key)
        {
            throw new NotImplementedException();
            return new BlobHelper().GetBlobContent(containerAddress, key);
        }

        public void DeleteBlob(string blobAddress)
        {
            throw new NotImplementedException();
            new BlobHelper().DeleteBlob(new Uri(blobAddress));
        }

        public void DeleteBlob(string containderName, string key)
        {
            throw new NotImplementedException();
            new BlobHelper().DeleteBlob(containderName, key);
        }
    }
}
