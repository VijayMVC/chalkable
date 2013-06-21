using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chalkable.Data.Common;
using Chalkable.Data.School.Model;

namespace Chalkable.Data.School.DataAccess
{
    public class PhoneDataAccess : DataAccessBase
    {
        public PhoneDataAccess(UnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public void Create(Phone phone)
        {
            SimpleInsert(phone);
        }
        public void Update(Phone phone)
        {
            SimpleUpdate(phone);
        }
        public void Delete(Phone phone)
        {
            SimpleDelete(phone);
        }
        public Phone GetById(Guid id)
        {
            var conds = new Dictionary<string, object> {{"Id", id}};
            return SelectOne<Phone>(conds);
        }
        public IList<Phone> GetList()
        {
            return SelectMany<Phone>(new Dictionary<string, object>());
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
