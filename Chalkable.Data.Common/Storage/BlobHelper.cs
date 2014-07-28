using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Chalkable.Common;
using Chalkable.Common.Exceptions;
using Microsoft.WindowsAzure.Storage.Blob;

namespace Chalkable.Data.Common.Storage
{
    public class BlobHelper : BaseStorageHelper
    {
        private CloudBlobClient blobClient;       
        private CloudBlobClient GetBlobClient()
        {
            return blobClient ?? (blobClient = GetStorageAccount().CreateCloudBlobClient());
        }
        private ICloudBlob ConvertToCloudBlob(IListBlobItem blob)
        {
            var cloudBlob = GetBlobClient().GetBlobReferenceFromServer(blob.Uri);
            return cloudBlob.Exists() ? cloudBlob : null;
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

        public PaginatedList<ICloudBlob> GetBlobs(string containeraddress, string blobKeyPrefix, int start, int count)
        {
            var container = GetBlobClient().GetContainerReference(containeraddress);
            string prefix = null;
            if(!string.IsNullOrEmpty(blobKeyPrefix))
                prefix = containeraddress + "/" + BuildBlobAddress(containeraddress, blobKeyPrefix);
            var all = container.ListBlobs(prefix).ToList();
            var page = all.Skip(start).Take(count).ToList();
            var list = new List<ICloudBlob>();
            foreach (var listBlobItem in page)
            {
                var blob = ConvertToCloudBlob(listBlobItem);
                if (blob != null)
                {
                    list.Add(blob);
                }
            }
            var blobs = new PaginatedList<ICloudBlob>(list, start / count, count, all.Count);
            return blobs;
        }

        public IList<IListBlobItem> GetBlobNames(string containeraddress, string blobKeyPrefix)
        {
            var container = GetBlobClient().GetContainerReference(containeraddress);
            string prefix = null;
            if (!string.IsNullOrEmpty(blobKeyPrefix))
                prefix = containeraddress + "/" + BuildBlobAddress(containeraddress, blobKeyPrefix);
            var all = container.ListBlobs(prefix).ToList();
            return all;
        }

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
            var client = GetBlobClient();
            var blob = client.GetBlobReferenceFromServer(blobAddress);
            blob.Delete();
        }

        private CloudBlockBlob GetBlockBlob(string containerName, string key)
        {
            var client = GetBlobClient();
            var container = client.GetContainerReference(containerName);
            if (!container.Exists())
                throw new BlobNotFoundException(ChlkResources.ERR_BLOB_WITH_NAME_NOT_EXISTS);
            var blob = container.GetBlockBlobReference(BuildBlobAddress(containerName, key));
            return blob;
        }

        public bool Exists(string containerName, string key)
        {
            var blob = GetBlockBlob(containerName, key);
            return blob.Exists();
        }
        
        public void DeleteBlob(string containerName, string key)
        {
            var blob = GetBlockBlob(containerName, key);
            if(!blob.Exists())
                throw new BlobNotFoundException(ChlkResources.ERR_BLOB_WITH_ADDRESS_NOT_EXISTS);
            blob.Delete();
        }
        
        public byte[] GetBlobContent(string containerName, string key)
        {
            var blob = GetBlockBlob(containerName, key);
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

        public string GetBlobsRelativeAddress(string containerName)
        {
            var conteiner = GetBlobClient().GetContainerReference(containerName);
            if (conteiner.Exists())
            {
                var conteinerAddress = conteiner.Uri.ToString();
                return conteinerAddress + "/" + BuildBlobAddress(containerName, "");
            }
            throw new BlobNotFoundException(string.Format("Container {0} doesn't exists", containerName));
        }

        private string blobAddressTpl = "{0}_blob_{1}";

        private string BuildBlobAddress(string containerAddress, string key)
        {
            return string.Format(blobAddressTpl, containerAddress, key);
        }
    }
}
