using System;
using System.Collections.Generic;
using Chalkable.BusinessLogic.Model;
using Chalkable.BusinessLogic.Services.School;
using Chalkable.Common;
using Microsoft.WindowsAzure.Storage.Blob;

namespace Chalkable.BusinessLogic.Services.DemoSchool
{
    public class DemoBlobStorage
    {
        private readonly Dictionary<string, byte[]> Blobs = new Dictionary<string, byte[]>(); 

        public byte[] GetBlobContent(string containerAddress, string key)
        {
            return Blobs.ContainsKey(key) ? Blobs[key] : null;
        }

        public void DeleteBlob(string containerName, string key)
        {
            Blobs.Remove(key);
        }
        public void AddBlob(string containerAddress, string key, byte[] content)
        {
            Blobs.Add(key, content);
        }
    }

    public class DemoStorageBlobService : DemoSchoolServiceBase, IStorageBlobService
    {
        private DemoBlobStorage BlobStorage { get; set; }
        public DemoStorageBlobService(IServiceLocatorSchool serviceLocator) : base(serviceLocator)
        {
            BlobStorage = new DemoBlobStorage();
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
            BlobStorage.AddBlob(containerAddress, key, content);
        }

        public byte[] GetBlobContent(string containerAddress, string key)
        {
            return BlobStorage.GetBlobContent(containerAddress, key);
        }

        public void DeleteBlob(string blobAddress)
        {
            throw new NotImplementedException();
        }

        public void DeleteBlob(string containerName, string key)
        {
            BlobStorage.DeleteBlob(containerName, key);
        }
    }
}
