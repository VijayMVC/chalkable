using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chalkable.Data.AcademicBenchmark.Model;
using Chalkable.BusinessLogic.Services;
using Chalkable.BusinessLogic.Services.AcademicBenchmark;
using Chalkable.Common;
using Chalkable.Data.Common;
using Chalkable.Data.Master.Model;
using NUnit.Framework;

namespace Chalkable.Tests.AcademicBenchmarkTests
{
    class ABServicesTests
    {
        private UserContext SuperAdminContext
        {
            get
            {
                var admin = new User { Id = Guid.Empty, Login = "Virtual system admin", LoginInfo = new UserLoginInfo() };
                var context = new UserContext(admin, CoreRoles.SUPER_ADMIN_ROLE, null, null, null, null, null);
                return context;
            }
        }

        private string ConnectionString => "Data Source=uhjc12n4yc.database.windows.net;Initial Catalog=ChalkableAcademicBenchmark;UID=chalkableadmin;Pwd=Hellowebapps1!";

        [Test]
        public void TestAuthorityInsert()
        {
            //var connectionString = Settings.AcademicBenchmarkDbConnectionString;          
            var locator = new AcademicBenchmarkServiceLocator(SuperAdminContext, ConnectionString);

            var authority = new Authority
            {
                Id = Guid.NewGuid(),
                Code = "Test Code",
                Description = "Test Description"
            };
            locator.AuthorityService.Add(new List<Authority> {authority});
        }

        [Test]
        public void ImportTest()
        {
            var importService = new AcademicBenchmarkImport.ImportService(null);
            try
            {
                importService.Import();
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                Debug.WriteLine(e.StackTrace);
            }

        }
    }
}
