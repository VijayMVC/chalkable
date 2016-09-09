using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using Chalkable.Common;

namespace Chalkable.Data.Common
{
    public class UnitOfWork : IDisposable
    {
        protected SqlConnection Connection { get; private set; }
        protected SqlTransaction Transaction { get; set; }

        public UnitOfWork(string connectionString, bool beginTransaction, IsolationLevel isolationLevel = IsolationLevel.ReadCommitted)
        {
            Connection = new SqlConnection(connectionString);
            Connection.Open();
            if (beginTransaction)
                Transaction = Connection.BeginTransaction(isolationLevel);
        }

        public virtual void Dispose()
        {
            if (Connection.State != ConnectionState.Closed)
                Connection.Close();
        }

        public virtual void Commit()
        {
            if (Transaction != null)
            {
                Transaction.Commit();
            }
            else
                throw new Exception("Transaction wasn't started");
        }

        public virtual void Rollback()
        {
            if (Transaction != null)
            {
                Transaction.Rollback();
            }
            else
                throw new Exception("Transaction wasn't started");
        }

        private SqlCommand GetStoredProcedureCommand(string name)
        {
            var command = new SqlCommand {Connection = Connection};
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

        public SqlCommand GetStoredProcedureCommandWithParams(string name, SqlParameter[] parameters)
        {
            var command = GetStoredProcedureCommand(name);
            command.Parameters.Clear();
            if (parameters != null)
            {
                command.Parameters.AddRange(parameters);
            }
            return command;
        }

        public SqlCommand GetTextCommandWithParams(string sql, IDictionary<string, object> parameters)
        {
            var command = GetTextCommand(sql);
            command.Parameters.Clear();
            AddParamsToCommand(command, parameters);
            return command;
        }

        public SqlCommand GetTextCommand(string sql)
        {
            var command = new SqlCommand {Connection = Connection};
            if (Transaction != null)
                command.Transaction = Transaction;
            command.CommandType = CommandType.Text;
            command.CommandText = sql;
            return command;
        }


        private void AddParamsToCommand(DbCommand command, IDictionary<string, object> parameters)
        {
            if (parameters == null)
                return;

            foreach (var pair in parameters)
            {
                SqlParameter parameter;
                if (pair.Value != null)
                {
                    if (pair.Value is IEnumerable && !(pair.Value is string))
                    {
                        var type = pair.Value.GetType();

                        while (!type.IsGenericType && type.BaseType != null)
                            type = type.BaseType;

                        if (type.IsGenericType)
                        {
                            if (type.GetGenericArguments().Length != 1)
                                throw new NotSupportedException();
                            var g = type.GetGenericArguments()[0];
                            if (g.IsValueType || g == typeof(string))
                            {
                                var table = new DataTable();
                                table.Columns.Add("value");
                                foreach (var obj in (pair.Value as IEnumerable))
                                {
                                    table.Rows.Add(obj);
                                }
                                parameter = new SqlParameter
                                {
                                    ParameterName = CompleteToParam(pair.Key),
                                    SqlDbType = SqlDbType.Structured,
                                    TypeName = "T" + g.Name,
                                    Value = table,
                                };
                            }
                            else
                            {
                                var fields = Orm.Orm.Fields(g);
                                var table = new DataTable();
                                var props = new PropertyInfo[fields.Count];
                                var isEnum = new bool[fields.Count];
                                for (int i = 0; i < fields.Count; i++)
                                {
                                    props[i] = g.GetProperty(fields[i]);
                                    var pt = Nullable.GetUnderlyingType(props[i].PropertyType) ?? props[i].PropertyType;
                                    isEnum[i] = pt.IsEnum;
                                    if (isEnum[i])
                                        pt = typeof(int);
                                    table.Columns.Add(fields[i], pt);
                                }
                                foreach (var obj in (pair.Value as IEnumerable))
                                {
                                    var ps = new object[fields.Count];
                                    for (int i = 0; i < fields.Count; i++)
                                    {
                                        var fieldValue = props[i].GetValue(obj);
                                        if (isEnum[i] && fieldValue != null)
                                            ps[i] = (int)fieldValue;
                                        else
                                            ps[i] = fieldValue ?? DBNull.Value;
                                    }
                                    table.Rows.Add(ps);
                                }
                                parameter = new SqlParameter
                                {
                                    ParameterName = CompleteToParam(pair.Key),
                                    SqlDbType = SqlDbType.Structured,
                                    TypeName = "T" + g.Name,
                                    Value = table,
                                };
                            }
                        }
                        else
                            throw new NotImplementedException();
                    }
                    else
                        parameter = new SqlParameter(CompleteToParam(pair.Key), pair.Value);
                }
                else
                    parameter = new SqlParameter(CompleteToParam(pair.Key), DBNull.Value);

                if(pair.Value is DateTime)
                    parameter.DbType = DbType.DateTime2;
                command.Parameters.Add(parameter);
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
