using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;

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


        private SqlCommand GetStoredProcedureCommandWithParams(string name, Dictionary<string, object> parameters)
        {
            var command = GetStoredProcedureCommand(name);
            command.Parameters.Clear();
            AddParamsToCommand(command, parameters);
            return command;
        }

        private void AddParamsToCommand(DbCommand command, Dictionary<string, object> parameters)
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

        protected SqlDataReader ExecuteStoredProcedureReader(string name, Dictionary<string, object> parameters)
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

        public void ExecuteNonQueryParametrized(string sql, Dictionary<string, object> parameters)
        {
            using (SqlCommand command = GetTextCommandWithParams(sql, parameters))
            {
                command.ExecuteNonQuery();
            }
        }

        private SqlCommand GetTextCommandWithParams(string sql, Dictionary<string, object> parameters)
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
    }
}
