using System;
using System.Diagnostics;
using Chalkable.BusinessLogic.Services;
using Chalkable.Common;
using Chalkable.Data.Common;
using Chalkable.Data.Master.DataAccess;
using Chalkable.Tests.Services.TestContext;

namespace Chalkable.Tests.Services
{
    public partial class ServiceTestBase : OnDataBaseTest
    {
        
        protected Data.Master.Model.School CreateTestSchool()
        {
            return CreateSimpleSchool("SchoolForTest");
        }
        protected Data.Master.Model.School CreateTestDemoSchool()
        {
            return CreateSimpleSchool("DemoSchoolForTest", true);
        }

        protected Data.Master.Model.School CreateSimpleSchool(string schoolName, bool isDemo = false)
        {
            var chalkableMasterConnection = Settings.MasterConnectionString;
            var masterConnection = chalkableMasterConnection.Replace(MASTER_DB_NAME, "Master");

            var server = Settings.Servers[0];
            var school = new Data.Master.Model.School
            {
                Id = Guid.NewGuid(),
                Name = schoolName
            };using (var uow = new UnitOfWork(chalkableMasterConnection, true))
            {
                var da = new SchoolDataAccess(uow);
                da.Insert(school);
                uow.Commit();
                ExecuteQuery(masterConnection, "create database [" + school.Id.ToString() + "]");
                var schoolDbConnectionString = string.Format(Settings.SchoolConnectionStringTemplate, server, school.Id.ToString());
                RunCreateSchoolScripts(schoolDbConnectionString);
            }
            return school;
        }

        protected override void BeforCreateDb(string chalkableConnection, string masterConnection)
        {
            if (ExistsDb(masterConnection, MASTER_DB_NAME))
            {
                using (var uow = new UnitOfWork(chalkableConnection, true))
                {
                    var schools = new SchoolDataAccess(uow).GetSchools(null, 0, int.MaxValue);
                    uow.Commit();
                    foreach (var school in schools)
                    {
                        try
                        {
                            DropDbIfExists(masterConnection, school.Id.ToString());
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine(ex.Message);
                        }
                    }
                }
            }

            base.BeforCreateDb(chalkableConnection, masterConnection);
        }


        protected DeveloperSchoolTestContex CreateDeveloperSchoolTestContext()
        {
            /*CreateTestDemoSchool();
            var sysLocator = ServiceLocatorFactory.CreateMasterSysAdmin();
            var school = sysLocator.SchoolService.UseDemoSchool();
            var sysSchoolL = sysLocator.SchoolServiceLocator(school.Id);
            return DeveloperSchoolTestContex.Create(sysSchoolL);*/
            throw new NotImplementedException();
        }

        protected SchoolTestContext CreateSchoolTestContext()
        {
            var school = CreateTestSchool();
            var sysLocator = ServiceLocatorFactory.CreateMasterSysAdmin();
            var schoolL = sysLocator.SchoolServiceLocator(school.Id);
            return SchoolTestContext.Create(schoolL);
        }

    }
}
