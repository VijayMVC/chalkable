namespace Chalkable.Common.Exceptions
{
    public class ChalkableSecurityException : ChalkableException
    {
        public ChalkableSecurityException() : base(ChlkResources.ERR_CHALKABLE_SECURITY)
        { 
        }
        public ChalkableSecurityException(string message, string title = null) : base(message, title)
        {
        }
    }
}
