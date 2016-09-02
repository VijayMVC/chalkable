using System;
using System.Collections.Generic;
using System.Linq;

namespace Chalkable.Common
{
    public interface IPaginatedList
    {
        long Count { get; }
        int PageIndex { get; }
        int PageSize { get; }
        int TotalCount {get;}
        int TotalPages { get; }
        bool HasPreviousPage { get; }
        bool HasNextPage { get; }
    }

    public class PaginatedList<T> : List<T>, IPaginatedList
    {
        public int PageIndex { get; private set; }
        public int PageSize { get; private set; }
        public int TotalCount { get; private set; }
        public int TotalPages { get; private set; }

        private long count;

        public PaginatedList(IEnumerable<T> source, int pageIndex, int pageSize)
            : this(source.Skip(pageIndex * pageSize).Take(pageSize), pageIndex, pageSize, source.Count())
        {
        }

        public PaginatedList(IEnumerable<T> source, int pageIndex, int pageSize,int sourceCount)
        {
            count = sourceCount;
            PageIndex = pageIndex;
            PageSize = pageSize;
            TotalCount = sourceCount;
            TotalPages = TotalCount / PageSize + ((TotalCount % PageSize == 0) ? 0 : 1);
            AddRange(source);
        }

        public new long Count
        {
            get
            {
                return count;
            }
        }

        public bool HasPreviousPage
        {
            get
            {
                return (PageIndex > 0);
            }
        }

        public bool HasNextPage
        {
            get
            {
                return (PageIndex + 1 < TotalPages);
            }
        }

        private PaginatedList()
        {

        }

        public PaginatedList<object> ToObjectList()
        {
            return Transform(x => (object) x);
        }

        public PaginatedList<R> Transform<R>(Func<T, R> f)
        {
            var res = new PaginatedList<R>
                          {
                              PageIndex = PageIndex,
                              PageSize = PageSize,
                              TotalCount = TotalCount,
                              TotalPages = TotalPages
                          };
            res.AddRange(this.Select(f));
            return res;
        }
    }

}
