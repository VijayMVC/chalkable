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
using Chalkable.Tests.Services.TestContext;
using NUnit.Framework;

namespace Chalkable.Tests.Services.School
{
    public partial class BaseSchoolServiceTest : ServiceTestBase
    {

        [SetUp]
        public void SetUp()
        {
            CreateMasterDb();
            InitBaseData();
        }

        public DistrictTestContext DistrictTestContext { get; private set; }
        public SchoolTestContext FirstSchoolContext { get { return DistrictTestContext.FirstSchoolContext; } }
        public SchoolTestContext SecondSchoolContext { get { return DistrictTestContext.SecondSchoolContext; } }

        public IServiceLocatorSchool SysAdminFirstSchoolLocator{ get; private set; }
        public IServiceLocatorSchool SysAdminSecondSchoolLocator { get; private set; }
        public IServiceLocatorMaster SysAdminMasterLocator
        {
            get { return SysAdminFirstSchoolLocator.ServiceLocatorMaster; }
        }
        
        [TearDown]
        public void TearDown()
        {
        }

        protected void InitBaseData()
        {
            DistrictTestContext = CreateDistrictTestContext();
            var sysLocator = ServiceLocatorFactory.CreateMasterSysAdmin();
            SysAdminFirstSchoolLocator = CreateSysAdminSchoolLocator(sysLocator, DistrictTestContext.FirstSchoolContext);
            SysAdminSecondSchoolLocator = CreateSysAdminSchoolLocator(sysLocator, DistrictTestContext.SecondSchoolContext);
        }
        
        private IServiceLocatorSchool CreateSysAdminSchoolLocator(IServiceLocatorMaster sysLocator, SchoolTestContext schoolContext)
        {
            var context = sysLocator.SchoolServiceLocator(schoolContext.School.Id).Context;
            return new BaseSchoolServiceLocatorTest(new BaseMasterServiceLocatorTest(context));
        }
    }
}
