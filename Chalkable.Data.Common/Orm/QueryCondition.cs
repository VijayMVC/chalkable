using System;
using System.Collections;
using System.Collections.Generic;

namespace Chalkable.Data.Common.Orm
{

    public abstract class QueryCondition
    {
        public abstract void BuildSqlWhere(DbQuery result, string tableName, bool first = true);
    }

    public class SimpleQueryCondition : QueryCondition
    {
        public string Field { get; private set; }
        public string Param { get; private set; }
        public object Value { get; private set; }
        public ConditionRelation Relation { get; private set; }
        public SimpleQueryCondition(string field, object value, ConditionRelation relation)
        {
            Field = field;
            Value = value;
            Relation = relation;
            Param = Field;
        }

        public SimpleQueryCondition(string field, string param, object value, ConditionRelation relation)
        {
            Field = field;
            Value = value;
            Relation = relation;
            Param = param;
        }

        private static Dictionary<ConditionRelation, string> relationMapping = new Dictionary<ConditionRelation, string>
            {
                {ConditionRelation.Less, "<"},
                {ConditionRelation.LessEqual, "<="},
                {ConditionRelation.Equal, "="},
                {ConditionRelation.GreaterEqual, ">="},
                {ConditionRelation.Greater, ">"},
                {ConditionRelation.NotEqual, "<>"},
            };

        public override void BuildSqlWhere(DbQuery result, string tableName, bool first = true)
        {
            if (first)
                result.Sql.Append(" Where ");
            if (Value != null)
            {
                result.Sql.Append("([").Append(tableName).Append("].[").Append(Field).Append("]")
                    .Append(relationMapping[Relation])
                    .Append("@").Append(Param).Append(")");
                result.Parameters.Add(Param, Value);    
            }
            else
            {
                if (Relation == ConditionRelation.Equal)
                    result.Sql.Append("([").Append(tableName).Append("].[").Append(Field).Append("] is null)");
                else if (Relation == ConditionRelation.NotEqual)
                    result.Sql.Append("([").Append(tableName).Append("].[").Append(Field).Append("] is not null)");
                else
                    throw new NotSupportedException();
            }
        }
    }

    public enum ConditionRelation
    {
        Less,
        LessEqual,
        Equal,
        GreaterEqual,
        Greater,
        NotEqual
    }

    public abstract class QueryConditionSet : QueryCondition, ICollection<QueryCondition>
    {
        private List<QueryCondition> conditions = new List<QueryCondition>();

        public IEnumerator<QueryCondition> GetEnumerator()
        {
            return conditions.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(QueryCondition item)
        {
            conditions.Add(item);
        }

        public void Add(string field, object value)
        {
            Add(new SimpleQueryCondition(field, value, ConditionRelation.Equal));
        }

        public void Add(string field, object value, ConditionRelation relation)
        {
            Add(new SimpleQueryCondition(field, value, relation));
        }

        public void Add(string field, string paramName, object value, ConditionRelation relation)
        {
            Add(new SimpleQueryCondition(field, paramName, value, relation));
        }

        public void Clear()
        {
            conditions.Clear();
        }

        public bool Contains(QueryCondition item)
        {
            return conditions.Contains(item);
        }

        public void CopyTo(QueryCondition[] array, int arrayIndex)
        {
            conditions.CopyTo(array, arrayIndex);
        }

        public bool Remove(QueryCondition item)
        {
            return conditions.Remove(item);
        }

        public int Count
        {
            get { return conditions.Count; }
        }
        public bool IsReadOnly
        {
            get { return false; }
        }

        public override void BuildSqlWhere(DbQuery result, string tableName, bool first = true)
        {
            if (first && conditions.Count > 0)
                result.Sql.Append(" Where ");
            if (conditions.Count > 0)
            {
                result.Sql.Append("(");
                bool firstIteration = true;
                foreach (var queryCondition in conditions)
                {
                    if (firstIteration)
                        firstIteration = false;
                    else
                        result.Sql.Append(JoinOperation);
                    queryCondition.BuildSqlWhere(result, tableName, false);
                }
                result.Sql.Append(")");
            }
        }

        protected string JoinOperation { get; set; }
    }

    public class AndQueryCondition : QueryConditionSet
    {
        public AndQueryCondition()
        {
            JoinOperation = "AND";
        }
    }

    public class OrQueryCondition : QueryConditionSet
    {
        public OrQueryCondition()
        {
            JoinOperation = "OR";
        }
    }
}
