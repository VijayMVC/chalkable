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
        protected virtual void BeforCreateDb(string chalkableConnection, string masterConnection)
        {
            try
            {
                DropDbIfExists(masterConnection, MASTER_DB_NAME);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }

            try
            {
                DropDbIfExists(masterConnection, SCHOOL_DB_TEMPLATE_NAME);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }
        

        protected void RunCreateSchoolScripts(string schoolDbConnectionString)
        {
            var schoolSqlRoot = Path.Combine(SQLRoot, "ChalkableSchool");
            ExecuteFile(schoolDbConnectionString, Path.Combine(schoolSqlRoot, "1000 - Create Script.sql"));
            ExecuteFile(schoolDbConnectionString, Path.Combine(schoolSqlRoot, "1002 - view Person.sql"));
            ExecuteFile(schoolDbConnectionString, Path.Combine(schoolSqlRoot, "1003 - Split function.sql"));
            ExecuteFile(schoolDbConnectionString, Path.Combine(schoolSqlRoot, "1004 - Get School Person Stored Proc.sql"));
            ExecuteFile(schoolDbConnectionString, Path.Combine(schoolSqlRoot, "1005 - rename column in AnnouncementReminder.sql"));
            ExecuteFile(schoolDbConnectionString, Path.Combine(schoolSqlRoot, "1007 - remove IsCurrent from SchoolYear.sql"));
            ExecuteFile(schoolDbConnectionString, Path.Combine(schoolSqlRoot, "1008 - create apply starring procedures and StudentAnnouncement table.sql"));
            ExecuteFile(schoolDbConnectionString, Path.Combine(schoolSqlRoot, "1009 - create final grade table.sql"));
            ExecuteFile(schoolDbConnectionString, Path.Combine(schoolSqlRoot, "1010 - create GetAnnouncement procedures.sql"));
            ExecuteFile(schoolDbConnectionString, Path.Combine(schoolSqlRoot, "1011 - create GetAnnouncementDetails procedure.sql"));
            ExecuteFile(schoolDbConnectionString, Path.Combine(schoolSqlRoot, "1012 - create CreateAnnouncement procedure.sql"));
            ExecuteFile(schoolDbConnectionString, Path.Combine(schoolSqlRoot, "1013 - create reorderAnnouncement and deleteAnnouncement procedure.sql"));
            ExecuteFile(schoolDbConnectionString, Path.Combine(schoolSqlRoot, "1014 - alter procedure DeleteAnnouncement.sql"));
            ExecuteFile(schoolDbConnectionString, Path.Combine(schoolSqlRoot, "1015 - create GetAnnouncementQnAs procedure.sql"));
            ExecuteFile(schoolDbConnectionString, Path.Combine(schoolSqlRoot, "1016 - create phone, message, period, roominfo.sql"));
            ExecuteFile(schoolDbConnectionString, Path.Combine(schoolSqlRoot, "1017 - create attendance , discipline tables.sql"));
            ExecuteFile(schoolDbConnectionString, Path.Combine(schoolSqlRoot, "1018 - create procedure ReBuildSections.sql"));
            ExecuteFile(schoolDbConnectionString, Path.Combine(schoolSqlRoot, "1019 - renaming courseINfo table to course table.sql"));
            ExecuteFile(schoolDbConnectionString, Path.Combine(schoolSqlRoot, "1020 - create studentParent , finalGradeAnnType, studentFinalGrade tables.sql"));
            ExecuteFile(schoolDbConnectionString, Path.Combine(schoolSqlRoot, "1021 - create studentDailyAttendace, Notification tables.sql"));
            ExecuteFile(schoolDbConnectionString, Path.Combine(schoolSqlRoot, "1022 - recreate  class table.sql"));
            ExecuteFile(schoolDbConnectionString, Path.Combine(schoolSqlRoot, "1023 - create procedure GetClasses.sql"));
            ExecuteFile(schoolDbConnectionString, Path.Combine(schoolSqlRoot, "1024 - create reports , appinstalls tables.sql"));
            ExecuteFile(schoolDbConnectionString, Path.Combine(schoolSqlRoot, "1025 - alter procedure getClasses.sql"));
            ExecuteFile(schoolDbConnectionString, Path.Combine(schoolSqlRoot, "1026 - alter procedure getPersons.sql"));
            ExecuteFile(schoolDbConnectionString, Path.Combine(schoolSqlRoot, "1027 - alter ClassPeriod table.sql"));
            ExecuteFile(schoolDbConnectionString, Path.Combine(schoolSqlRoot, "1028 - create  Attendances procedures.sql"));
            ExecuteFile(schoolDbConnectionString, Path.Combine(schoolSqlRoot, "1029 - create procedure  SetDailyAttendance.sql"));
            ExecuteFile(schoolDbConnectionString, Path.Combine(schoolSqlRoot, "1030 - alter procedure ReBuildSections.sql"));
            ExecuteFile(schoolDbConnectionString, Path.Combine(schoolSqlRoot, "1031 - fixed view announcement.sql"));
            ExecuteFile(schoolDbConnectionString, Path.Combine(schoolSqlRoot, "1032 - added unique keys.sql"));
            ExecuteFile(schoolDbConnectionString, Path.Combine(schoolSqlRoot, "1033 - fixed GetClasses procedure.sql"));
            ExecuteFile(schoolDbConnectionString, Path.Combine(schoolSqlRoot, "1034 - create procedure GetPersonDetails.sql"));
            ExecuteFile(schoolDbConnectionString, Path.Combine(schoolSqlRoot, "1035 - create procedure spGetParents.sql"));
            ExecuteFile(schoolDbConnectionString, Path.Combine(schoolSqlRoot, "1036 - create gradingStyle table.sql"));
            ExecuteFile(schoolDbConnectionString, Path.Combine(schoolSqlRoot, "1037 - alter spGetStudentAnnouncementForAnnouncement.sql"));
            ExecuteFile(schoolDbConnectionString, Path.Combine(schoolSqlRoot, "1038 - fix in spGetAnnouncementDetails.sql"));
            ExecuteFile(schoolDbConnectionString, Path.Combine(schoolSqlRoot, "1039 - fix in spGetAnnouncementQnA.sql"));
            ExecuteFile(schoolDbConnectionString, Path.Combine(schoolSqlRoot, "1040 - fix in spDeleteAnnouncement.sql"));
            ExecuteFile(schoolDbConnectionString, Path.Combine(schoolSqlRoot, "1041 - fix in spUpdateAnnouncementRecipientData.sql"));
            ExecuteFile(schoolDbConnectionString, Path.Combine(schoolSqlRoot, "1042 - removed MarkingPeriodClassRef from FinalGrade.sql"));
            ExecuteFile(schoolDbConnectionString, Path.Combine(schoolSqlRoot, "1043 - fix in spGetClasses.sql"));
            ExecuteFile(schoolDbConnectionString, Path.Combine(schoolSqlRoot, "1044 - create spGetFinalGrades.sql"));
            ExecuteFile(schoolDbConnectionString, Path.Combine(schoolSqlRoot, "1045 - fix in spGetAnnouncementDetails.sql"));
            ExecuteFile(schoolDbConnectionString, Path.Combine(schoolSqlRoot, "1046 - create function fnGetStudentGradeAvgForClass.sql"));
            ExecuteFile(schoolDbConnectionString, Path.Combine(schoolSqlRoot, "1047 - create view vwPrivateMessage.sql"));
            ExecuteFile(schoolDbConnectionString, Path.Combine(schoolSqlRoot, "1048 - create procedure spGetAnnouncementRecipientPersons.sql"));
            ExecuteFile(schoolDbConnectionString, Path.Combine(schoolSqlRoot, "1049 - fix in vwAnnouncementQnA.sql"));
            ExecuteFile(schoolDbConnectionString, Path.Combine(schoolSqlRoot, "1050 - add Number field to grade level.sql"));
            ExecuteFile(schoolDbConnectionString, Path.Combine(schoolSqlRoot, "1051 - fix spGetClasses.sql"));             
        
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
            ExecuteFile(chalkableMasterConnection, Path.Combine(masterSqlRoot, "1007 - added SisSync table.sql"));
            ExecuteFile(chalkableMasterConnection, Path.Combine(masterSqlRoot, "1008 - added ChalkableDepartment table.sql"));
            ExecuteFile(chalkableMasterConnection, Path.Combine(masterSqlRoot, "1009 - application related tables.sql"));
            ExecuteFile(chalkableMasterConnection, Path.Combine(masterSqlRoot, "1010 - added sis related columns to district table.sql"));
            ExecuteFile(chalkableMasterConnection, Path.Combine(masterSqlRoot, "1011 - add HasParentMyApps column to application table.sql"));
            ExecuteFile(chalkableMasterConnection, Path.Combine(masterSqlRoot, "1012 - add application grade level table.sql"));
            ExecuteFile(chalkableMasterConnection, Path.Combine(masterSqlRoot, "1013 - added ConfirmationKey column to User table.sql"));
            ExecuteFile(chalkableMasterConnection, Path.Combine(masterSqlRoot, "1014 - added uq_developer_schoolRef key to Developer.sql"));
            
            RunCreateSchoolScripts(chalkableSchoolTemplateConnection);
        }
    }
}
