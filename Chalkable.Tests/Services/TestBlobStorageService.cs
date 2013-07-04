using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chalkable.BusinessLogic.Services;

namespace Chalkable.Tests.Services
{
    public class TestBlobStorageService : IStorageBlobService
    {
        private static IDictionary<string, byte[]> blobs = new Dictionary<string, byte[]>();
 
        private string BuildBlobAddress(string containerAddress, string blobName)
        {
            return containerAddress + "/" + blobName;
        }

        public void AddBlob(string containerAddress, string key, byte[] content)
        {
            var blobAddress = BuildBlobAddress(containerAddress, key);
            if (!blobs.ContainsKey(blobAddress))
                blobs.Add(blobAddress, content);
            else blobs[blobAddress] = content;
        }

        public byte[] GetBlobContent(string containerAddress, string key)
        {
            var blobAddress = BuildBlobAddress(containerAddress, key);
            if (blobs.ContainsKey(blobAddress))
                return blobs[BuildBlobAddress(containerAddress, key)];
            return null;
        }

        public void DeleteBlob(string containderName, string key)
        {
            var blobAddress = BuildBlobAddress(containderName, key);
            if (blobs.ContainsKey(blobAddress))
                blobs.Remove(blobAddress);
        }


        public Chalkable.Common.PaginatedList<BusinessLogic.Model.BlobContainerInfo> GetBlobContainers(int start = 0, int count = int.MaxValue)
        {
            throw new NotImplementedException();
        }

        public Chalkable.Common.PaginatedList<BusinessLogic.Model.BlobInfo> GetBlobs(string containeraddress, string keyPrefix = null, int start = 0, int count = int.MaxValue)
        {
            throw new NotImplementedException();
        }

        public void DeleteBlob(string blobAddress)
        {
            throw new NotImplementedException();
        }
    }
}
