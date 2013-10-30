using System.Collections.Generic;
using System.Linq;
using Chalkable.Data.Common;
using Chalkable.Data.Common.Orm;
using Chalkable.Data.School.Model;

namespace Chalkable.Data.School.DataAccess
{
    public class PhoneDataAccess : DataAccessBase<Phone, int>
    {
        public PhoneDataAccess(UnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public IList<Person> GetUsersByPhone(string phone)
        {
            var phoneTName = "Phone";
            var query = new DbQuery();
            query.Sql.AppendFormat(@"select [{1}].* from [{0}] 
                                     join [{1}] on [{1}].[{2}] = [{0}].[{3}]"
                , "Phone", phoneTName, Person.ID_FIELD, Phone.PERSON_REF_FIELD);
            var conds = new AndQueryCondition {{Phone.DIGIT_ONLY_VALUE_FIELD, phone}};
            conds.BuildSqlWhere(query, phoneTName);
            return ReadMany<Person>(query);
        } 

        public Phone GetPhone(int personId, string digitOnlyValue)
        {
            var conds = new AndQueryCondition();
            conds.Add(Phone.PERSON_REF_FIELD, personId);
            conds.Add(Phone.DIGIT_ONLY_VALUE_FIELD, digitOnlyValue);
            return GetAll(conds).First();
        }

        public void Delete(Phone phone)
        {
            var conds = new AndQueryCondition();
            conds.Add(Phone.DIGIT_ONLY_VALUE_FIELD, phone.DigitOnlyValue);
            conds.Add(Phone.PERSON_REF_FIELD, phone.PersonRef);
            SimpleDelete(conds);
        }
    }
}
