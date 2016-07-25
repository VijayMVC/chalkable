using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GCObjectRenderer
{
    public abstract class Condition
    {
        public abstract bool Check(Object model);
    }
    public class AndCondition : Condition
    {
        public Condition Left { get; private set; }
        public Condition Right { get; private set; }
        public override bool Check(object model)
        {
            return Left.Check(model) && Right.Check(model);
        }
        public AndCondition(Condition left, Condition right)
        {
            Left = left;
            Right = right;
        }
    }
    public class OrCondition : Condition
    {
        public Condition Left { get; private set; }
        public Condition Right { get; private set; }
        public override bool Check(object model)
        {
            return Left.Check(model) || Right.Check(model);
        }
        public OrCondition(Condition left, Condition right)
        {
            Left = left;
            Right = right;
        }
    }
    public class SimpleCondition : Condition
    {
        public string Left{get; private set;}
        public string Right{get; private set;}
        public SimpleCondition(string left, string right, Relation relation)
        {
            Left = left;
            Right = right;
            Relation = relation;
        }
        public Relation Relation { get; private set; }


        public override bool Check(object model)
        {
            int cr = ObjectComparer.Compare(Left, Right, model);
            switch (Relation)
            {
                case Relation.Less:
                    return cr < 0;
                case Relation.LessEqual:
                    return cr <= 0;
                case Relation.Equal:
                    return cr == 0;
                case Relation.GreaterEqual:
                    return cr >= 0;
                case Relation.Greater:
                    return cr > 0;
                case Relation.NotEqual:
                    return cr != 0;
                default:
                    throw new Exception("Invalid Relation");
            }
        }
    }

    public static class ObjectComparer
    {
        private class TypePair
        {
            public Type Left, Right;
            public void Swap()
            {
                Type t = Left;
                Left = Right;
                Right = t;
            }
            public TypePair(Type left, Type right)
            {
                Left = left;
                Right = right;
            }
            public override int GetHashCode()
            {
                return Left.GetHashCode() ^ Right.GetHashCode();
            }
            public override bool Equals(object obj)
            {
                TypePair other = (TypePair) obj;
                return Left.Equals(other.Left) && Right.Equals(other.Right);
            }
        }

        private delegate int CompareDelegate(object left, object right);
        private static Dictionary<TypePair, CompareDelegate> comparers = new Dictionary<TypePair, CompareDelegate>();

        static ObjectComparer()
        {
            comparers.Add(new TypePair(typeof(int), typeof(string)), delegate(object left, object right)
             {
                 int l = (int)left;
                 int r;
                 if (int.TryParse(right.ToString(), out r))
                     return l > r ? 1 : l == r ? 0 : -1;
                 return string.Compare(left.ToString(), right.ToString());

             });

            comparers.Add(new TypePair(typeof(DateTime), typeof(string)), delegate(object left, object right)
            {
                DateTime l = (DateTime)left;
                DateTime r;
                if (DateTime.TryParse(right.ToString(), out r))
                    return l > r ? 1 : l == r ? 0 : -1;
                return string.Compare(left.ToString(), right.ToString());
            });

            comparers.Add(new TypePair(typeof(double), typeof(string)), delegate(object left, object right)
            {
                double l = (double)left;
                double r;
                if (double.TryParse(right.ToString(), out r))
                    return l > r ? 1 : l == r ? 0 : -1;
                return string.Compare(left.ToString(), right.ToString());
            });

            comparers.Add(new TypePair(typeof(bool), typeof(string)), delegate(object left, object right)
            {
                bool l = (bool)left;
                bool r;
                if (bool.TryParse(right.ToString(), out r))
                {
                    if (l == r)
                        return 0;
                    if (l)
                        return 1;
                    return -1;
                }
                return string.Compare(left.ToString(), right.ToString());
            });
        }
        
        public static int Compare(string left, string right, object model)
        {
            object l, r;
            int li = 0, ri = 0;
            if (left.StartsWith("^"))
                l = ReflectionHelper.ReadObject(left, ref li, model);
            else
                l = left;
            if (right.StartsWith("^"))
                r = ReflectionHelper.ReadObject(right, ref ri, model);
            else
                r = right;

            return Compare(l, r);
        }

        
        private static int Compare(object left, object right)
        {
            if (left == null)
                left = "null";
            if (right == null)
                right = "null";
            TypePair p = new TypePair(left.GetType(), right.GetType());
            if (comparers.ContainsKey(p))
                return comparers[p](left, right);
            p.Swap();
            if (comparers.ContainsKey(p))
                return -comparers[p](right, left);
            return -string.Compare(right.ToString(), left.ToString());
        }
    }

    public enum Relation
    {
        Less = 0,
        LessEqual = 1,
        Equal = 2,
        GreaterEqual = 3,
        Greater = 4,
        NotEqual = 5
    }

}
