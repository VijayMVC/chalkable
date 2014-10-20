using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.DemoSchool.Storage
{
    public class DemoStudentStorage : BaseDemoIntStorage<Student>
    {
        public DemoStudentStorage(DemoStorage storage) 
            : base(storage, x => x.Id, true)
        {
        }
    }
}
