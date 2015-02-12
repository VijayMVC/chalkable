using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chalkable.Data.Common;
using Chalkable.Data.Common.Orm;
using Chalkable.Data.School.Model;

namespace Chalkable.Data.School.DataAccess
{
    public class AutoGradeDataAccess : DataAccessBase<AutoGrade, int>
    {
        public AutoGradeDataAccess(UnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public AutoGrade GetAutoGrade(int announcementApplicationId, int studentId)
        {
           var res = Orm.SimpleSelect<AutoGrade>(new AndQueryCondition
                {
                    {AutoGrade.ANNOUNCEMENT_APPLICATION_REF_FIELD, announcementApplicationId},
                    {AutoGrade.STUDENT_REF_FIELD, studentId}
                });
            return ReadOne<AutoGrade>(res);
        }
    }
}
