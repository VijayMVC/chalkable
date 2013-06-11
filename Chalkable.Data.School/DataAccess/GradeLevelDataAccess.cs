using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chalkable.Data.Common;
using Chalkable.Data.School.Model;

namespace Chalkable.Data.School.DataAccess
{
    public class GradeLevelDataAccess : DataAccessBase
    {
        public GradeLevelDataAccess(UnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public IList<GradeLevel> GetGradeLeveles()
        {
            var sql = "select * from GradeLevel";
            var conds = new Dictionary<string, object>();
            using (var reader = ExecuteReaderParametrized(sql, conds))
            {
                return reader.ReadList<GradeLevel>();
            }
        }
    }
}
