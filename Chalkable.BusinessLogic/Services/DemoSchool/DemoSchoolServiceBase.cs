using Chalkable.BusinessLogic.Services.DemoSchool.Storage;
using Chalkable.BusinessLogic.Services.School;
using Chalkable.StiConnector.Connectors;

namespace Chalkable.BusinessLogic.Services.DemoSchool
{
    public class DemoSchoolServiceBase: SchoolServiceBase
    {

        public DemoStorage Storage { get; private set; }
        public DemoSchoolServiceBase(IServiceLocatorSchool serviceLocator, DemoStorage demoStorage) : base(serviceLocator)
        {
            Storage = demoStorage;
        }
    }


    public class DemoSisConnectedService : DemoSchoolServiceBase
    {
        public DemoSisConnectedService(IServiceLocatorSchool serviceLocator, DemoStorage storage)
            : base(serviceLocator, storage)
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
