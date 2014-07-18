using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;

namespace Chalkable.Data.Common
{
    public class UnitOfWork : IDisposable
    {
        protected SqlConnection Connection { get; private set; }
        protected SqlTransaction Transaction { get; set; }

        public UnitOfWork(string connectionString, bool beginTransaction)
        {
            Connection = new SqlConnection(connectionString);
            Connection.Open();
            if (beginTransaction)
                Transaction = Connection.BeginTransaction(IsolationLevel.ReadUncommitted);
        }

        public virtual void Dispose()
        {
            if (Connection.State != ConnectionState.Closed)
                Connection.Close();
        }

        public virtual void Commit()
        {
            if (Transaction != null)
                Transaction.Commit();
            else
                throw new Exception("Transaction wasn't started");
        }

        public void Rollback()
        {
            if (Transaction != null)
            {
                Transaction.Rollback();
                Transaction = null;
            }
            else
                throw new Exception("Transaction wasn't started");
        }

        private SqlCommand GetStoredProcedureCommand(string name)
        {
            var command = new SqlCommand();
            command.Connection = Connection;
            if (Transaction != null)
                command.Transaction = Transaction;
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = name;
            return command;
        }

        public SqlCommand GetStoredProcedureCommandWithParams(string name, IDictionary<string, object> parameters)
        {
            var command = GetStoredProcedureCommand(name);
            command.Parameters.Clear();
            AddParamsToCommand(command, parameters);
            return command;
        }

        public SqlCommand GetTextCommandWithParams(string sql, IDictionary<string, object> parameters)
        {
            SqlCommand command = GetTextCommand(sql);
            command.Parameters.Clear();
            AddParamsToCommand(command, parameters);
            return command;
        }

        public SqlCommand GetTextCommand(string sql)
        {
            var command = new SqlCommand();
            command.Connection = Connection;
            if (Transaction != null)
                command.Transaction = Transaction;
            command.CommandType = CommandType.Text;
            command.CommandText = sql;
            return command;
        }


        private void AddParamsToCommand(DbCommand command, IDictionary<string, object> parameters)
        {
            if (parameters != null)
            {
                foreach (var pair in parameters)
                {
                   SqlParameter parameter;
                   if (pair.Value != null)
                        parameter = new SqlParameter(CompleteToParam(pair.Key), pair.Value);
                   else
                        parameter = new SqlParameter(CompleteToParam(pair.Key), DBNull.Value);

                   if(pair.Value is DateTime)
                       parameter.DbType = DbType.DateTime2;
                   command.Parameters.Add(parameter);
                }
            }
        }

        private string CompleteToParam(string name)
        {
            if (!name.StartsWith(AT_SIGN))
                name = AT_SIGN + name;
            return name;
        }

        private const string AT_SIGN = "@";

        public SqlBulkCopy GetBulkCopy()
        {
            var res = new SqlBulkCopy(Connection, SqlBulkCopyOptions.Default, Transaction);
            return res;
        }
    }
}
