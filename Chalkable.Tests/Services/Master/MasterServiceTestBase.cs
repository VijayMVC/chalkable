using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chalkable.BusinessLogic.Services;
using Chalkable.BusinessLogic.Services.Master;
using Chalkable.Tests.Services.TestContext;
using NUnit.Framework;

namespace Chalkable.Tests.Services.Master
{
    public class MasterServiceTestBase : ServiceTestBase
    {
        [SetUp]
        public void SetUp()
        {
            CreateMasterDb();
            var locator = ServiceLocatorFactory.CreateMasterSysAdmin();
            sysAdminMasterLocator = new BaseMasterServiceLocatorTest(locator.Context);
        }

        private IServiceLocatorMaster sysAdminMasterLocator;
        public IServiceLocatorMaster SysAdminMasterLocator
        {
            get { return sysAdminMasterLocator; }
        }

        [TearDown]
        public void TearDown()
        {
        }
    }
}
