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
        }

        private ConnectorLocator connectorLocator;
        protected ConnectorLocator ConnectorLocator
        {
            get
            {
                if (connectorLocator == null)
                    connectorLocator = new ConnectorLocator(Context.SisToken, Context.SisUrl);
                return connectorLocator;
            }
        }
    }
}
