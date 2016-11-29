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
    public class StudentCustomAlertDetailDataAccess : DataAccessBase<StudentCustomAlertDetail, int>
    {
        public StudentCustomAlertDetailDataAccess(UnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public IList<StudentCustomAlertDetail> GetList(IList<int> studentsIds, int schoolYear)
        {
            var sql = string.Format($@"select * from {nameof(StudentCustomAlertDetail)} 
                                        where 
	                                        {nameof(StudentCustomAlertDetail.SchoolYearRef)} = @{nameof(schoolYear)}
	                                        and
                                            {nameof(StudentCustomAlertDetail.StudentRef)} in (select * from @{nameof(studentsIds)})");

            var ps = new Dictionary<string, object> { { nameof(schoolYear), schoolYear }, { nameof(studentsIds), studentsIds } };

            return ReadMany<StudentCustomAlertDetail>(new DbQuery(sql, ps)).OrderBy(x => x.AlertText).ToList();
        }
    }
}
