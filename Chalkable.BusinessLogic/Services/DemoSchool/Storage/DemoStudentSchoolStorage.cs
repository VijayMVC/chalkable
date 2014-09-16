using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.DemoSchool.Storage
{
    public class DemoStudentSchoolStorage : BaseDemoIntStorage<StudentSchool>
    {
        public DemoStudentSchoolStorage(DemoStorage storage) 
            : base(storage, null, true)
        {
        }
    }
}