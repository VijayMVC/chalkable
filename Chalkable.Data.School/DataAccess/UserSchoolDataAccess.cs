using System.Collections.Generic;
using Chalkable.Data.Common;
using Chalkable.Data.School.Model;

namespace Chalkable.Data.School.DataAccess
{
    public class UserSchoolDataAccess : DataAccessBase<UserSchool, int>
    {
        public UserSchoolDataAccess(UnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public void Delete(IList<UserSchool> userSchools)
        {
            SimpleDelete(userSchools);
        }
    }
}