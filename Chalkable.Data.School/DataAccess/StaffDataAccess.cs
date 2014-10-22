using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chalkable.Data.Common;
using Chalkable.Data.School.Model;

namespace Chalkable.Data.School.DataAccess
{
    public class StaffDataAccess : DataAccessBase<Staff, int>
    {
        public StaffDataAccess(UnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public void Delete(IList<Staff> staffs)
        {
            SimpleDelete(staffs);
        }
    }
}
