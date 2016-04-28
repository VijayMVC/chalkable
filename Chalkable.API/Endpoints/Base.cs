namespace Chalkable.API.Endpoints
{
    public class Base
    {
        protected IConnector Connector { get; }

        public Base(IConnector connector)
        {
            Connector = connector;
        }
    }
}