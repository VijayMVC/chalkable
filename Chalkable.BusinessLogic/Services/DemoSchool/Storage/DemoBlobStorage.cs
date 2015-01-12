using System;
using System.Collections.Generic;

namespace Chalkable.BusinessLogic.Services.DemoSchool.Storage
{
    public class DemoBlobStorage
    {

        private readonly Dictionary<string, byte[]> Blobs = new Dictionary<string, byte[]>(); 
        private DemoStorage storage;
        public DemoBlobStorage(DemoStorage st)
        {
            storage = st;
        }

        public void Add(string containerAddress, string key, byte[] content)
        {
           Blobs.Add(key, content);
        }

        public byte[] GetBlob(string containerAddress, string key)
        {
            return Blobs.ContainsKey(key) ? Blobs[key] : null;
        }

        public void DeleteBlob(string containerName, string key)
        {
            Blobs.Remove(key);
        }
    }
}
