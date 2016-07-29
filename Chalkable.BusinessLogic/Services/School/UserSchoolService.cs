using System.Collections.Generic;
using Chalkable.BusinessLogic.Security;
using Chalkable.Data.Common;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.School
{
    public interface IUserSchoolService
    {
        void Add(IList<UserSchool> userSchools);
        void Delete(IList<UserSchool> userSchools);
    }

    public class UserSchoolService : SchoolServiceBase, IUserSchoolService
    {
        public UserSchoolService(IServiceLocatorSchool serviceLocator) : base(serviceLocator)
        {
        }

        public void Add(IList<UserSchool> userSchools)
        {
            BaseSecurity.EnsureSysAdmin(Context);
            DoUpdate(u => new DataAccessBase<UserSchool>(u).Insert(userSchools));
        }

        public void Delete(IList<UserSchool> userSchools)
        {
            BaseSecurity.EnsureSysAdmin(Context);
            DoUpdate(u => new DataAccessBase<UserSchool>(u).Delete(userSchools));
        }
    }
}