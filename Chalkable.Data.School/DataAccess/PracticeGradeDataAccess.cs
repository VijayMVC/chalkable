using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chalkable.Data.Common;
using Chalkable.Data.Common.Orm;
using Chalkable.Data.School.Model;

namespace Chalkable.Data.School.DataAccess
{
    public class PracticeGradeDataAccess : DataAccessBase<PracticeGrade, int>
    {
        public PracticeGradeDataAccess(UnitOfWork unitOfWork) : base(unitOfWork)
        {
        }
    }
}
