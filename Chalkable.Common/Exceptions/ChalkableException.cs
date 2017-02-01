using System;

namespace Chalkable.Common.Exceptions
{
    public class ChalkableException : Exception
    {
        public virtual string Title { get; private set; } 
        public ChalkableException() : base(ChlkResources.ERR_CHALKABLE_EXCEPTION)
        {
        }

        public ChalkableException(string message, string title = null) : base(message)
        {
            Title = title;
        }

        public ChalkableException(string message, Exception innerException, string title = null)
            : base(message, innerException)
        {
            Title = title;
        }
    }
}
