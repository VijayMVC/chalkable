using System.Collections.Generic;
using System.Linq;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.DemoSchool.Storage
{
    public class DemoStudentSchoolYearStorage:BaseDemoIntStorage<StudentSchoolYear>
    {
        public DemoStudentSchoolYearStorage(DemoStorage storage) : base(storage, null, true)
        {
        }
        
        public IList<StudentSchoolYear> GetAll(int personId)
        {
            return data.Where(x => x.Value.StudentRef == personId).Select(x => x.Value).ToList();
        }

        public bool Exists(IList<int> gradeLevelIds, int personId)
        {
            return GetAll(personId).Count(x => gradeLevelIds.Contains(x.GradeLevelRef)) > 0;
        }

        public IList<StudentSchoolYear> GetList(int? schoolYearId, StudentEnrollmentStatusEnum? enrollmentStatus)
        {
            var ssYears = data.Select(x => x.Value);
            if (schoolYearId.HasValue)
                ssYears = ssYears.Where(x => x.SchoolYearRef == schoolYearId);
            if (enrollmentStatus.HasValue)
                ssYears = ssYears.Where(x => x.EnrollmentStatus == enrollmentStatus);
            return ssYears.ToList();
        }
    }
}
