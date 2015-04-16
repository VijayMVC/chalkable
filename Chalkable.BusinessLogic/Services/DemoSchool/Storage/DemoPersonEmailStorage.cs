using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.DemoSchool.Storage
{
    public class DemoPersonEmailStorage : BaseDemoIntStorage<PersonEmail>
    {
        public DemoPersonEmailStorage(DemoStorage storage) : base(storage, null, true)
        {
        }

    }
}
