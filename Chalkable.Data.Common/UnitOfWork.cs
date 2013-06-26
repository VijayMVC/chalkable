using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;

namespace Chalkable.Data.Common
{
    public class UnitOfWork : IDisposable
    {
        public SqlConnection Connection { get; private set; }
        public SqlTransaction Transaction { get; private set; }

        public UnitOfWork(string connectionString, bool beginTransaction)
        {
            Connection = new SqlConnection(connectionString);
            Connection.Open();
            if (beginTransaction)
                Transaction = Connection.BeginTransaction();
        }

        public void Dispose()
        {
            if (Connection.State != ConnectionState.Closed)
                Connection.Close();
        }

        public void Commit()
        {
            if (Transaction != null)
                Transaction.Commit();
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

        private SqlCommand GetTextCommand(string sql)
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

                    if (pair.Value != null)
                        command.Parameters.Add(new SqlParameter(CompleteToParam(pair.Key), pair.Value));
                    else
                        command.Parameters.Add(new SqlParameter(CompleteToParam(pair.Key), DBNull.Value));
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
    }
}
