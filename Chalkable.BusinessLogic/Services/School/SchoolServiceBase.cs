using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chalkable.Data.Common;
using Chalkable.StiConnector.Connectors;

namespace Chalkable.BusinessLogic.Services.School
{
    public class SchoolServiceBase
    {
        protected IServiceLocatorSchool ServiceLocator { get; private set; }
        protected UserContext Context
        {
            get { return ServiceLocator.Context; }
        }

        public SchoolServiceBase(IServiceLocatorSchool serviceLocator)
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

    public class SisConnectedService : SchoolServiceBase
    {
        public SisConnectedService(IServiceLocatorSchool serviceLocator) : base(serviceLocator)
        {
            ConnectorLocator = new ConnectorLocator(Context.SisToken, Context.SisUrl);
        }

        protected ConnectorLocator ConnectorLocator { get; private set; }
    }
}
