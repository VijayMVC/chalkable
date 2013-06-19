using System;
using System.Data;
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
    }
}
