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
using Chalkable.BusinessLogic.Services.Master;
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
                if (e.InnerException != null)
                {
                    Debug.WriteLine(e.InnerException.Message);
                    Debug.WriteLine(e.InnerException.StackTrace);
                    if (e.InnerException.InnerException != null)
                    {
                        Debug.WriteLine(e.InnerException.InnerException.Message);
                        Debug.WriteLine(e.InnerException.InnerException.StackTrace);
                    }
                }
            }

        }

        [Test]
        public void TestRelations()
        {
            var locator = new AcademicBenchmarkServiceLocator(SuperAdminContext, ConnectionString);

            var standardIds = new List<Guid>
            {
                new Guid("6A59A66C-6EC0-11DF-AB2D-366B9DFF4B22"),
                new Guid("cdff6dc0-416f-457c-90cc-ba148f926b9d"),
                new Guid("c5e3b333-bcec-41fc-82dc-d49b648d3b13"),
                new Guid("39eb58bf-b931-48d0-b129-11f1f836552d"),
                new Guid("c5e3b333-bcec-41fc-82dc-d49b648d3b13"),
                new Guid("c8487c0c-92c7-4eb2-b976-52d16c861cf7"),
                new Guid("4f1047f3-51ac-401d-b231-f5c87afe2de8"),
                new Guid("08bd11c1-2102-43f7-9d9f-9dd96dd594f0"),
                new Guid("fb78924e-6d25-49f4-b05c-176b0be371c9"),
                new Guid("be60a98c-a875-44b2-abf0-40105151ffd3")
            };

            var startTime = DateTime.Now;
            var relation = locator.StandardService.GetStandardsRelations(standardIds);
            var delta = DateTime.Now - startTime;

            Debug.WriteLine(delta);
            foreach (var rel in relation)
            {
                Debug.WriteLine($"{rel?.Derivatives?.Count} --- {rel?.Origins?.Count} --- {rel?.RelatedDerivatives?.Count}");
            }
        }


        [Test]
        public void TestRelationsApi()
        {
            var masterService = new ServiceLocatorMaster(SuperAdminContext);

            var standardIds = new List<Guid>
            {
                new Guid("6A59A66C-6EC0-11DF-AB2D-366B9DFF4B22"),
                new Guid("cdff6dc0-416f-457c-90cc-ba148f926b9d"),
                new Guid("c5e3b333-bcec-41fc-82dc-d49b648d3b13"),
                new Guid("39eb58bf-b931-48d0-b129-11f1f836552d"),
                new Guid("c5e3b333-bcec-41fc-82dc-d49b648d3b13"),
                new Guid("c8487c0c-92c7-4eb2-b976-52d16c861cf7"),
                new Guid("4f1047f3-51ac-401d-b231-f5c87afe2de8"),
                new Guid("08bd11c1-2102-43f7-9d9f-9dd96dd594f0"),
                new Guid("fb78924e-6d25-49f4-b05c-176b0be371c9"),
                new Guid("be60a98c-a875-44b2-abf0-40105151ffd3")
            };

            var startTime = DateTime.Now;
            var relations = Task.Run(() => masterService.AcademicBenchmarkService.GetListOfStandardRelations(standardIds)).Result;
            var delta = DateTime.Now - startTime;

            Debug.WriteLine(delta);
            foreach (var rel in relations)
            {
                Debug.WriteLine($"{rel?.Derivatives?.Count} --- {rel?.Origins?.Count} --- {rel?.RelatedDerivatives?.Count}");
            }
        }
    }
}
