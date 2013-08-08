using System;
using Chalkable.BusinessLogic.Services.Master;
using Chalkable.BusinessLogic.Services.School;
using Chalkable.Common;
using Chalkable.Data.Master.Model;
using Chalkable.Tests.Services.Master;
using Chalkable.Tests.Services.School;

namespace Chalkable.Tests.Services.TestContext
{
    public class DeveloperSchoolTestContex : SchoolTestContext
    {
        public const string DEVELOPER_USER = "Devloper";
        private const string DEVELOPER_WEB_SITE = "http://testDeveloper.com";
        private Guid developerId;
        private IServiceLocatorMaster masterLocator;

        public Developer Developer
        {
            get { return masterLocator.DeveloperService.GetDeveloperById(developerId); }
        }
        public IServiceLocatorSchool DeveloperSl { get; set; }
        public IServiceLocatorMaster DeveloperMl { get { return DeveloperSl.ServiceLocatorMaster; } }


        protected DeveloperSchoolTestContex(IServiceLocatorSchool sysSchoolSl)
            : base(sysSchoolSl)
        {
            var userInfo = CreateUserInfo(DEVELOPER_USER, CoreRoles.DEVELOPER_ROLE);
            masterLocator = sysSchoolSl.ServiceLocatorMaster;
            var developer = masterLocator.DeveloperService.Add(userInfo.Login, userInfo.Password, userInfo.FirstName,
                             DEVELOPER_WEB_SITE, sysSchoolSl.Context.SchoolId.Value);
            developerId = developer.Id;
            var devContext = masterLocator.UserService.Login(userInfo.Login, userInfo.Password);
            DeveloperSl = new BaseSchoolServiceLocatorTest(new BaseMasterServiceLocatorTest(devContext));
        }

        public new static DeveloperSchoolTestContex Create(IServiceLocatorSchool sysSchoolSl)
        {
            return new DeveloperSchoolTestContex(sysSchoolSl);
        }
    }
}
