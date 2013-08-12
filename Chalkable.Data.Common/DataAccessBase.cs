﻿using System;
using System.Linq;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using Chalkable.Common;
using Chalkable.Data.Common.Orm;

namespace Chalkable.Data.Common
{
    public class DataAccessBase<TEntity> where TEntity : new()
    {
        protected const string FILTER_FORMAT = "%{0}%";
        
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

        protected void ExecuteNonQueryParametrized(string sql, IDictionary<string, object> parameters)
        {
            using (var command = unitOfWork.GetTextCommandWithParams(sql, parameters))
            {
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
            ModifyList(objs, SimpleInsert, Orm.Orm.SimpleListInsert);
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
                if (fields.Count * objs.Count > MAX_PARAMETER_NUMBER)
                {
                    var list1 = objs.Take(objs.Count / 2).ToList();
                    modifyAction(list1);
                    modifyAction(objs.Skip(list1.Count).ToList());
                }
                else
                {
                    var q = buildQueryAction(objs);
                    ExecuteNonQueryParametrized(q.Sql.ToString(), q.Parameters);
                }
            }
        }


        protected void SimpleUpdate<T>(IDictionary<string, object> updateParams, IDictionary<string, object> conditions)
        {
            var q = Orm.Orm.SimpleUpdate<T>(updateParams, conditions);
            ExecuteNonQueryParametrized(q.Sql.ToString(), q.Parameters);
        }

        protected void SimpleDelete<T>(T obj)
        {
            var q = Orm.Orm.SimpleDelete(obj);
            ExecuteNonQueryParametrized(q.Sql.ToString(), q.Parameters);
        }

        protected void SimpleDelete<T>(Guid id)
        {
            var conds = new AndQueryCondition {{"id", id}};
            var q = Orm.Orm.SimpleDelete<T>(conds);
            ExecuteNonQueryParametrized(q.Sql.ToString(), q.Parameters);
        }

        protected void SimpleDelete<T>(QueryCondition conds)
        {
            var q = Orm.Orm.SimpleDelete<T>(conds);
            ExecuteNonQueryParametrized(q.Sql.ToString(), q.Parameters);
        }
        private T Read<T>(DbQuery query, Func<DbDataReader, T> action)
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
            var command = Orm.Orm.SimpleSelect<T>(conditions);
            return ReadOne<T>(command);
        }
        protected T SelectOneOrNull<T>(QueryCondition conditions) where T : new()
        {
            var command = Orm.Orm.SimpleSelect<T>(conditions);
            return ReadOneOrNull<T>(command);
        }

        protected IList<T> SelectMany<T>() where T : new()
        {
            return SelectMany<T>(new AndQueryCondition());
        }
        protected IList<T> SelectMany<T>(QueryCondition conditions) where T : new()
        {
            var q = Orm.Orm.SimpleSelect<T>(conditions);
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
            var q = Orm.Orm.PaginationSelect<T>(conditions, orderByColumn, orderType, start, count);
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
                    var allCount = SqlTools.ReadInt32(reader, "AllCount");
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
            var resName = "AllCount";
            var q = Orm.Orm.CountSelect<T>(conditions, resName);
            return Read(q, reader => reader.Read() && SqlTools.ReadInt32(reader, resName) > 0);
        }

        protected bool Exists(DbQuery query, string resName = "AllCount")
        {
            var q = Orm.Orm.CountSelect(query, resName);
            return Read(q, reader => reader.Read() && SqlTools.ReadInt32(reader, resName) > 0);
        }
        protected int Count(DbQuery query)
        {
            var resName = "AllCount";
            var q = Orm.Orm.CountSelect(query, resName);
            return Read(q, reader => reader.Read() ? SqlTools.ReadInt32(reader, resName) : 0);
        }

        public virtual TEntity GetById(Guid id)
        {
            return SelectOne<TEntity>(new AndQueryCondition { { "Id", id } });
        }

        public virtual TEntity GetByIdOrNull(Guid id)
        {
            return SelectOneOrNull<TEntity>(new AndQueryCondition { { "Id", id } });
        }

        public virtual IList<TEntity> GetAll(QueryCondition conditions = null)
        {
            return SelectMany<TEntity>(conditions ?? new AndQueryCondition());
        }

        public virtual PaginatedList<TEntity> GetPage(int start, int count, string orderBy = null, Orm.Orm.OrderType orderType = Orm.Orm.OrderType.Asc)
        {
            if (string.IsNullOrEmpty(orderBy))
                orderBy = "Id";
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
        public virtual void Delete(Guid id)
        {
            SimpleDelete<TEntity>(id);
        }
    }
}
