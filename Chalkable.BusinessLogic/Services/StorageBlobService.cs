using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chalkable.Data.Common.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace Chalkable.BusinessLogic.Services
{
    public interface IStorageBlobService
    {
        IList<CloudBlobContainer> GetBlobContainers();
        IList<ICloudBlob> GetBlobs(string containeraddress);
        void AddBlob(string containerAddress, string key, byte[] content);
        byte[] GetBlobContent(string containerAddress, string key);
        void DeleteBlob(Uri blobAddress);
        void DeleteBlob(string containderName, string key);
    }
    public class StorageBlobService :  IStorageBlobService
    {
        private BlobHelper helper;
        public StorageBlobService()
        {
            helper = new BlobHelper();
        }

        public IList<CloudBlobContainer> GetBlobContainers()
        {
            return  helper.GetBlobContainers();
        }

        public IList<ICloudBlob> GetBlobs(string containeraddress)
        {
            return helper.GetBlobs(containeraddress);
        }

        public void AddBlob(string containerAddress, string key, byte[] content)
        {
            helper.AddBlob(containerAddress, key, content);
        }

        public byte[] GetBlobContent(string containerAddress, string key)
        {
            return helper.GetBlobContent(containerAddress, key);
        }

        public void DeleteBlob(Uri blobAddress)
        {
            helper.DeleteBlob(blobAddress);
        }

        public void DeleteBlob(string containderName, string key)
        {
            helper.DeleteBlob(containderName, key);
        }
    }
}
