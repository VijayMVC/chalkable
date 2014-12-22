using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using Chalkable.Data.Common;

namespace Chalkable.Tests
{
    public partial class TestBase 
    {
        protected const string MASTER_DB_NAME = "ChalkableMasterTest";
        protected const string SCHOOL_DB_TEMPLATE_NAME = "ChalkableSchoolTemplateTest";
        
        public static void ExecuteQuery(string connectionString, string sql)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                ExecuteQuery(connection, sql);
            }
        }

        public static void ExecuteQuery(SqlConnection connection, string sql)
        {
            connection.Open();
            var command = new SqlCommand();
            command.Connection = connection;
            command.CommandText = sql;
            command.CommandType = CommandType.Text;
            command.ExecuteNonQuery();
            connection.Close();
        }

        //private static void ExecuteQuery(SqlConnection connection, string sql, Func<SqlCommand> action)
        //{
            
        //}

        protected static void DropDbIfExists(string connectionString, string dbName)
        {
            if (ExistsDb(connectionString, dbName))
                ExecuteQuery(connectionString, string.Format("drop database [{0}]", dbName));
        }

        protected static bool ExistsDb(string connectionString, string dbName)
        {
            var sql = string.Format("select count(*) as DbCount from sys.databases where name = '{0}'", dbName);

            using (var connection = new SqlConnection(connectionString))
            {
                bool exists = false;
                connection.Open();
                var command = new SqlCommand
                    {
                        Connection = connection,
                        CommandText = sql,
                        CommandType = CommandType.Text
                    };
                using (var reader = command.ExecuteReader())
                {
                   exists = reader.Read() && SqlTools.ReadInt32(reader, "DbCount") > 0;
                }
                connection.Close();
                return exists;
            }
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
