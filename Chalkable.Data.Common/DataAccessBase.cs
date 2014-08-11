using System;
using System.Data;
using System.Linq;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.Reflection;
using Chalkable.Common;
using Chalkable.Data.Common.Orm;

namespace Chalkable.Data.Common
{
    public class DataAccessBase<TEntity, TParam> where TEntity : new()
    {
        protected const string FILTER_FORMAT = "%{0}%";
        private const string ALL_COUNT_FIELD = "AllCount";

        private UnitOfWork unitOfWork;
        public DataAccessBase(UnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        protected SqlDataReader ExecuteStoredProcedureReader(string name, IDictionary<string, object> parameters)
        {
            using (var command = unitOfWork.GetStoredProcedureCommandWithParams(name, parameters))
            {
                var res = command.ExecuteReader();
                return res;
            }
        }

        protected DbDataReader ExecuteReaderParametrized(string sql, IDictionary<string, object> parameters)
        {
            using (var command = unitOfWork.GetTextCommandWithParams(sql, parameters))
            {
                DbDataReader res = command.ExecuteReader();
                return res;
            }
        }

        protected void ExecuteNonQueryParametrized(string sql, IDictionary<string, object> parameters, int? timeout = null)
        {
            using (var command = unitOfWork.GetTextCommandWithParams(sql, parameters))
            {
                if (timeout.HasValue)
                    command.CommandTimeout = timeout.Value;
                command.ExecuteNonQuery();
            }
        }

        protected void SimpleInsert<T>(T obj)
        {
            var q = Orm.Orm.SimpleInsert(obj);
            ExecuteNonQueryParametrized(q.Sql.ToString(), q.Parameters);
        }
        
        protected void SimpleInsert<T>(IList<T> objs)
        {
            if (objs.Count > 0)
            {
                //TODO: move query build to orm
                var t = typeof(T);
                var fields = Orm.Orm.Fields(t, false);
                var keyCount = Orm.Orm.GetPrimaryKeyFields(t).Count;
                if ((fields.Count + keyCount) * objs.Count > MAX_PARAMETER_NUMBER)
                {
                    var table = new DataTable();
                    var props = new PropertyInfo[fields.Count];
                    var isEnum = new bool[fields.Count];
                    for (int i = 0; i < fields.Count; i++)
                    {
                        props[i] = t.GetProperty(fields[i]);
                        var pt = Nullable.GetUnderlyingType(props[i].PropertyType) ?? props[i].PropertyType;
                        isEnum[i] = pt.IsEnum;
                        if (isEnum[i])
                            pt = typeof (int);
                        table.Columns.Add(fields[i], pt);
                    }
                    foreach (var obj in objs)
                    {
                        var ps = new object[fields.Count];
                        for (int i = 0; i < fields.Count; i++)
                        {
                            var fieldValue = props[i].GetValue(obj);
                            if (isEnum[i] && fieldValue != null)
                                ps[i] = (int) fieldValue;
                            else
                                ps[i] = fieldValue ?? DBNull.Value;
                        }
                        table.Rows.Add(ps);
                    }
                    var f = fields.Select(x => "[" + x + "]").JoinString(",");
                    var sql = string.Format("INSERT INTO [{0}]({1}) SELECT {1} FROM @t", t.Name, f);
                    using (var c = unitOfWork.GetTextCommand(sql))
                    {
                        c.CommandTimeout = 10 + objs.Count;
                        c.Parameters.Add(
                           new SqlParameter
                           {
                               ParameterName = "@t",
                               SqlDbType = SqlDbType.Structured,
                               TypeName = "T" + t.Name,
                               Value = table,
                           });

                        c.ExecuteNonQuery();
                    }
                }
                else
                {
                    var q = Orm.Orm.SimpleListInsert(objs);
                    ExecuteNonQueryParametrized(q.Sql.ToString(), q.Parameters);
                }
            }

        }

        protected void SimpleUpdate<T>(T obj)
        {
            var q = Orm.Orm.SimpleUpdate(obj);
            ExecuteNonQueryParametrized(q.Sql.ToString(), q.Parameters);
        }
        protected void SimpleUpdate<T>(IList<T> objs)
        {
            ModifyList(objs, SimpleUpdate, Orm.Orm.SimpleUpdate);
        }

        private const int MAX_PARAMETER_NUMBER = 2000;
        private void ModifyList<T>(IList<T> objs, Action<IList<T>> modifyAction, Func<IList<T>, DbQuery> buildQueryAction)
        {
            if (objs.Count > 0)
            {
                var fields = Orm.Orm.Fields<T>();
                var keyCount = Orm.Orm.GetPrimaryKeyFields(typeof (T)).Count;
                if ((fields.Count + keyCount) * objs.Count > MAX_PARAMETER_NUMBER)
                {
                    var list1 = objs.Take(objs.Count / 2).ToList();
                    modifyAction(list1);
                    modifyAction(objs.Skip(list1.Count).ToList());
                }
                else
                {
                    var q = buildQueryAction(objs);
                    ExecuteNonQueryParametrized(q.Sql.ToString(), q.Parameters, 10 + objs.Count);
                }
            }
        }

        protected void SimpleUpdate<T>(T obj, QueryCondition condition)
        {
            var q = Orm.Orm.SimpleUpdate(obj, condition);
            ExecuteNonQueryParametrized(q.Sql.ToString(), q.Parameters);
        }

        protected void SimpleUpdate<T>(IDictionary<string, object> updateParams, QueryCondition conditions)
        {
            var q = Orm.Orm.SimpleUpdate<T>(updateParams, conditions);
            ExecuteNonQueryParametrized(q.Sql.ToString(), q.Parameters);
        }

        protected void SimpleDelete<T>(IList<T> objs)
        {
            if (objs.Count > 0)
            {
                var keyCount = Orm.Orm.GetPrimaryKeyFields(typeof(T)).Count;
                if (keyCount*objs.Count > MAX_PARAMETER_NUMBER)
                {
                    var list1 = objs.Take(objs.Count / 2).ToList();
                    SimpleDelete(list1);
                    SimpleDelete(objs.Skip(list1.Count).ToList());
                }
                else
                {
                    var q = Orm.Orm.SimpleDelete(objs);
                    ExecuteNonQueryParametrized(q.Sql.ToString(), q.Parameters);    
                }
            }
        }

        protected void SimpleDelete(TEntity obj)
        {
            var q = Orm.Orm.SimpleDelete(obj);
            ExecuteNonQueryParametrized(q.Sql.ToString(), q.Parameters);
        }

        protected void SimpleDelete<T>(QueryCondition conds)
        {
            var q = Orm.Orm.SimpleDelete<T>(FilterConditions(conds));
            ExecuteNonQueryParametrized(q.Sql.ToString(), q.Parameters);
        }
        
        protected void SimpleDelete(QueryCondition conds)
        {
            var q = Orm.Orm.SimpleDelete<TEntity>(FilterConditions(conds));
            ExecuteNonQueryParametrized(q.Sql.ToString(), q.Parameters);
        }

        protected T Read<T>(DbQuery query, Func<DbDataReader, T> action)
        {
            using (var reader = ExecuteReaderParametrized(query.Sql.ToString(), query.Parameters as Dictionary<string, object>))
            {
                return action(reader);
            }
        }
        protected T ReadOne<T>(DbQuery query, bool complexResultSet = false) where T : new()
        {
            return Read(query, reader =>
                {
                    reader.Read();
                    return reader.Read<T>(complexResultSet);
                });
        }
        protected T ReadOneOrNull<T>(DbQuery query, bool complexResultSet = false) where T : new()
        {
            return Read(query, reader => reader.ReadOrNull<T>(complexResultSet));
        }
        protected IList<T> ReadMany<T>(DbQuery query, bool complexResultSet = false) where T : new()
        {
            return Read(query, reader => reader.ReadList<T>(complexResultSet));
        }

        protected T SelectOne<T>(QueryCondition conditions) where T : new() 
        {
            var command = Orm.Orm.SimpleSelect<T>(FilterConditions(conditions), 1);
            return ReadOne<T>(command);
        }
        protected T SelectOneOrNull<T>(QueryCondition conditions) where T : new()
        {
            var command = Orm.Orm.SimpleSelect<T>(FilterConditions(conditions), 1);
            return ReadOneOrNull<T>(command);
        }

        protected IList<T> SelectMany<T>() where T : new()
        {
            return SelectMany<T>(new AndQueryCondition());
        }
        protected IList<T> SelectMany<T>(QueryCondition conditions) where T : new()
        {
            var q = Orm.Orm.SimpleSelect<T>(FilterConditions(conditions));
            return ReadMany<T>(q);
        }

        protected PaginatedList<T> PaginatedSelect<T>(string orderByColumn, int start, int count, Orm.Orm.OrderType orderType = Orm.Orm.OrderType.Asc) where T : new()
        {
            var conds = new AndQueryCondition();
            return PaginatedSelect<T>(conds, orderByColumn, start, count, orderType);
        }

        protected PaginatedList<T> PaginatedSelect<T>(QueryCondition conditions, string orderByColumn,
                                                            int start, int count, Orm.Orm.OrderType orderType = Orm.Orm.OrderType.Asc) where T : new()
        {
            var q = Orm.Orm.PaginationSelect<T>(FilterConditions(conditions), orderByColumn, orderType, start, count);
            return ReadPaginatedResult<T>(q, start, count);
        }

        protected PaginatedList<T> PaginatedSelect<T>(DbQuery innerSelect, string orderByColumn, int start, int count,
                                                      Orm.Orm.OrderType orderType = Orm.Orm.OrderType.Asc) where T : new()
        {
            var q = Orm.Orm.PaginationSelect(innerSelect, orderByColumn,orderType, start, count);
            return ReadPaginatedResult<T>(q, start, count);
        }

        protected PaginatedList<T> ReadPaginatedResult<T>(DbQuery dbQuery, int start, int count,
                                                          Func<DbDataReader, IList<T>> readAction) where T : new()
        {
            using (var reader = ExecuteReaderParametrized(dbQuery.Sql.ToString(), dbQuery.Parameters as Dictionary<string, object>))
            {
                if (reader.Read())
                {
                    var allCount = SqlTools.ReadInt32(reader, ALL_COUNT_FIELD);
                    reader.NextResult();
                    var res = readAction(reader);
                    return new PaginatedList<T>(res, start / count, count, allCount);
                }
                return new PaginatedList<T>(new List<T>(), start / count, count, 0);
            }
        }

        protected PaginatedList<T> ReadPaginatedResult<T>(DbQuery dbQuery, int start, int count) where T : new()
        {
            return ReadPaginatedResult(dbQuery, start, count, x => x.ReadList<T>());
        }

        protected bool Exists<T>(QueryCondition conditions) where T : new()
        {
            var q = Orm.Orm.CountSelect<T>(FilterConditions(conditions), ALL_COUNT_FIELD);
            return Read(q, reader => reader.Read() && SqlTools.ReadInt32(reader, ALL_COUNT_FIELD) > 0);
        }

        protected bool Exists(DbQuery query, string resName = ALL_COUNT_FIELD)
        {
            var q = Orm.Orm.CountSelect(query, resName);
            return Read(q, reader => reader.Read() && SqlTools.ReadInt32(reader, resName) > 0);
        }
        protected int Count(DbQuery query)
        {
            var q = Orm.Orm.CountSelect(query, ALL_COUNT_FIELD);
            return Read(q, reader => reader.Read() ? SqlTools.ReadInt32(reader, ALL_COUNT_FIELD) : 0);
        }

        public virtual IList<TEntity> GetAll(QueryCondition conditions = null)
        {
            return SelectMany<TEntity>(conditions ?? new AndQueryCondition());
        }

        public virtual PaginatedList<TEntity> GetPage(int start, int count, string orderBy = null, Orm.Orm.OrderType orderType = Orm.Orm.OrderType.Asc)
        {
            if (string.IsNullOrEmpty(orderBy))
                orderBy = Orm.Orm.GetPrimaryKeyFields(typeof (TEntity)).First().Name;
            return PaginatedSelect<TEntity>(orderBy, start, count, orderType);
        }

        public void Insert(TEntity entity)
        {
            SimpleInsert(entity);
        }

        public void Insert(IList<TEntity> entities)
        {
            SimpleInsert(entities);
        }

        public void Update(TEntity entity)
        {
            SimpleUpdate(entity);
        }
        public void Update(IList<TEntity> entities)
        {
            SimpleUpdate(entities);
        }

        public virtual void Delete(TParam key)
        {
            SimpleDelete(BuildCondsByKey(key));
        }

        public virtual TEntity GetById(TParam key)
        {
            return SelectOne<TEntity>(BuildCondsByKey(key));
        }
        public virtual TEntity GetByIdOrNull(TParam key)
        {
            return SelectOneOrNull<TEntity>(BuildCondsByKey(key));
        }

        private QueryCondition BuildCondsByKey(TParam key)
        {
            var primaryKeyFields = Orm.Orm.GetPrimaryKeyFields(typeof(TEntity));
            return new AndQueryCondition {{primaryKeyFields.First().Name, key}};
        }

        protected virtual QueryCondition FilterConditions(QueryCondition condition)
        {
            return condition;
        }
    }
}
