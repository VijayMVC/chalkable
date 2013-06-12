using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using Chalkable.BusinessLogic.Model;
using Chalkable.BusinessLogic.Services;
using Chalkable.Data.Master.Model;
using NUnit.Framework;

namespace Chalkable.Tests.Services.Master
{
    public class SchoolServiceTest : ServiceTestBase
    {
        [Test]
        public void CreateSchoolTest()
        {
            var sl = ServiceLocatorFactory.CreateMasterSysAdmin();
            sl.SchoolService.CreateEmpty();
            var distr = sl.SchoolService.CreateDistrict("some distr");
            var principals = new List<UserInfo>
                {
                    new UserInfo
                        {
                            BirthDate = null,
                            FirstName = "John",
                            Gender = "M",
                            LastName = "Smith",
                            Login = "jsmith@chalkable.com",
                            Password = "qwerty",
                            Salutation = "Mr"
                        }
                };
            School school = null;
            for (int i = 0; i <= 300;i++ )
            {
                Debug.WriteLine("Time: " + i * 10);
                school = sl.SchoolService.Create(distr.Id, "test school", principals);
                if (school != null)
                    break;
                Thread.Sleep(10000);
                
            }
            Assert.IsNotNull(school);
            
        }
    }
}