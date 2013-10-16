using Chalkable.Data.Common;
using Chalkable.Data.School.Model;

namespace Chalkable.Data.School.DataAccess
{
    public class AddressDataAccess : DataAccessBase<Address>
    {
        public AddressDataAccess(UnitOfWork unitOfWork) : base(unitOfWork)
        {
        }
    }
}
