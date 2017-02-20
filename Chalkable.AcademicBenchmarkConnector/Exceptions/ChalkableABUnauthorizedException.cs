using Chalkable.Common.Exceptions;

namespace Chalkable.AcademicBenchmarkConnector.Exceptions
{
    public class ChalkableABUnauthorizedException : ChalkableException
    {
        private const string DEFAULT_MESSAGE = "Academic Benchmark Unauthorized Exception";
        public ChalkableABUnauthorizedException()
            : base(DEFAULT_MESSAGE)
        {
        }
        public ChalkableABUnauthorizedException(string message) : base(message)
        {
        }

        public ChalkableABUnauthorizedException(string errorMsg, string infoMsg) : base($"{errorMsg}. {infoMsg}")
        {
            
        }
    }
}
