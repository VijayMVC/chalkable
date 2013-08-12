using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Chalkable.Common;
using Chalkable.Common.Exceptions;

namespace Chalkable.Data.Common.Orm
{
    public static class Orm
    {

        public enum OrderType
        {
            Asc = 1,
            Desc = 2
        }
        private static IDictionary<OrderType, string> orederTypesMap = new Dictionary<OrderType, string>
            {
                {OrderType.Asc, "ASC"},
                {OrderType.Desc, "DESC"}
            }; 

        private static bool IsDbField(PropertyInfo propertyInfo)
        {
            return propertyInfo.GetCustomAttribute<DataEntityAttr>() == null
                && propertyInfo.GetCustomAttribute<NotDbFieldAttr>() == null;
        }
        
        public static List<string> Fields(Type t)
        {
            var res = new List<string>();
            var props = t.GetProperties(BindingFlags.Instance | BindingFlags.Public);
            foreach (var propertyInfo in props)
                if (propertyInfo.CanWrite && propertyInfo.CanRead)
                {
                    if (IsDbField(propertyInfo))
                        res.Add(propertyInfo.Name);
                }
            return res;
        }

        public static List<string> Fields<T>()
        {
            var t = typeof (T);
            return Fields(t);
        }

        private const string COMPLEX_RESULT_SET_FORMAT = " [{0}].[{1}] as {0}_{1}";
        private const string ID_FIELD = "Id";

        public static IList<string> FullFieldsNames(Type t)
        {
            return FullFieldsNames(t, t.Name);
        }
        public static IList<string> FullFieldsNames(Type t, string prefix)
        {
            var fields = Fields(t);
            return fields.Select(x => FullFieldName(t, prefix, x)).ToList();
        }
        public static string FullFieldName(Type t, string field)
        {
            return FullFieldName(t, t.Name, field);
        }
        public static string FullFieldName(Type t, string prefix, string field)
        {
            return string.Format(COMPLEX_RESULT_SET_FORMAT, prefix, field);
        }

        public static string ComplexResultSetQuery(IList<Type> types)
        {
            return ComplexResultSetQuery(types.ToDictionary(x => x, x => x.Name));
        }
        
        public static string ComplexResultSetQuery(IDictionary<Type, string> modelsNames)
        {
            var res = new StringBuilder();
            var index = 0;
            foreach (var modelName in modelsNames)
            {
                res.Append(FullFieldsNames(modelName.Key, modelName.Value).JoinString(","));
                if (index != modelsNames.Keys.Count - 1)
                    res.Append(",");
                index++;
            }
            return res.ToString();
        }


        //TODO: insert from select 

        public static DbQuery SimpleInsert<T>(T obj)
        {
            return SimpleListInsert(new List<T> { obj });
        } 

        public static DbQuery SimpleListInsert<T>(IList<T> objs)
        {
            var res = new DbQuery {Parameters = new Dictionary<string, object>()};
            var b = new StringBuilder();
            var t = typeof(T);
            var fields = Fields(t);
            b.Append("Insert into [").Append(t.Name).Append("] (");
            b.Append(fields.Select(x => "[" + x + "]").JoinString(",")).Append(")");
            b.Append(" values ");

            for (int i = 0; i < objs.Count; i++)
            {
                b.Append("(").Append(fields.Select(x => "@" + x + "_" + i.ToString()).JoinString(",")).Append(")");
                if (i != objs.Count - 1)
                {
                    b.Append(",");
                }
                foreach (var field in fields)
                {
                    var fieldValue = t.GetProperty(field).GetValue(objs[i]);
                    res.Parameters.Add(field + "_" + i.ToString(), fieldValue);
                }                
            }
            res.Sql = b;
            return res;
        }


         public static DbQuery SimpleUpdate<T>(T obj)
         {
             var t = typeof(T);
             var fields = Fields(t);
             return SimpleUpdate(t, fields, obj);
         }

         public static DbQuery SimpleUpdate<T>(IList<T> objs)
         {
             var b = new StringBuilder();
             var t = typeof(T);
             var fields = Fields(t);
             var res = new DbQuery {Parameters = new Dictionary<string, object>()};
             for (int i = 0; i < objs.Count; i++)
             {
                 var q = SimpleUpdate(t, fields, objs[i], i);
                 b.Append(q.Sql).Append(" ");
                 foreach (var parameter in q.Parameters)
                 {
                     if (!res.Parameters.ContainsKey(parameter.Key))
                         res.Parameters.Add(parameter);
                 }
             }
             res.Sql = b;
             return res;
         }



         public static DbQuery SimpleUpdate<T>(IDictionary<string, object> updateParams, QueryCondition conditions)
         {
             var t = typeof(T);
             //var fields = Fields(t).Select(x => x.ToLower()).ToList();
             return SimpleUpdate(t,  updateParams, conditions);
         }

         private static DbQuery SimpleUpdate<T>(Type t, IList<string> fields, T obj, int index = 0)
         {
              var updateParams = new Dictionary<string, object>();
             foreach (var field in fields)
             {
                 var value = t.GetProperty(field).GetValue(obj);
                 //if(value != null)
                     updateParams.Add(field, value);
             }
             var queryConds = new AndQueryCondition
                 {
                     {ID_FIELD, ID_FIELD + "_" + index, t.GetProperty(ID_FIELD).GetValue(obj), ConditionRelation.Equal}
                 };
             return SimpleUpdate(t, updateParams, queryConds, index);
         }

         private static DbQuery SimpleUpdate(Type t, IEnumerable<KeyValuePair<string, object>> updateParams, QueryCondition queryCondition, int index = 0)
         {
             var setParamsPrefix = "set_param_" + index + "_";
             var setParamMapper = new Dictionary<string, string>();
             var setParams = new Dictionary<string, object>();
             foreach (var updateParam in updateParams)
             {
                 var setParamsKey = setParamsPrefix + updateParam.Key;
                 setParamMapper.Add(setParamsKey, updateParam.Key);
                 setParams.Add(setParamsKey, updateParam.Value);
             }
             return SimpleUpdate(t, setParams, setParamMapper, queryCondition);
         }
         private static DbQuery SimpleUpdate(Type t, IDictionary<string, object> updateParams,
                                             IDictionary<string, string> updateParamsMapper, QueryCondition queryCondition)
         {
             var res = new DbQuery { Parameters = new Dictionary<string, object>() };
             res.Sql.Append("Update [").Append(t.Name).Append("] set");
             res.Sql.Append(updateParams.Select(x => "[" + updateParamsMapper[x.Key] + "]=@" + x.Key).JoinString(","));

             foreach (var updateParam in updateParams)
             {
                 if (!res.Parameters.ContainsKey(updateParam.Key))
                     res.Parameters.Add(updateParam);
             }
             queryCondition.BuildSqlWhere(res, t.Name);
             return res;
         }

        public static DbQuery SimpleDelete<T>(T obj)
        {
            var t = typeof(T);
            var conds = new AndQueryCondition { { ID_FIELD, t.GetProperty(ID_FIELD).GetValue(obj) } };
            return SimpleDelete<T>(conds);
        }

        public static DbQuery SimpleDelete<T>(QueryCondition conditioins)
        {
            var res = new DbQuery();
            var t = typeof(T);
            res.Sql.AppendFormat("Delete from [{0}]", t.Name);
            conditioins.BuildSqlWhere(res, t.Name);
            return res;
        }


        public static DbQuery SimpleSelect<T>(QueryCondition queryCondition)
        {
            return SimpleSelect(typeof(T).Name, queryCondition);
        }

        public static DbQuery SimpleSelect(string tableName, QueryCondition queryCondition)
        {
            var res = new DbQuery();
            res.Sql.AppendFormat("Select * from [{0}] ", tableName);
            queryCondition.BuildSqlWhere(res, tableName);
            return res;   
        }

        public static DbQuery CountSelect<T>(QueryCondition queryCondition, string resultName)
        {
            return CountSelect(SimpleSelect<T>(queryCondition), resultName);
        }

        public static DbQuery CountSelect(DbQuery query, string resultName)
        {
            var inner = query.Sql.ToString();
            query.Sql.Clear();
            query.Sql.AppendFormat("Select Count(*) as {1} from ({0}) x", inner, resultName);
            return query;
        }

        public static DbQuery PaginationSelect<T>(QueryCondition queryCondition, string orderColumn, OrderType orderType, int start, int count)
        {
            return PaginationSelect(SimpleSelect<T>(queryCondition), orderColumn, orderType, start, count);
        }

        public static DbQuery PaginationSelect(DbQuery innerSelect, string orderColumn, OrderType orderType, int start, int count)
        {
            var b = new StringBuilder();
            b.AppendFormat("select count(*) as AllCount from ({0}) x;", innerSelect.Sql);
            b.AppendFormat("select x.* from ({0}) x ", innerSelect.Sql);
            b.AppendFormat(" order by x.{0} {1}", orderColumn, orederTypesMap[orderType]);
            b.AppendFormat(" OFFSET {0} ROWS FETCH NEXT {1} ROWS ONLY ", start, count);
            innerSelect.Sql = b;
            return innerSelect;
        }

    }
}
