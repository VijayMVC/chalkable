using System;
using System.Diagnostics;
using Chalkable.BusinessLogic.Services;
using Chalkable.Common;
using Chalkable.Data.Common;
using Chalkable.Data.Master.DataAccess;
using Chalkable.Data.Master.Model;
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
            };
            using (var uow = new UnitOfWork(chalkableMasterConnection, true))
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

        protected District CreateSimpleDistrict(string districtName)
        {
            var chalkableMasterConnection = Settings.MasterConnectionString;
            var masterConnection = chalkableMasterConnection.Replace(MASTER_DB_NAME, "Master");
            var server = Settings.Servers[0];
            var district = new District
                {
                    Id = Guid.NewGuid(),
                    Name = districtName,
                    TimeZone = "UTC",
                    ServerUrl = server
                };
            using (var uow = new UnitOfWork(chalkableMasterConnection, true))
            {
                var da = new DistrictDataAccess(uow);
                da.Insert(district);
                uow.Commit();
                ExecuteQuery(masterConnection, "create database [" + district.Id.ToString() + "]");
                var schoolDbConnectionString = string.Format(Settings.SchoolConnectionStringTemplate, server, district.Id.ToString());
                RunCreateSchoolScripts(schoolDbConnectionString);
            
            }
            return district;
        }

        protected override void BeforCreateDb(string chalkableConnection, string masterConnection)
        {
            if (ExistsDb(masterConnection, MASTER_DB_NAME))
            {
                using (var uow = new UnitOfWork(chalkableConnection, true))
                {
                    var districts = new DistrictDataAccess(uow).GetAll();
                    uow.Commit();
                    foreach (var district in districts)
                    {
                        try
                        {
                            DropDbIfExists(masterConnection, district.Id.ToString());
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

        protected DistrictTestContext CreateDistrictTestContext()
        {
            var district = CreateSimpleDistrict("DistrictForTest");
            var sysLocator = ServiceLocatorFactory.CreateMasterSysAdmin();
            return DistrictTestContext.Create(sysLocator, district);
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
