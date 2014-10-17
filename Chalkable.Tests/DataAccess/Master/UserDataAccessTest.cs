using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using Chalkable.Common;
using Chalkable.Data.Common;
using Chalkable.Data.Master.DataAccess;
using Chalkable.Data.Master.Model;
using NUnit.Framework;

namespace Chalkable.Tests.DataAccess.Master
{
    public class UserDataAccessTest : DataAccessTestBase
    {
        [Test]
        public void GetUserTest()
        {
            var da = new UserDataAccess(UnitOfWork);
            User user = new User
                {
                    Id = Guid.NewGuid(),
                    IsDeveloper = false,
                    IsSysAdmin = false,
                    Login = "TestUser",
                    Password = "somepwd"
                };
            da.Insert(user);

            var u = da.GetUser("TestUser", "somepwd", null);
            Assert.NotNull(u);
            Assert.AreEqual(u.Id, user.Id);
            u = da.GetUser("TestUser", "wrong pwd", null);
            Assert.IsNull(u);

            u = da.GetUser("TestUser", null, null);
            Assert.NotNull(u);
            Assert.AreEqual(u.Id, user.Id);

            u = da.GetUser(null, null, user.Id);
            Assert.NotNull(u);
            Assert.AreEqual(u.Id, user.Id);
        }

        [Test]
        public void PerformanceTest()
        {
            int count = 1;
            var connectionString = Settings.MasterConnectionString;
            
            var users = new Dictionary<string, string>();
            for (int i = 0; i < count; i++)
            {
                var un = Guid.NewGuid().ToString();
                var pwd = Guid.NewGuid().ToString();

                using (var uow = new UnitOfWork(connectionString, true))
                {
                    var da = new UserDataAccess(uow);
                    var user = new User
                    {
                        Id = Guid.NewGuid(),
                        IsDeveloper = false,
                        IsSysAdmin = false,
                        Login = un,
                        Password = pwd
                    };
                    da.Insert(user);
                    uow.Commit();
                }
                users.Add(un, pwd);
                if ((i+1) % 1000 == 0)
                    Debug.WriteLine("Created: " + (i+1));
            }

            var startTime = DateTime.Now;
            int counter = 0;
            foreach (var user in users)
            {
                using (var uow = new UnitOfWork(connectionString, true))
                {
                    var da = new UserDataAccess(uow);
                    
                    var u = da.GetUser(user.Key, user.Value,null);
                    if (u == null)
                        throw new Exception();
                }
                counter++;
                if (counter % 1000 == 0)
                    Debug.WriteLine("logged in: " + counter);
            }
            var endTime = DateTime.Now;
            var tt = (endTime - startTime).TotalSeconds;
            Debug.WriteLine("total ligging in time: " + tt);
        }
    }
}
