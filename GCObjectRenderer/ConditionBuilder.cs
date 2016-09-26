using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GCObjectRenderer
{
    public static class ConditionBuilder
    {
        private class FakeCondition : Condition
        {
            public string Value { get; private set; }
            public FakeCondition(string val)
            {
                Value = val;
            }
            public override bool Check(object model)
            {
                throw new System.NotImplementedException();
            }
        }
        public static Condition BuildCondition(string s)
        {
            return BuildConditionInternal(s);
        }

        private static Condition BuildConditionInternal(string s)
        {
            s = s.Trim();
            Stack<Condition> condStack = new Stack<Condition>();
            Stack<int> opStack = new Stack<int>();

            for (int i = 0; i < s.Length; )
            {
                if (s[i] == '(')
                {
                    string p = BraceHelper.SelectBracesContent(s, ref i, '(', ')');
                    condStack.Push(BuildConditionInternal(p));
                    continue;
                }
                if (char.IsLetterOrDigit(s[i]) || s[i]=='^' || s[i] == '$')
                {
                    string p = "";
                    while (i < s.Length && !"<>=!&|".Contains(s[i]))
                    {
                        p += s[i];
                        i++;
                    }
                    condStack.Push(new FakeCondition(p.Trim()));
                    continue;
                }
                string ops = "";
                while (i < s.Length && "<>=!&|".Contains(s[i]))
                {
                    ops += s[i];
                    i++;
                }
                if (ops.Length > 0)
                {
                    int op = GetOperationPriority(ops);
                    while (opStack.Count > 0 && opStack.Peek() > op)
                    {
                        condStack.Push(MakeCondition(condStack, opStack));
                    }
                    opStack.Push(op);
                    continue;
                }
                i++;
            }
            while (opStack.Count > 0)
            {
                condStack.Push(MakeCondition(condStack, opStack));
            }
            if (condStack.Count != 1)
                throw new Exception("Invalid condition statement: [" + s + "]");
            return condStack.Pop();
        }
        
        private static int GetOperationPriority(string op)
        {
            if (op == "&")
                return -1;
            if (op == "|")
                return -2;

            if (op == "<")
                return 0;
            if (op == "<=")
                return 1;
            if (op == "=")
                return 2;
            if (op == ">=")
                return 3;
            if (op == ">")
                return 4;
            if (op == "!=")
                return 5;
            
            throw new Exception("Invalid operation " + op);
        }

        private static Condition MakeCondition(Stack<Condition> conds, Stack<int> ops)
        {
            int op = ops.Pop();
            Condition right = conds.Pop();
            Condition left = conds.Pop();
            if (op >= 0)
            {
                FakeCondition l = (FakeCondition) left;
                FakeCondition r = (FakeCondition) right;
                return new SimpleCondition(l.Value, r.Value, (Relation)op);
            }
            if (left is FakeCondition)
                throw new Exception("Invalid condition type");
            if (right is FakeCondition)
                throw new Exception("Invalid condition type");
            if (op == -1)
                return new AndCondition(left, right);
            if (op == -2)
                return new OrCondition(left, right);
            throw new Exception("Invalid opeation priority " + op);
        }
    }
}
