using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chalkable.BusinessLogic.Services;
using NUnit.Framework;

namespace Chalkable.Tests.Services.Master
{
    public class UserService : OnDataBaseTest
    {
        [Test]
        public void CreateLoginTest()
        {
            var sl = ServiceLocatorFactory.CreateMasterSysAdmin();
            var created = sl.UserService.CreateSysAdmin("sysadmin", "qqqq");
            var loggedIn = sl.UserService.Login("sysadmin", "qqqq");
            Assert.NotNull(loggedIn);
            Assert.AreEqual(created.Id, loggedIn.Id);
        }
    }
}
