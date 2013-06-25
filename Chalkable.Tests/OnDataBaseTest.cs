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
            ExecuteFile(chalkableMasterConnection, Path.Combine(masterSqlRoot, "1003 - added Status to School table.sql"));
            ExecuteFile(chalkableMasterConnection, Path.Combine(masterSqlRoot, "1004 - create preference table.sql"));
            ExecuteFile(chalkableMasterConnection, Path.Combine(masterSqlRoot, "1005 - added import system type to school table.sql"));
            ExecuteFile(chalkableMasterConnection, Path.Combine(masterSqlRoot, "1006 - added TimZone to school.sql"));


            var schoolSqlRoot = Path.Combine(SQLRoot, "ChalkableSchool");
            ExecuteFile(chalkableSchoolTemplateConnection, Path.Combine(schoolSqlRoot, "1000 - Create Script.sql"));
            ExecuteFile(chalkableSchoolTemplateConnection, Path.Combine(schoolSqlRoot, "1002 - view Person.sql"));
            ExecuteFile(chalkableSchoolTemplateConnection, Path.Combine(schoolSqlRoot, "1003 - Split function.sql"));
            ExecuteFile(chalkableSchoolTemplateConnection, Path.Combine(schoolSqlRoot, "1004 - Get School Person Stored Proc.sql"));
            ExecuteFile(chalkableSchoolTemplateConnection, Path.Combine(schoolSqlRoot, "1005 - rename column in AnnouncementReminder.sql"));
            ExecuteFile(chalkableSchoolTemplateConnection, Path.Combine(schoolSqlRoot, "1007 - remove IsCurrent from SchoolYear.sql"));
            ExecuteFile(chalkableSchoolTemplateConnection, Path.Combine(schoolSqlRoot, "1008 - create apply starring procedures and StudentAnnouncement table.sql"));
            ExecuteFile(chalkableSchoolTemplateConnection, Path.Combine(schoolSqlRoot, "1009 - create final grade table.sql"));
            ExecuteFile(chalkableSchoolTemplateConnection, Path.Combine(schoolSqlRoot, "1010 - create GetAnnouncement procedures.sql"));
            ExecuteFile(chalkableSchoolTemplateConnection, Path.Combine(schoolSqlRoot, "1011 - create GetAnnouncementDetails procedure.sql"));
            ExecuteFile(chalkableSchoolTemplateConnection, Path.Combine(schoolSqlRoot, "1012 - create CreateAnnouncement procedure.sql"));
            ExecuteFile(chalkableSchoolTemplateConnection, Path.Combine(schoolSqlRoot, "1013 - create reorderAnnouncement and deleteAnnouncement procedure.sql"));
            ExecuteFile(chalkableSchoolTemplateConnection, Path.Combine(schoolSqlRoot, "1014 - alter procedure DeleteAnnouncement.sql"));
            ExecuteFile(chalkableSchoolTemplateConnection, Path.Combine(schoolSqlRoot, "1015 - create GetAnnouncementQnAs procedure.sql"));
            ExecuteFile(chalkableSchoolTemplateConnection, Path.Combine(schoolSqlRoot, "1016 - create phone, message, period, roominfo.sql"));
            ExecuteFile(chalkableSchoolTemplateConnection, Path.Combine(schoolSqlRoot, "1017 - create attendance , discipline tables.sql"));
            ExecuteFile(chalkableSchoolTemplateConnection, Path.Combine(schoolSqlRoot, "1018 - create procedure ReBuildSections.sql"));
            ExecuteFile(chalkableSchoolTemplateConnection, Path.Combine(schoolSqlRoot, "1019 - renaming courseINfo table to course table.sql"));
            ExecuteFile(chalkableSchoolTemplateConnection, Path.Combine(schoolSqlRoot, "1020 - create studentParent , finalGradeAnnType, studentFinalGrade tables.sql"));
            ExecuteFile(chalkableSchoolTemplateConnection, Path.Combine(schoolSqlRoot, "1021 - create studentDailyAttendace, Notification tables.sql"));
            ExecuteFile(chalkableSchoolTemplateConnection, Path.Combine(schoolSqlRoot, "1022 - recreate  class table.sql"));
            ExecuteFile(chalkableSchoolTemplateConnection, Path.Combine(schoolSqlRoot, "1023 - create procedure GetClasses.sql"));
            ExecuteFile(chalkableSchoolTemplateConnection, Path.Combine(schoolSqlRoot, "1024 - create reports , appinstalls tables.sql"));

            
        }
    }
}
