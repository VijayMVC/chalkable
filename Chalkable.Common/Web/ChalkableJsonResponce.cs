using System;
using Chalkable.Common.Exceptions;

namespace Chalkable.Common.Web
{
    public class ChalkableJsonResponce
    {
        public Object Data { get; set; }
        public Boolean Success { get; set; }

        public ChalkableJsonResponce(Object data)
        {
            Data = data;
            Success = true;
            if (data is ChalkableException)
            {
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