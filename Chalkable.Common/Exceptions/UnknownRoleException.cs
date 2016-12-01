namespace Chalkable.Common.Exceptions
{
    public class UnknownRoleException : ChalkableException
    {
        public UnknownRoleException() : base(ChlkResources.ERR_UNKNOWN_ROLE)
        {
        }
        public UnknownRoleException(string message) : base(message)
        {
        } 
    }
}
