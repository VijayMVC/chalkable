using System;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using Chalkable.Common;
using NUnit.Framework;

namespace Chalkable.Tests
{
    [TestFixture]
    public class OnDataBaseTest : TestBase
    {
        protected void BeforCreateDb(string chalkableConnection, string masterConnection)
        {
            try
            {
                string dropQuery = String.Format("drop database {0}", MASTER_DB_NAME);
                ExecuteQuery(masterConnection, dropQuery);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }

            try
            {
                string dropQuery = String.Format("drop database {0}", SCHOOL_DB_TEMPLATE_NAME);
                ExecuteQuery(masterConnection, dropQuery);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }
        

        protected void CreateMasterDb()
        {
            

            var chalkableMasterConnection = Settings.MasterConnectionString;
            var chalkableSchoolTemplateConnection = Settings.SchoolTemplateConnectionString;

            var masterConnection = chalkableMasterConnection.Replace(MASTER_DB_NAME, "Master");
            BeforCreateDb(chalkableMasterConnection, masterConnection);

            ExecuteQuery(masterConnection, "create database " + MASTER_DB_NAME);
            ExecuteQuery(masterConnection, "create database " + SCHOOL_DB_TEMPLATE_NAME);
            


            
            
            SqlConnection.ClearAllPools();
            var masterSqlRoot = Path.Combine(SQLRoot, "ChalkableMaster");
            ExecuteFile(chalkableMasterConnection, Path.Combine(masterSqlRoot, "1000 - Create DB Script.sql"));
            ExecuteFile(chalkableMasterConnection, Path.Combine(masterSqlRoot, "1001 - add field Empty to school.sql"));
            ExecuteFile(chalkableMasterConnection, Path.Combine(masterSqlRoot, "1002 - add background task table.sql"));

            var schoolSqlRoot = Path.Combine(SQLRoot, "ChalkableSchool");
            ExecuteFile(chalkableSchoolTemplateConnection, Path.Combine(schoolSqlRoot, "1000 - Create Script.sql"));
            ExecuteFile(chalkableSchoolTemplateConnection, Path.Combine(schoolSqlRoot, "1002 - view Person.sql"));
            ExecuteFile(chalkableSchoolTemplateConnection, Path.Combine(schoolSqlRoot, "1003 - Split function.sql"));
            ExecuteFile(chalkableSchoolTemplateConnection, Path.Combine(schoolSqlRoot, "1004 - Get School Person Stored Proc.sql"));
            ExecuteFile(chalkableSchoolTemplateConnection, Path.Combine(schoolSqlRoot, "1005 - rename column in AnnouncementReminder.sql"));
            ExecuteFile(chalkableSchoolTemplateConnection, Path.Combine(schoolSqlRoot, "1007 - remove IsCurrent from SchoolYear.sql"));
            
        }
    }
}
