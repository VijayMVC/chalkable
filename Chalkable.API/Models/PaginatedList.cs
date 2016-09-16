using System.Collections.Generic;
using System.Linq;

namespace Chalkable.API.Models
{
    public interface IPaginatedList
    {
        int TotalCount { get; }
    }

    public class PaginatedList<TItem> : List<TItem>, IPaginatedList
    {
        public int TotalCount { get; }

        public PaginatedList(IEnumerable<TItem> source, int start, int count) 
            :this(source.Skip(start).Take(count), source.Count())
        {
        }

        public PaginatedList(IEnumerable<TItem> source, int totalCount)
        {
            TotalCount = totalCount;
            AddRange(source);
        } 
    }
}
