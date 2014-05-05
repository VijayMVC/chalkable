using System;
using System.Collections.Generic;

namespace Chalkable.BusinessLogic.Services.DemoSchool.Storage
{
    public class DemoBlobStorage
    {

        private Dictionary<string, byte[]> blobs = new Dictionary<string, byte[]>(); 
        private DemoStorage storage;
        public DemoBlobStorage(DemoStorage st)
        {
            storage = st;
        }

        public void Add(string containerAddress, string key, byte[] content)
        {
           blobs.Add(key, content);
        }

        public byte[] GetBlob(string containerAddress, string key)
        {
            return blobs.ContainsKey(key) ? blobs[key] : null;
        }

        public void DeleteBlob(string containerName, string key)
        {
            blobs.Remove(key);
        }
    }
}
