namespace Chalkable.Common.Exceptions
{
    public class AnnouncementDeleteFailedException : ChalkableException
    {
        public AnnouncementDeleteFailedException(string message, string title = null) : base(message, title)
        {
        }
    }
}
