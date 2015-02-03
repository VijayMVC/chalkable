using System.Collections.Generic;
using Chalkable.Data.School.DataAccess;
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
            using (var uow = Update())
            {
                var da = new UserSchoolDataAccess(uow);
                da.Insert(userSchools);
                uow.Commit();
            }
        }

        public void Delete(IList<UserSchool> userSchools)
        {
            using (var uow = Update())
            {
                var da = new UserSchoolDataAccess(uow);
                da.Delete(userSchools);
                uow.Commit();
            }
        }
    }
}