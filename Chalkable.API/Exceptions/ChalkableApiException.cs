using System;

namespace Chalkable.API.Exceptions
{
    public class ChalkableApiException : Exception
    {
        public string Body { get; }

        public ChalkableApiException(string message = null, Exception e = null, string body = null) : base(message ?? string.Empty, e)
        {
            Body = body;
        }
    }

}