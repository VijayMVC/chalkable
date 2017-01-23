namespace Chalkable.Common.Exceptions
{
    public class NoClassAnnouncementTypeException : ChalkableException
    {
        public NoClassAnnouncementTypeException() : base("There is no ClassAnnouncementTypes")
        { 
        }
        public NoClassAnnouncementTypeException(string message)
            : base(message)
        {
            
        }
    }
}
