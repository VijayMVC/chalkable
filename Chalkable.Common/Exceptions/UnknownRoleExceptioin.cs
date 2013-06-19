
namespace Chalkable.Common.Exceptions
{
    public class UnknownRoleExceptioin : ChalkableException
    {
        public UnknownRoleExceptioin() : base(ChlkResources.ERR_UNKNOWN_ROLE)
        {
        }
        public UnknownRoleExceptioin(string message) : base(message)
        {
        } 
    }
}
