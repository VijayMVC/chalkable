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
    public class DisciplineTypeDataAccess : DataAccessBase<DisciplineType>
    {
        public DisciplineTypeDataAccess(UnitOfWork unitOfWork) : base(unitOfWork)
        {
        }
    }
}
