namespace Chalkable.Common.Exceptions
{
    public class ChalkableSecurityException : ChalkableException
    {
        public ChalkableSecurityException() : base("Chalkable security error")
        { 
        }
        public ChalkableSecurityException(string message) : base(message)
        {
            
        }
    }
}
