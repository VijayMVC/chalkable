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
                exceptionViewData.InnerStackTrace = inner.StackTrace;
            }
            exceptionViewData.Message = outer.Message;
            exceptionViewData.StackTrace = outer.StackTrace;
            exceptionViewData.ExceptionType = outer.GetType().Name;
            return exceptionViewData;
        }
    }
}
