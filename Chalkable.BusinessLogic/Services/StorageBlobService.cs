using System;
using System.Collections.Generic;
using Chalkable.BusinessLogic.Model;
using Chalkable.Common;
using Chalkable.Data.Common.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace Chalkable.BusinessLogic.Services
{
    public interface IStorageBlobService
    {
        PaginatedList<BlobContainerInfo> GetBlobContainers(int start = 0, int count = int.MaxValue);
        PaginatedList<BlobInfo> GetBlobs(string containeraddress, string keyPrefix = null, int start = 0, int count = int.MaxValue);
        IList<IListBlobItem> GetBlobNames(string containeraddress, string keyPrefix = null);
        void AddBlob(string containerAddress, string key, byte[] content);
        byte[] GetBlobContent(string containerAddress, string key);
        void DeleteBlob(string blobAddress);
        void DeleteBlob(string containderName, string key);

    }
    public class StorageBlobService :  IStorageBlobService
    {
        public PaginatedList<BlobContainerInfo> GetBlobContainers(int start = 0, int count = int.MaxValue)
        {
            var res = BlobContainerInfo.Create(new BlobHelper().GetBlobContainers());
            return new PaginatedList<BlobContainerInfo>(res, start/count, count);
        }

        public PaginatedList<BlobInfo> GetBlobs(string containeraddress, string keyPrefix = null, int start = 0, int count = int.MaxValue)
        {
            return new BlobHelper().GetBlobs(containeraddress, keyPrefix, start, count).Transform(BlobInfo.Create);
        }

        public IList<IListBlobItem> GetBlobNames(string containeraddress, string keyPrefix = null)
        {
            return new BlobHelper().GetBlobNames(containeraddress, keyPrefix);
        }

        public void AddBlob(string containerAddress, string key, byte[] content)
        {
            new BlobHelper().AddBlob(containerAddress, key, content);
        }

        public byte[] GetBlobContent(string containerAddress, string key)
        {
            return new BlobHelper().GetBlobContent(containerAddress, key);
        }

        public void DeleteBlob(string blobAddress)
        {
            var bh = new BlobHelper();
            bh.DeleteBlob(new Uri(blobAddress));
        }

        public void DeleteBlob(string containderName, string key)
        {
            var bh = new BlobHelper();
            if (bh.Exists(containderName, key))
                bh.DeleteBlob(containderName, key);
        }
    }
}
