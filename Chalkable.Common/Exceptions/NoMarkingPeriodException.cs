
namespace Chalkable.Common.Exceptions
{
    public class NoMarkingPeriodException : ChalkableException
    {
        public NoMarkingPeriodException(): base(ChlkResources.ERR_NO_MARKING_PERIOD_FOUND)
        {
        }
        public NoMarkingPeriodException(string message) : base(message)
        {   
        }
    }
}
