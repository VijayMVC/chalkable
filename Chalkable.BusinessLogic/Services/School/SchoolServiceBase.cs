using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chalkable.Data.Common;

namespace Chalkable.BusinessLogic.Services.School
{

    public class SchoolServiceBase
    {
        protected IServiceLocatorMaster ServiceLocator { get; private set; }
        protected UserContext Context
        {
            get { return ServiceLocator.Context; }
        }

        public SchoolServiceBase(IServiceLocatorMaster serviceLocator)
        {
            ServiceLocator = serviceLocator;
        }
        protected UnitOfWork Read()
        {
            return new UnitOfWork(ServiceLocator.Context.SchoolConnectionString, false);
        }

        protected UnitOfWork Update()
        {
            return new UnitOfWork(ServiceLocator.Context.SchoolConnectionString, true);
        }

    }
}
