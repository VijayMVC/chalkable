using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chalkable.BusinessLogic.Services;
using Chalkable.BusinessLogic.Services.Master;
using Chalkable.BusinessLogic.Services.School;

namespace Chalkable.Tests.Services.School
{
    public class BaseSchoolServiceLocatorTest : ServiceLocatorSchool
    {
        public BaseSchoolServiceLocatorTest(IServiceLocatorMaster serviceLocatorMaster) : base(serviceLocatorMaster)
        {
            StorageBlobService = new StorageBlobService();
        }
    }
}
