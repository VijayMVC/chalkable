
namespace Chalkable.Common.Exceptions
{
    public class IncorrectTaskDataException : ChalkableException
    {
        public IncorrectTaskDataException()
            : base(ChlkResources.ERR_TASK_INFO_INCORRECT_DATA)
        {
        }

        public IncorrectTaskDataException(string message) : base(message)
        {
        }
    }
}
