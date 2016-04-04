using System;
namespace Chalkable.Common.Exceptions
{
    public class ChalkableException : Exception
    {

        public ChalkableException() : base(ChlkResources.ERR_CHALKABLE_EXCEPTION)
        {
        }

        public ChalkableException(string message) : base(message)
        {
        }

        public ChalkableException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
