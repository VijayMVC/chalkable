using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;

namespace Chalkable.Tests
{
    public partial class TestBase 
    {
        protected const string DB_NAME = "ChalkableTestDb";

        protected void BeforCreateDB(string chalkableConnection, string masterConnection)
        {
            string closeConnections =
                String.Format(
                    "if exists(select db_id('{0}') where db_id('{0}') is not null) ALTER DATABASE {0} SET SINGLE_USER WITH ROLLBACK IMMEDIATE",
                    DB_NAME);
            ExecuteQuery(masterConnection, closeConnections);
            string dropQuery =
                String.Format("if exists(select db_id('{0}') where db_id('{0}') is not null) drop database {0}", DB_NAME);
            ExecuteQuery(masterConnection, dropQuery);
        }


        public static void ExecuteQuery(string connectionString, string sql)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                ExecuteQuery(connection, sql);
            }
        }

        public static void ExecuteQuery(SqlConnection connection, string sql)
        {
            connection.Open();
            SqlCommand command = new SqlCommand();
            command.Connection = connection;
            command.CommandText = sql;
            command.CommandType = CommandType.Text;
            command.ExecuteNonQuery();
            connection.Close();
        }

        protected void ExecuteFile(string connectionString, string file)
        {
            Debug.WriteLine("execute file " + file);
            string[] sl = File.ReadAllLines(file);
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "";
                for (int i = 0; i < sl.Length; i++)
                {
                    string s = sl[i].Trim();
                    if (s.ToLower() == "go")
                    {
                        ExecuteQuery(connection, query);
                        query = "";
                    }
                    else
                        query += s + "\n";
                }
                if (!String.IsNullOrEmpty(query.Trim()))
                {
                    ExecuteQuery(connection, query);
                }
            }
        }

        protected static byte[] GetFileContent(string path)
        {
            byte[] buffer = null;
            using (var fileStream = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                buffer = new byte[fileStream.Length];
                fileStream.Read(buffer, 0, buffer.Length);
            }
            return buffer;
        }
        protected static byte[] GetDefaulImage1Content
        {
            get { return GetFileContent(DefaulImage1Path); } 
        }

    }
}
