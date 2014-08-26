using System.Collections.Generic;
using System.Linq;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.DemoSchool.Storage
{
    public class DemoStudentHealthConditionStorage : BaseDemoIntStorage<StudentHealthCondition>
    {
        public DemoStudentHealthConditionStorage(DemoStorage storage)
            : base(storage, x => x.Id)
        {
        }


        public IList<StudentHealthCondition> GetByStudentId(int studentId)
        {
            return data.Select(x => x.Value).ToList();
        } 
    }
}
