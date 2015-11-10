namespace Chalkable.Common.Exceptions
{
    public class MessagingDisabledException : ChalkableException
    {
        public MessagingDisabledException() : base("Messaging is disabled")
        { 
        }
        public MessagingDisabledException(string message): base(message)
        {
            
        }
    }
}
