using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Chalkable.Common;
using Chalkable.Common.Exceptions;

namespace Chalkable.Data.Common
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
        public static IList<string> FullFieldsNames(Type t)
        {
            var fields = Fields(t);
            var objName = t.Name;
            return fields.Select(x => string.Format(COMPLEX_RESULT_SET_FORMAT, objName, x)).ToList();
        } 

        public static string ComplexResultSetQuery(IList<Type> types)
        {
            var res = new StringBuilder();
            for (int i = 0; i < types.Count; i++)
            {
                res.Append(FullFieldsNames(types[i]).JoinString(","));
                if (i != types.Count - 1)
                    res.Append(",");
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
            res.Sql = b.ToString();
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
             res.Sql = b.ToString();
             return res;
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
             var conds = new Dictionary<string, object> { { "id", t.GetProperty("Id").GetValue(obj) } };
             return SimpleUpdate(t, updateParams, conds, index);
         }

         public static DbQuery SimpleUpdate<T>(IDictionary<string, object> updateParams, IDictionary<string, object> conditions)
         {
             var t = typeof(T);
             //var fields = Fields(t).Select(x => x.ToLower()).ToList();
             return SimpleUpdate(t,  updateParams, conditions);
         }

         private static DbQuery SimpleUpdate(Type t, IDictionary<string, object> updateParams,
                                             IDictionary<string, object> conditions, int index = 0)
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
            var conds = new Dictionary<string, object>();
            var condsMapper = new Dictionary<string, string>();
            foreach (var condition in conditions)
            {
                var condsKey = condition.Key + "_" + index;
                condsMapper.Add(condsKey, condition.Key);
                conds.Add(condsKey, condition.Value);
            }
            return SimpleUpdate(t, setParams, setParamMapper, conds, condsMapper);
         }

        private static DbQuery SimpleUpdate(Type t, IDictionary<string, object> updateParams, IDictionary<string, string> updateParamsMapper,
                                            IDictionary<string, object> conditions, IDictionary<string, string> condsMapper)
        {
            var res = new DbQuery { Parameters = new Dictionary<string, object>() };
            var b = new StringBuilder();
            b.Append("Update [").Append(t.Name).Append("] set");
            b.Append(updateParams.Select(x => "[" + updateParamsMapper[x.Key] + "]=@" + x.Key).JoinString(","));
            foreach (var condition in conditions)
            {
               res.Parameters.Add(condition);
            }
            foreach (var updateParam in updateParams)
            {
                if(!res.Parameters.ContainsKey(updateParam.Key))
                    res.Parameters.Add(updateParam);
            }

            b = BuildSqlWhere(b, t, conditions, condsMapper);
            res.Sql = b.ToString();
            return res;
        }


        public static DbQuery SimpleDelete<T>(T obj)
        {
            var t = typeof(T);
            var conds = new Dictionary<string, object> {{"Id", t.GetProperty("Id").GetValue(obj)}};
            return SimpleDelete<T>(conds);
        }
        
        public static DbQuery SimpleDelete<T>(Dictionary<string, object> conditioins)
        {
            var res = new DbQuery { Parameters = conditioins };
            var b = new StringBuilder();
            var t = typeof(T);
            b.AppendFormat("Delete from [{0}]", t.Name);
            b = BuildSqlWhere(b, t, conditioins);
            res.Sql = b.ToString();
            return res;
        }


        public static StringBuilder BuildSqlWhere(StringBuilder builder, Type t, IDictionary<string, object> conds)
        {
            return BuildSqlWhere(builder, t, conds, conds.Keys.ToDictionary(x => x, x => x));
        }
        public static StringBuilder BuildSqlWhere(StringBuilder builder, Type t, IDictionary<string, object> conds
            , IDictionary<string, string> condsMapping)
        {
            if (conds != null && conds.Count > 0)
            {
                builder.Append(" where (");
                bool first = true;
                foreach (var cond in conds)
                {
                    if(!condsMapping.ContainsKey(cond.Key))
                        throw new ChalkableException("Incorrect conditions mapping");

                    if (first) first = false;
                    else
                    {
                        builder.Append(" and ");
                    }
                    if (cond.Value != null)
                        builder.AppendFormat("[{0}].[{1}] =@{2}", t.Name, condsMapping[cond.Key], cond.Key);
                    else
                        builder.AppendFormat("[{0}].[{1}] is null", t.Name, cond.Key);
                }
                builder.Append(")");
            }
            return builder;
        }

        
        public static DbQuery SimpleSelect<T>(Dictionary<string, object> conds)
        {
            var res = new DbQuery {Parameters = conds};
            var b = new StringBuilder();
            var t = typeof (T);
            b.AppendFormat("Select * from [{0}] ", t.Name);
            b = BuildSqlWhere(b, t, conds);
            res.Sql = b.ToString();
            return res;
        }

        public static DbQuery CountSelect<T>(Dictionary<string, object> conds, string resultName)
        {
            return CountSelect(SimpleSelect<T>(conds), resultName);
        }

        public static DbQuery CountSelect(DbQuery query, string resultName)
        {
            var b = new StringBuilder();
            b.AppendFormat("Select Count(*) as {1} from ({0}) x", query.Sql, resultName);
            query.Sql = b.ToString();
            return query;
        }

        public static DbQuery PaginationSelect<T>(Dictionary<string, object> conds, string orderColumn, OrderType orderType, int start, int count)
        {
            return PaginationSelect<T>(SimpleSelect<T>(conds), orderColumn, orderType, start, count);
        }

        public static DbQuery PaginationSelect<T>(DbQuery innerSelect, string orderColumn, OrderType orderType, int start, int count)
        {
            var b = new StringBuilder();
            b.AppendFormat("select count(*) as AllCount from ({0}) x;", innerSelect.Sql);
            b.AppendFormat("select x.* from ({0}) x ", innerSelect.Sql);
            b.AppendFormat(" order by x.{0} {1}", orderColumn, orederTypesMap[orderType]);
            b.AppendFormat(" OFFSET {0} ROWS FETCH NEXT {1} ROWS ONLY ", start, count);
            innerSelect.Sql = b.ToString();
            return innerSelect;
        }

    }


    public class DbQuery
    {
        public string Sql { get; set; }
        public IDictionary<string, object> Parameters { get; set; } 
    }
}
