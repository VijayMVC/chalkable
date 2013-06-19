using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Chalkable.Tests.Services
{
    public class ServiceTestBase : OnDataBaseTest
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
