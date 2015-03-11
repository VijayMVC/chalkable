using System.Collections.Generic;
using System.Linq;
using Chalkable.Data.Common;
using Chalkable.Data.School.Model;

namespace Chalkable.Data.School.DataAccess
{
    public class AddressDataAccess : DataAccessBase<Address, int>
    {
        public AddressDataAccess(UnitOfWork unitOfWork)
            : base(unitOfWork)
        {
        }

        public void Delete(IList<int> ids)
        {
            SimpleDelete(ids.Select(x=> new Address{Id = x}).ToList());
        }
    }
}
