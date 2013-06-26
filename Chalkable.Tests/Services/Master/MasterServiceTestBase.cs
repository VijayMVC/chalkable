using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Chalkable.Tests.Services.Master
{
    public class MasterServiceTestBase : ServiceTestBase
    {
        [SetUp]
        public void SetUp()
        {
            CreateMasterDb();
        }

        [TearDown]
        public void TearDown()
        {
        }
    }
}
