using System;

namespace Chalkable.Common.Exceptions
{
    public class ExceptionViewData
    {
        public string Message { get; set; }
        public string StackTrace { get; set; }
        public string InnerMessage { get; set; }
        public string InnerStackTrace { get; set; }
        public string ExceptionType { get; set; }

        public static ExceptionViewData Create(Exception outer, Exception inner)
        {
            var exceptionViewData = new ExceptionViewData();
            if (inner != null)
            {
                exceptionViewData.InnerMessage = inner.Message;
#if !RELEASE
                exceptionViewData.InnerStackTrace = inner.StackTrace;
#endif
            }
            exceptionViewData.Message = outer.Message;
#if !RELEASE
            exceptionViewData.StackTrace = outer.StackTrace;
#endif
            exceptionViewData.ExceptionType = outer.GetType().Name;
            return exceptionViewData;
        }
    }
}
