using System;

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
}
