using System.Collections.Generic;
using System.Linq;
using Chalkable.Data.Common;
using Chalkable.Data.Common.Orm;
using Chalkable.Data.School.Model;

namespace Chalkable.Data.School.DataAccess
{
    public class AddressDataAccess : DataAccessBase<Address, int>
    {
        public AddressDataAccess(UnitOfWork unitOfWork)
            : base(unitOfWork)
        {
        }

        public IList<Address> GetAddress(int personId)
        {
            var dbQuery = new DbQuery();
            var personTName = "Person";
            dbQuery.Sql.AppendFormat(@"select [{0}].* from [{0}] 
                                       join [{1}] on [{1}].[{2}] = [{0}].[{3}]"
                , "Address", personTName, Person.ADDRESS_REF_FIELD, Address.ID_FIELD);
            var conds = new AndQueryCondition { { Person.ID_FIELD, personId } };
            conds.BuildSqlWhere(dbQuery, personTName);
            return ReadMany<Address>(dbQuery);
        }

        public void Delete(IList<int> ids)
        {
            SimpleDelete<Address>(ids.Select(x=> new Address{Id = x}).ToList());
        }
    }
}
