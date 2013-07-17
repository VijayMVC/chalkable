using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chalkable.BusinessLogic.Services;
using Chalkable.BusinessLogic.Services.Master;

namespace Chalkable.Tests.Services.Master
{
    public class BaseMasterServiceLocatorTest : ServiceLocatorMaster
    {
        public BaseMasterServiceLocatorTest(UserContext context) : base(context)
        {
            StorageBlobService = new TestBlobStorageService();
        }
    }
}
