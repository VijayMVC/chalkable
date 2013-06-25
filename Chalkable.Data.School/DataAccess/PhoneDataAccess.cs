using System.Collections.Generic;
using Chalkable.Data.Common;
using Chalkable.Data.School.Model;

namespace Chalkable.Data.School.DataAccess
{
    public class PhoneDataAccess : DataAccessBase<Phone>
    {
        public PhoneDataAccess(UnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public IList<Person> GetUsersByPhone(string phone)
        {
            var sql = @"select Person.* from Phone 
                      join Person p on p.Id = Phone.PersonRef
                      where Phone.DigitOnlyValue = @phone";
            var conds = new Dictionary<string, object> {{"phone", phone}};
            using (var reader = ExecuteReaderParametrized(sql, conds))
            {
                return reader.ReadList<Person>();
            }
        } 
    }
}
