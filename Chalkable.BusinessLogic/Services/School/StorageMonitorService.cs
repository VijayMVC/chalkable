using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using Chalkable.BusinessLogic.Security;
using Chalkable.Common;
using Chalkable.Common.Exceptions;

using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.ServiceRuntime;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace Chalkable.BusinessLogic.Services.School
{
public interface IStorageMonitorService
    {
        IList<CloudBlobContainer> GetBlobContainers();
        IList<ICloudBlob> GetBlobs(string containeraddress);
        void AddBlob(string containerAddress, string key, byte[] content);
        byte[] GetBlobContent(string containerAddress, string key);
        void DeleteBlob(Uri blobAddress);
        void DeleteBlob(string containderName, string key);
    }


    public class StorageMonitorService : SchoolServiceBase, IStorageMonitorService
    {
        private const string ConnectionStringName = "Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString";
        private CloudStorageAccount storageAccount;
        private CloudBlobClient blobClient;


        public StorageMonitorService(IServiceLocatorSchool serviceLocator)
            : base(serviceLocator)
        {
        }

        private CloudBlobClient GetBlobClient()
        {
            return blobClient ?? (blobClient = GetStorageAccount().CreateCloudBlobClient());
        }

        private CloudStorageAccount GetStorageAccount()
        {
            if (storageAccount == null)
            {
                string connectionString = RoleEnvironment.GetConfigurationSettingValue(ConnectionStringName);
                storageAccount = CloudStorageAccount.Parse(connectionString);
            }
            return storageAccount;
        }

        private ICloudBlob ConvertToCloudBlob(IListBlobItem blob)
        {

            var cloudBlob = GetBlobClient().GetBlobReferenceFromServer(blob.Uri);
            if (cloudBlob.Exists())
            {
                return cloudBlob;
            }
            return null;
        }

        public IList<CloudBlobContainer> GetBlobContainers()
        {
            var containers = GetBlobClient().ListContainers();
            IList<CloudBlobContainer> res = new List<CloudBlobContainer>();
            foreach (var cloudBlobContainer in containers)
            {
                if (cloudBlobContainer.Exists())
                    res.Add(cloudBlobContainer);
            }
            return res;
        }

        public IList<ICloudBlob> GetBlobs(string containeraddress)
        {
            CloudBlobContainer container = GetBlobClient().GetContainerReference(containeraddress);
            var blobsItems = container.ListBlobs().ToList();
            IList<ICloudBlob> blobs = new List<ICloudBlob>();
            foreach (var listBlobItem in blobsItems)
            {
                var blob = ConvertToCloudBlob(listBlobItem);
                if (blob != null)
                {
                    blobs.Add(blob);
                }
            }
            return blobs;
        }


        //TODO: needs test
        public void AddBlob(string containerAddress, string key, byte[] content)
        {
            var blobCl = GetBlobClient();
            var container = blobCl.GetContainerReference(containerAddress);
            container.CreateIfNotExists();
            var blob = container.GetBlockBlobReference(BuildBlobAddress(containerAddress, key));
            blob.UploadFromStream(new MemoryStream(content));
        }

        public void DeleteBlob(Uri blobAddress)
        {
            if (!BaseSecurity.IsSysAdmin(Context))
                throw new ChalkableSecurityException(ChlkResources.ERR_BLOB_INVALID_RIGHTS);
            var client = GetBlobClient();
            var blob = client.GetBlobReferenceFromServer(blobAddress);
            if (!blob.Exists())
                throw new BlobNotFoundException(ChlkResources.ERR_BLOB_WITH_NAME_NOT_EXISTS);
            blob.Delete();
        }
        
       
        public void DeleteBlob(string containderName, string key)
        {
            var client = GetBlobClient();
            var container = client.GetContainerReference(containderName);
            
            if(!container.Exists())
                throw new BlobNotFoundException(ChlkResources.ERR_BLOB_WITH_NAME_NOT_EXISTS);

            var blob = container.GetBlobReferenceFromServer(BuildBlobAddress(containderName, key));
            if(!blob.Exists())
                throw new BlobNotFoundException(ChlkResources.ERR_BLOB_WITH_ADDRESS_NOT_EXISTS);
            blob.Delete();
        }



        //TODO: needs test
        public byte[] GetBlobContent(string containerAddress, string key)
        {
            var blobCl = GetBlobClient();
            var container = blobCl.GetContainerReference(containerAddress);
            if (!container.Exists())
                throw new BlobNotFoundException(ChlkResources.ERR_BLOB_WITH_NAME_NOT_EXISTS);
            var blob = container.GetBlockBlobReference(BuildBlobAddress(containerAddress, key));
            if (!blob.Exists())
                throw new BlobNotFoundException(ChlkResources.ERR_BLOB_WITH_ADDRESS_NOT_EXISTS);

            Stream stream = new MemoryStream();
            try
            {
                blob.DownloadToStream(stream);
                stream.Seek(0, SeekOrigin.Begin);
                var buff = new byte[stream.Length];
                stream.Read(buff, 0, buff.Count());
                return buff;
            }
            finally
            {
                stream.Dispose();
            }
        }

        private string BuildBlobAddress(string containerAddress, string key)
        {
            return containerAddress + "_blob_" + key;
        }


 
    }
}
