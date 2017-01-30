using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Diagnostics.Tracing.Parsers.MicrosoftWindowsTCPIP;

namespace Chalkable.Common
{
    public struct ComparablePair<T1, T2> : IComparable<ComparablePair<T1, T2>>
        where T1 : IComparable<T1>
        where T2 : IComparable<T2>
    {
        public T1 First;
        public T2 Second;
        public int CompareTo(ComparablePair<T1, T2> other)
        {
            var res = First.CompareTo(other.First);
            if (res == 0)
                res = Second.CompareTo(other.Second);
            return res;
        }

        public ComparablePair(T1 first, T2 second)
        {
            First = first;
            Second = second;
        }

        public override bool Equals(object obj)
        {
            if (obj is ComparablePair<T1, T2>)
                return CompareTo((ComparablePair<T1, T2>)obj) == 0;
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return First.GetHashCode() ^ Second.GetHashCode();
        }
    }

    public class Pair<T1, T2>
    {
        public T1 First;
        public T2 Second;

        public Pair(T1 first, T2 second)
        {
            First = first;
            Second = second;
        }

        public override bool Equals(object obj)
        {
            var another = obj as Pair<T1, T2>;
            if (another == null)
                return false;
            return First.Equals(another.First) && Second.Equals(another.Second);
        }

        public override int GetHashCode()
        {
            return First.GetHashCode() ^ Second.GetHashCode();
        }
    }

    public static class PairHelper
    {
        public static IEnumerable<Pair<TFirst2, TSecond2>> Transform<TFirst1, TSecond1, TFirst2, TSecond2>(
            this IEnumerable<Pair<TFirst1, TSecond1>> collection,
            Func<TFirst1, TFirst2> firstSelector, Func<TSecond1, TSecond2> secondSelector)
        {
            return collection.Select(x => new Pair<TFirst2, TSecond2>(firstSelector(x.First), secondSelector(x.Second))).ToList();
        }

        public static IEnumerable<Pair<T2, T2>> Transform<T1, T2>(this IEnumerable<Pair<T1, T1>> collection, Func<T1, T2> selector)
        {
            return collection.Transform(selector, selector);
        }
    }
}
