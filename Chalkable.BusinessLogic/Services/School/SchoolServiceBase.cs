using System;
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
            return ServiceLocator.SchoolDbService.GetUowForRead();
        }

        protected UnitOfWork Update()
        {
            return ServiceLocator.SchoolDbService.GetUowForUpdate();
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
                    connectorLocator = new ConnectorLocator(Context.SisToken, Context.SisUrl, Context.SisTokenExpires.Value);
                return connectorLocator;
            }
        }
    }
}
