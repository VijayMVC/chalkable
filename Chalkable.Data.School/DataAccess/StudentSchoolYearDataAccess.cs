using System.Collections.Generic;
using Chalkable.Data.Common;
using Chalkable.Data.Common.Orm;
using Chalkable.Data.School.Model;

namespace Chalkable.Data.School.DataAccess
{
    public class StudentSchoolYearDataAccess : DataAccessBase<StudentSchoolYear, int>
    {
        public StudentSchoolYearDataAccess(UnitOfWork unitOfWork)
            : base(unitOfWork)
        {
        }

        public void Delete(IList<StudentSchoolYear> studentSchoolYears)
        {
            SimpleDelete(studentSchoolYears);
        }

        public IList<StudentSchoolYear> GetList(int? schoolYearId, StudentEnrollmentStatusEnum? enrollmentStatus)
        {
            var conds = new AndQueryCondition();
            if(schoolYearId.HasValue)
                conds.Add(StudentSchoolYear.SCHOOL_YEAR_REF_FIELD, schoolYearId);
            if(enrollmentStatus.HasValue)
                conds.Add(StudentSchoolYear.ENROLLMENT_STATUS_FIELD, enrollmentStatus.Value);
            return GetAll(conds);
        }
    }
}
