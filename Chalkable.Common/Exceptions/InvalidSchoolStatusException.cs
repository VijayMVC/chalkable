
namespace Chalkable.Common.Exceptions
{
    public class InvalidSchoolStatusException : ChalkableException
    {
        public InvalidSchoolStatusException() : base(ChlkResources.ERR_SCHOOL_STATUS_INVALID_ACTION)
        {
        }
        public InvalidSchoolStatusException(string message) : base(message)
        {
        }
    }
}
