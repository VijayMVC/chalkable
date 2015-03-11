using System.Collections.Generic;
using Chalkable.Data.Common;
using Chalkable.Data.School.Model;

namespace Chalkable.Data.School.DataAccess
{
    public class PersonEmailDataAccess : DataAccessBase<PersonEmail, int>
    {
        public PersonEmailDataAccess(UnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public void Delete(IList<PersonEmail> personEmails)
        {
            SimpleDelete(personEmails);
        }
    }
}
