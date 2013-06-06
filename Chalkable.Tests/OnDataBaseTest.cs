using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Chalkable.Tests
{
    [TestFixture]
    public class OnDataBaseTest : TestBase
    {
        protected void CreateMasterDb()
        {

            var chalkableConnection = ConfigurationManager.ConnectionStrings["ChalkableMaster"].ConnectionString;
            var masterConnection = chalkableConnection.Replace(DB_NAME, "Master");

            BeforCreateDB(chalkableConnection, masterConnection);

            ExecuteQuery(masterConnection, "create database " + DB_NAME);
            SqlConnection.ClearAllPools();
            var masterSqlRoot = Path.Combine(SQLRoot, "ChalkableMaster");
            ExecuteFile(chalkableConnection, Path.Combine(masterSqlRoot, "1000 - Create DB Script.sql"));
        }
    }
}
