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
        public string Title { get; set; }

        public ExceptionViewData Inner { get; set; }

        public static ExceptionViewData Create(Exception outer)
        {
            if (outer == null)
                return null;
            
            return new ExceptionViewData
            {
                Message = outer.Message,
                ExceptionType = outer.GetType().Name,
                Title = (outer as ChalkableException)?.Title,
                InnerMessage = outer.InnerException?.Message,
                Inner = Create(outer.InnerException),

#if DEBUG
                StackTrace = outer.StackTrace,
                InnerStackTrace = outer.InnerException?.StackTrace,
#endif
            };
        }
    }
}
