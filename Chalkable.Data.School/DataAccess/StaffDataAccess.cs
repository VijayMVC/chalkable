using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chalkable.Common;
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
        
        public PaginatedList<Staff> SearchStaff(int? schoolYearId, int? classId, int? studentId, string filter, bool orderByFirstName, int start, int count)
        {
            var ps = new Dictionary<string, object>
            {
                {"@start", start},
                {"@count", count},
                {"@classId", classId},
                {"@studentId", studentId},
                {"@schoolYearId", schoolYearId},
                {"@filter", "%" + filter + "%"},
                {"@orderByFirstName", orderByFirstName}
            };
            return ExecuteStoredProcedurePaginated<Staff>("spSearchStaff", ps, start, count);
        }
    }
}
