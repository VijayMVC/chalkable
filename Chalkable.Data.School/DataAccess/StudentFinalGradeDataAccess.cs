using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chalkable.Data.Common;
using Chalkable.Data.School.Model;

namespace Chalkable.Data.School.DataAccess
{
    public class StudentFinalGradeDataAccess : DataAccessBase<StudentFinalGrade>
    {
        public StudentFinalGradeDataAccess(UnitOfWork unitOfWork) : base(unitOfWork)
        {
        }
    }
}
