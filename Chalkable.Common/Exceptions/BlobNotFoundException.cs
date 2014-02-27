using System;


namespace Chalkable.Common.Exceptions
{
    public class BlobNotFoundException : ChalkableException
    {

        public BlobNotFoundException() : base(ChlkResources.ERR_BLOB_NOT_FOUND)
        {
        }
        public BlobNotFoundException(string message) : base(message)
        {
        }
    }
}
