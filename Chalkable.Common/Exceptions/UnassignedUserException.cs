namespace Chalkable.Common.Exceptions
{
    public class UnassignedUserException : ChalkableException
    {
        public UnassignedUserException() 
            : base(ChlkResources.ERR_USER_IS_NOT_ASSIGNED_TO_SCHOOL)
        {
        }

        public UnassignedUserException(string message): base(message)
        {
        }
    }
}
