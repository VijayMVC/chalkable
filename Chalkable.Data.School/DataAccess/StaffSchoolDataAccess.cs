using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chalkable.Data.Common;
using Chalkable.Data.School.Model;

namespace Chalkable.Data.School.DataAccess
{
    public class StaffSchoolDataAccess : BaseSchoolDataAccess<StaffSchool>
    {
        public StaffSchoolDataAccess(UnitOfWork unitOfWork, int? schoolId) : base(unitOfWork, schoolId)
        {
        }

        public void Delete(IList<StaffSchool> staffSchools)
        {
            SimpleDelete(staffSchools);
        }
    }
}
