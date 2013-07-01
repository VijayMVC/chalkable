using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chalkable.BusinessLogic.Services;
using Chalkable.BusinessLogic.Services.Master;
using Chalkable.BusinessLogic.Services.School;
using Chalkable.Common;
using Chalkable.Data.Common;
using Chalkable.Data.Master.DataAccess;
using Chalkable.Data.Master.Model;
using Chalkable.Tests.Services.Master;
using NUnit.Framework;

namespace Chalkable.Tests.Services.School
{
    public partial class BaseSchoolServiceTest : ServiceTestBase
    {

        [SetUp]
        public void SetUp()
        {
            CreateMasterDb();
            CreateTestSchool();
        }

        protected override void BeforCreateDb(string chalkableConnection, string masterConnection)
        {        
            if (ExistsDb(masterConnection, MASTER_DB_NAME))
            {
                using (var uow = new UnitOfWork(chalkableConnection, true))
                {
                    var schools = new SchoolDataAccess(uow).GetSchools();
                    schools = schools.Where(x => x.Name == TEST_SCHOOL_NAME).ToList();
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

        private SchoolTestContext schoolTestContext;
        public SchoolTestContext SchoolTestContext
        {
            get { return schoolTestContext; }
            private set { schoolTestContext = value; }
        }

        private IServiceLocatorSchool sysAdminSchoolLocator;
        public IServiceLocatorSchool SysAdminSchoolLocator
        {
            get { return sysAdminSchoolLocator; }
            private set { sysAdminSchoolLocator = value; }
        }
        public IServiceLocatorMaster SysAdminMasterLocator
        {
            get { return SysAdminSchoolLocator.ServiceLocatorMaster; }
        }

        [TearDown]
        public void TearDown()
        {
        }

        private const string TEST_SCHOOL_NAME = "SchoolForTest";
        protected void CreateTestSchool()
        {
            var chalkableMasterConnection = Settings.MasterConnectionString;
            var masterConnection = chalkableMasterConnection.Replace(MASTER_DB_NAME, "Master");

            var server = Settings.Servers[0];
            var school = new Data.Master.Model.School
            {
                Id = Guid.NewGuid(),
                Name = TEST_SCHOOL_NAME,
                IsEmpty = false,
                ServerUrl = server,
                Status = SchoolStatus.PayingCustomer,
                TimeZone = "UTC"
            };
            var sysLocator =ServiceLocatorFactory.CreateMasterSysAdmin();
            using (var uow = new UnitOfWork(chalkableMasterConnection, true))
            {
                var da = new SchoolDataAccess(uow);
                da.Create(school);
                uow.Commit();
                ExecuteQuery(masterConnection, "create database [" + school.Id.ToString() + "]");
                var schoolDbConnectionString = string.Format(Settings.SchoolConnectionStringTemplate, server, school.Id.ToString());
                RunCreateSchoolScripts(schoolDbConnectionString);
            }
            var schoolSl = sysLocator.SchoolServiceLocator(school.Id);
            SysAdminSchoolLocator = new BaseSchoolServiceLocatorTest(new BaseMasterServiceLocatorTest(schoolSl.Context));
            SchoolTestContext = SchoolTestContext.CreateSchoolContext(schoolSl);
        }

    }
}
