using System;
using System.Collections.Generic;
using System.Linq;
using Chalkable.Common.Exceptions;

namespace Chalkable.Common
{
    public static class ListTools
    {


        public static IEnumerable<TItem> Merge<TItem>(this IEnumerable<TItem> list1, IEnumerable<TItem> list2, IComparer<TItem> comparetor)
        {
            return list1.Merge(list2, x => x, comparetor);
        }

        public static IEnumerable<TItem> Merge<TItem, TKey>(this IEnumerable<TItem> list1, IEnumerable<TItem> list2, Func<TItem, TKey> keySelector, bool sortByDesc = false)
        {
            return Merge(list1, list2, keySelector, null, sortByDesc);
        }

        public static IEnumerable<TItem> Merge<TItem, TKey>(this IEnumerable<TItem> list1, IEnumerable<TItem> list2
            , Func<TItem, TKey> keySelector, IComparer<TKey> comparator, bool sortByDesc = false)
        {

            if(keySelector == null) throw new ChalkableException("parameter keySelector is not defined for marge sort");

            comparator = comparator ?? Comparer<TKey>.Default;

            if (sortByDesc)
            {
                list1 = list1.Reverse();
                list2 = list2.Reverse();
            }
            
            IList<TItem> res = new List<TItem>();
            var enumerator1 = list1.GetEnumerator();
            var enumerator2 = list2.GetEnumerator();

            bool isEndList1 = enumerator1.MoveNext(), isEndList2 = enumerator2.MoveNext();

            while (isEndList1 && isEndList2)
            {
                var key1 = keySelector(enumerator1.Current);
                var key2 = keySelector(enumerator2.Current);

                var comRes = comparator.Compare(key1, key2);
                if (comRes < 0)
                {
                    res.Add(enumerator1.Current);
                    isEndList1 = enumerator1.MoveNext();
                }
                else
                {
                    res.Add(enumerator2.Current);
                    isEndList2 = enumerator2.MoveNext();
                }
            }
            while (isEndList1)
            {
                res.Add(enumerator1.Current);
                isEndList1 = enumerator1.MoveNext();
            }
            while (isEndList2)
            {
                res.Add(enumerator2.Current);
                isEndList2 = enumerator2.MoveNext();
            }
            return sortByDesc ? res.Reverse() : res;
        }
    }
}
