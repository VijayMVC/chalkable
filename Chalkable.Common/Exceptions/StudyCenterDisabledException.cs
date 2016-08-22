namespace Chalkable.Common.Exceptions
{
    public class StudyCenterDisabledException : ChalkableException
    {
        public StudyCenterDisabledException() : base("Study center is disabled")
        { 
        }
        public StudyCenterDisabledException(string message) : base(message)
        {
            
        }
    }
}