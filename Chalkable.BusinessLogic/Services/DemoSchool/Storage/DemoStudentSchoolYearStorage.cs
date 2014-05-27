using System.Collections.Generic;
using System.Linq;
using Chalkable.BusinessLogic.Services.DemoSchool.Common;
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

        public override void Setup()
        {
            Add(new StudentSchoolYear
            {
                SchoolYearRef = DemoSchoolConstants.CurrentSchoolYearId,
                StudentRef = DemoSchoolConstants.FirstStudentId,
                GradeLevel = Storage.GradeLevelStorage.GetById(DemoSchoolConstants.GradeLevel12),
                GradeLevelRef = DemoSchoolConstants.GradeLevel12
            });

            Add(new StudentSchoolYear
            {
                SchoolYearRef = DemoSchoolConstants.CurrentSchoolYearId,
                StudentRef = DemoSchoolConstants.SecondStudentId,
                GradeLevel = Storage.GradeLevelStorage.GetById(DemoSchoolConstants.GradeLevel12),
                GradeLevelRef = DemoSchoolConstants.GradeLevel12
            });

            Add(new StudentSchoolYear
            {
                SchoolYearRef = DemoSchoolConstants.CurrentSchoolYearId,
                StudentRef = DemoSchoolConstants.ThirdStudentId,
                GradeLevel = Storage.GradeLevelStorage.GetById(DemoSchoolConstants.GradeLevel12),
                GradeLevelRef = DemoSchoolConstants.GradeLevel12
            });
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
