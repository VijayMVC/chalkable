using System.Collections.Generic;
using System.Linq;
using Chalkable.Data.Common;
using Chalkable.Data.Common.Orm;
using Chalkable.Data.School.Model;

namespace Chalkable.Data.School.DataAccess
{
    public class StudentCustomAlertDetailDataAccess : DataAccessBase<StudentCustomAlertDetail, int>
    {
        public StudentCustomAlertDetailDataAccess(UnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public IList<StudentCustomAlertDetail> GetList(IList<int> studentsIds, int schoolYear)
        {
            if(studentsIds?.Count == 0) return new List<StudentCustomAlertDetail>();
            var conds = new AndQueryCondition {{nameof(StudentCustomAlertDetail.SchoolYearRef), schoolYear}};
            var dbQuery = Orm.SimpleSelect<StudentCustomAlertDetail>(conds);

            dbQuery.Sql.Append($" And {nameof(StudentCustomAlertDetail.StudentRef)} in (select * from @{nameof(studentsIds)})");
            dbQuery.Parameters.Add($"{nameof(studentsIds)}", studentsIds);
            return ReadMany<StudentCustomAlertDetail>(dbQuery).OrderBy(x => x.AlertText).ToList();
        }
    }
}
