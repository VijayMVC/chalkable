﻿using System;
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

        public const string SELECT_FORMAT = "SELECT {0} FROM [{1}] ";
        public const string JOIN = "JOIN";
        public const string SIMPLE_JOIN_FORMAT = JOIN + " [{0}] ON [{0}].[{1}] = [{2}].[{3}]";
        public const string ASC = "ASC";
        public const string DESC = "DESC";
        
        private const string COMPLEX_RESULT_FORMAT = " [{0}].[{1}] as {0}_{1}";
        private const string ORDER_BY_FORMAT = "ORDER BY [{0}].[{1}] {2}";
        private const string OFFSET_ROWS_FORMAT = " OFFSET {0} ROWS FETCH NEXT {1} ROWS ONLY ";

        public const string DELETE_FORMAT = "DELETE FROM [{0}]";

        public enum OrderType
        {
            Asc = 1,
            Desc = 2
        }
        private static IDictionary<OrderType, string> orederTypesMap = new Dictionary<OrderType, string>
            {
                {OrderType.Asc, ASC},
                {OrderType.Desc, DESC}
            }; 

        private static bool IsDbField(PropertyInfo propertyInfo)
        {
            return propertyInfo.GetCustomAttribute<DataEntityAttr>() == null
                && propertyInfo.GetCustomAttribute<NotDbFieldAttr>() == null;
        }
        
        public static List<string> Fields(Type t, bool identityFields = true, bool pkFields = true)
        {
            var res = new List<string>();
            var props = t.GetProperties(BindingFlags.Instance | BindingFlags.Public);
            foreach (var propertyInfo in props)
                if (propertyInfo.CanWrite && propertyInfo.CanRead)
                {
                    if (IsDbField(propertyInfo) 
                        && (identityFields || propertyInfo.GetCustomAttribute<IdentityFieldAttr>() == null)
                        && (pkFields || propertyInfo.GetCustomAttribute<PrimaryKeyFieldAttr>() == null))
                        res.Add(propertyInfo.Name);
                }
            return res;
        }
        
        public static List<string> Fields<T>()
        {
            var t = typeof (T);
            return Fields(t);
        }

        public static IList<PropertyInfo> GetPrimaryKeyFields(Type t)
        {
            var primaryKeyFields = t.GetProperties(BindingFlags.Instance | BindingFlags.Public).ToList();
            primaryKeyFields = primaryKeyFields.Where(x => x.CanRead && x.GetCustomAttribute<PrimaryKeyFieldAttr>() != null).ToList();
            if (primaryKeyFields.Count == 0)
                throw new ChalkableException("There are no primary keys attributes in current model");
            return primaryKeyFields;
        } 
        
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
            return string.Format(COMPLEX_RESULT_FORMAT, prefix, field);
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

        private static QueryCondition BuildCondsByProperties<T>(Type t, T obj, IList<PropertyInfo> properties, string prefix = "")
        {
            var res = new AndQueryCondition();
            foreach (var property in properties)
            {
                res.Add(property.Name, prefix + property.Name, t.GetProperty(property.Name).GetValue(obj), ConditionRelation.Equal);
            }
            return res;
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
            var fields = Fields(t, false);
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
            return SimpleUpdate<T>(new List<T> {obj});
        }

        public static DbQuery SimpleUpdate<T>(IList<T> objs)
        {
            var b = new StringBuilder();
            var t = typeof(T);
            var fields = Fields(t, false, false);
            var primaryKeyFields = GetPrimaryKeyFields(t);
            var res = new DbQuery {Parameters = new Dictionary<string, object>()};
            for (int i = 0; i < objs.Count; i++)
            {
                 var q = SimpleUpdate(t, fields, primaryKeyFields, objs[i], i);
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


         public static DbQuery SimpleUpdate<T>(T obj, QueryCondition condition)
         {
             var t = typeof(T);
             var fields = Fields(t, false, false);
             return SimpleUpdate(t, fields.ToDictionary(x => x, x => t.GetProperty(x).GetValue(obj)), condition);
         }

         public static DbQuery SimpleUpdate<T>(IDictionary<string, object> updateParams, QueryCondition conditions)
         {
             var t = typeof(T);
             return SimpleUpdate(t,  updateParams, conditions);
         }

         private static DbQuery SimpleUpdate<T>(Type t, IList<string> fields, IList<PropertyInfo> primaryKeyFields, T obj, int index = 0)
         {
             var updateParams = new Dictionary<string, object>();
             foreach (var field in fields)
             {
                 var value = t.GetProperty(field).GetValue(obj);
                 updateParams.Add(field, value);
             }
             return SimpleUpdate(t, updateParams, BuildCondsByProperties(t, obj, primaryKeyFields, index + "_"), index);
         }

         public static DbQuery SimpleUpdate(Type t, IEnumerable<KeyValuePair<string, object>> updateParams, QueryCondition queryCondition, int index = 0)
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
            var primaryKeyFields = GetPrimaryKeyFields(t);
            return SimpleDelete<T>(BuildCondsByProperties(t, obj, primaryKeyFields));
        }

        public static DbQuery SimpleDelete<T>(QueryCondition conditioins)
        {
            var res = new DbQuery();
            var t = typeof(T);
            res.Sql.AppendFormat(DELETE_FORMAT, t.Name);
            conditioins.BuildSqlWhere(res, t.Name);
            return res;
        }

        public static DbQuery SimpleDelete<T>(IList<T> objs)
        {
            var t = typeof(T);
            var primaryKeyFields = GetPrimaryKeyFields(t);
            var queries = new List<DbQuery>(); 
            for (int i = 0; i < objs.Count; i++)
            {
               queries.Add(SimpleDelete<T>(BuildCondsByProperties(t, objs[i], primaryKeyFields, i + "_")));
            }
            return new DbQuery(queries);
        }

        public static DbQuery SimpleSelect<T>(QueryCondition queryCondition, int? count = null)
        {
            return SimpleSelect(typeof(T).Name, queryCondition, count);
        }

        public static DbQuery OrderedSelect(string tName, QueryCondition queryCondition, string orderBy, OrderType orderType, int? count = null)
        {
            return OrderBy(SimpleSelect(tName, queryCondition, count), tName, orderBy, orderType);
        }

        public static DbQuery OrderBy(DbQuery dbQuery, string tName, string orderByColumn, OrderType orderType)
        {
            dbQuery.Sql.AppendFormat(ORDER_BY_FORMAT, tName, orderByColumn, orederTypesMap[orderType]);
            return dbQuery;
        }

        public static DbQuery SimpleSelect(string tableName, QueryCondition queryCondition, int? count = null)
        {
            var res = new DbQuery();
            res.Sql.AppendFormat("Select {1} * from [{0}] ", tableName, count.HasValue ? " TOP " + count : "");
            if (queryCondition != null)
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

        public static DbQuery PaginationSelect(DbQuery innerSelect, IDictionary<string, OrderType> order, int start, int count)
        {
            var b = new StringBuilder();
            b.AppendFormat("select count(*) as AllCount from ({0}) x;", innerSelect.Sql);
            b.AppendFormat("select x.* from ({0}) x ", innerSelect.Sql);
            var orderBy = string.Format("x.[Id] {0}", orederTypesMap[OrderType.Asc]);
            if (order != null && order.Count > 0)
            {
                orderBy = order.Select(x => string.Format("x.[{0}] {1}", x.Key, orederTypesMap[x.Value])).JoinString(",");
            }
            b.AppendFormat(" order by {0}", orderBy);
            b.AppendFormat(OFFSET_ROWS_FORMAT, start, count);
            innerSelect.Sql = b;
            return innerSelect;
        }

        public static DbQuery PaginationSelect(DbQuery innerSelect, string orderColumn, OrderType orderType, int start, int count)
        {
            return PaginationSelect(innerSelect, new Dictionary<string, OrderType>{{orderColumn, orderType}}, start, count);
        }

    }
}
