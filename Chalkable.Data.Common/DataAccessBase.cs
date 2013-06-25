using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using Chalkable.Common;

namespace Chalkable.Data.Common
{
    public class DataAccessBase
    {
        private const string AT_SIGN = "@";
        private UnitOfWork unitOfWork;
        public DataAccessBase(UnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        private SqlCommand GetStoredProcedureCommand(string name)
        {
            var command = new SqlCommand();
            command.Connection = unitOfWork.Connection;
            if (unitOfWork.Transaction != null)
                command.Transaction = unitOfWork.Transaction;
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = name;
            return command;
        }

        private string CompleteToParam(string name)
        {
            if (!name.StartsWith(AT_SIGN))
                name = AT_SIGN + name;
            return name;
        }


        private SqlCommand GetStoredProcedureCommandWithParams(string name, IDictionary<string, object> parameters)
        {
            var command = GetStoredProcedureCommand(name);
            command.Parameters.Clear();
            AddParamsToCommand(command, parameters);
            return command;
        }

        private void AddParamsToCommand(DbCommand command, IDictionary<string, object> parameters)
        {
            if (parameters != null)
            {
                foreach (var pair in parameters)
                {
                    
                    if (pair.Value != null)
                        command.Parameters.Add(new SqlParameter(CompleteToParam(pair.Key), pair.Value));
                    else
                        command.Parameters.Add(new SqlParameter(CompleteToParam(pair.Key), DBNull.Value));
                }
            }
        }

        protected SqlDataReader ExecuteStoredProcedureReader(string name, IDictionary<string, object> parameters)
        {
            using (var command = GetStoredProcedureCommandWithParams(name, parameters))
            {
                var res = command.ExecuteReader();
                return res;
            }
        }

        public DbDataReader ExecuteReaderParametrized(string sql, Dictionary<string, object> parameters)
        {
            using (SqlCommand command = GetTextCommandWithParams(sql, parameters))
            {
                DbDataReader res = command.ExecuteReader();
                return res;
            }
        }

        public void ExecuteNonQueryParametrized(string sql, IDictionary<string, object> parameters)
        {
            using (SqlCommand command = GetTextCommandWithParams(sql, parameters))
            {
                command.ExecuteNonQuery();
            }
        }

        private SqlCommand GetTextCommandWithParams(string sql, IDictionary<string, object> parameters)
        {
            SqlCommand command = GetTextCommand(sql);
            command.Parameters.Clear();
            AddParamsToCommand(command, parameters);
            return command;
        }

        private SqlCommand GetTextCommand(string sql)
        {
            var command = new SqlCommand();
            command.Connection = unitOfWork.Connection;
            if (unitOfWork.Transaction != null)
                command.Transaction = unitOfWork.Transaction;
            command.CommandType = CommandType.Text;
            command.CommandText = sql;
            return command;
        }

        protected void SimpleInsert<T>(T obj)
        {
            var q = Orm.SimpleInsert(obj);
            ExecuteNonQueryParametrized(q.Sql, q.Parameters);
        }

        protected void SimpleInsert<T>(IList<T> objs)
        {
            var q = Orm.SimpleListInsert(objs);
            ExecuteNonQueryParametrized(q.Sql, q.Parameters);
        }

        protected void SimpleUpdate<T>(T obj)
        {
            var q = Orm.SimpleUpdate(obj);
            ExecuteNonQueryParametrized(q.Sql, q.Parameters);
        }
        protected void SimpleUpdate<T>(Dictionary<string, object> updateParams, Dictionary<string, object> conditions)
        {
            var q = Orm.SimpleUpdate<T>(updateParams, conditions);
            ExecuteNonQueryParametrized(q.Sql, q.Parameters);
        }

        protected void SimpleDelete<T>(T obj)
        {
            var q = Orm.SimpleDelete(obj);
            ExecuteNonQueryParametrized(q.Sql, q.Parameters);
        }

        protected void SimpleDelete<T>(Guid id)
        {
            var conds = new Dictionary<string, object> {{"id", id}};
            var q = Orm.SimpleDelete<T>(conds);
            ExecuteNonQueryParametrized(q.Sql, q.Parameters);
        }

        protected void SimpleDelete<T>(Dictionary<string, object> conds)
        {
            var q = Orm.SimpleDelete<T>(conds);
            ExecuteNonQueryParametrized(q.Sql, q.Parameters);
        }
        private T Read<T>(DbQuery query, Func<DbDataReader, T> action)
        {
            using (var reader = ExecuteReaderParametrized(query.Sql, query.Parameters as Dictionary<string, object>))
            {
                return action(reader);
            }
        }
        protected T ReadOne<T>(DbQuery query, bool complexResultSet = false) where T : new()
        {
            return Read(query, reader => reader.Read<T>(complexResultSet));
        }
        protected T ReadOneOrNull<T>(DbQuery query, bool complexResultSet = false) where T : new()
        {
            return Read(query, reader => reader.ReadOrNull<T>(complexResultSet));
        }
        protected IList<T> ReadMany<T>(DbQuery query, bool complexResultSet = false) where T : new()
        {
            return Read(query, reader => reader.ReadList<T>(complexResultSet));
        } 

        protected T SelectOne<T>(Dictionary<string, object> conditions) where T : new() 
        {
            var command = Orm.SimpleSelect<T>(conditions);
            return ReadOne<T>(command);
        }
        protected T SelectOneOrNull<T>(Dictionary<string, object> conditions) where T : new()
        {
            var command = Orm.SimpleSelect<T>(conditions);
            return ReadOneOrNull<T>(command);
        }

        protected IList<T> SelectMany<T>() where T : new()
        {
            return SelectMany<T>(new Dictionary<string, object>());
        }
        protected IList<T> SelectMany<T>(Dictionary<string, object> conditions) where T : new()
        {
            var q = Orm.SimpleSelect<T>(conditions);
            return ReadMany<T>(q);
        }

        protected PaginatedList<T> PaginatedSelect<T>(string orderByColumn, int start, int count, Orm.OrderType orderType = Orm.OrderType.Asc) where T : new()
        {
            var conds = new Dictionary<string, object>();
            return PaginatedSelect<T>(conds, orderByColumn, start, count, orderType);
        } 

        protected PaginatedList<T> PaginatedSelect<T>(Dictionary<string, object> conditions, string orderByColumn,
                                                      int start, int count, Orm.OrderType orderType = Orm.OrderType.Asc) where T : new()
        {
            var q = Orm.PaginationSelect<T>(conditions, orderByColumn, orderType, start, count);
            return ReadPaginatedResult<T>(q, start, count);
        }

        protected PaginatedList<T> PaginatedSelect<T>(DbQuery innerSelect, string orderByColumn, int start, int count,
                                                      Orm.OrderType orderType = Orm.OrderType.Asc) where T : new()
        {
            var q = Orm.PaginationSelect<T>(innerSelect, orderByColumn,orderType, start, count);
            return ReadPaginatedResult<T>(q, start, count);
        } 

        private PaginatedList<T> ReadPaginatedResult<T>(DbQuery dbQuery,int start, int count) where T : new()
        {
            using (var reader = ExecuteReaderParametrized(dbQuery.Sql, dbQuery.Parameters as Dictionary<string, object>))
            {
                if (reader.Read())
                {
                    var allCount = SqlTools.ReadInt32(reader, "AllCount");
                    reader.NextResult();
                    var res = reader.ReadList<T>();
                    return new PaginatedList<T>(res, start / count, count, allCount);
                }
                return new PaginatedList<T>(new List<T>(), start / count, count, 0);
            }
        }
        
        protected bool Exists<T>(Dictionary<string, object> conditions) where T : new()
        {
            var resName = "AllCount";
            return Exists(Orm.CountSelect<T>(conditions, resName));   
        }

        protected bool Exists(DbQuery query, string resName = "AllCount")
        {
            return Read(query, reader => reader.Read() && SqlTools.ReadInt32(reader, resName) > 0);
        }
    }
}
