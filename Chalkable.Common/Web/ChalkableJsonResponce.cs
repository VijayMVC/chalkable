using System;
using Chalkable.Common.Exceptions;

namespace Chalkable.Common.Web
{
    public class ChalkableJsonResponce
    {
        public object Data { get; set; }
        public bool Success { get; set; }

        public ChalkableJsonResponce(object data)
        {
            Data = data;
            Success = true;

            var exception = data as ChalkableException;
            if (exception != null)
            {
                Data = ExceptionViewData.Create(exception);
                Success = false;
            }
        }
    }

    public class ChalkableJsonPaginatedResponse : ChalkableJsonResponce
    {
        public int PageIndex { get; private set; }
        public int PageSize { get; private set; }
        public int TotalCount { get; private set; }
        public int TotalPages { get; private set; }
        public bool HasPreviousPage { get; set; }
        public bool HasNextPage { get; set; }

        public ChalkableJsonPaginatedResponse(IPaginatedList data)
            : base(data)
        {
            PageIndex = data.PageIndex;
            PageSize = data.PageSize;
            TotalCount = data.TotalCount;
            TotalPages = data.TotalPages;
            HasPreviousPage = data.HasPreviousPage;
            HasNextPage = data.HasNextPage;
        }
    }

}