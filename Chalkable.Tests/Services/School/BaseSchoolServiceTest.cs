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
            InitBaseData();
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

        protected void InitBaseData()
        {
            var school = CreateTestSchool();
            var sysLocator = ServiceLocatorFactory.CreateMasterSysAdmin();
            var schoolSl = sysLocator.SchoolServiceLocator(school.Id);
            SysAdminSchoolLocator = new BaseSchoolServiceLocatorTest(new BaseMasterServiceLocatorTest(schoolSl.Context));
            SchoolTestContext = SchoolTestContext.Create(schoolSl);
        }

    }
}
