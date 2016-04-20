using System;

namespace Chalkable.API.Exceptions
{
    public class ChalkableApiException : Exception
    {
        public ChalkableApiException(string message = null, Exception e = null) : base(message ?? string.Empty, e)
        {
            
        }
    }

}