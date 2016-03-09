namespace Chalkable.AcademicBenchmarkConnector.Connectors
{
    public interface ITopicsConnector
    {
         
    }

    public class TopicsConnector : ConnectorBase, ITopicsConnector
    {
        public TopicsConnector(IConnectorLocator connectorLocator) : base(connectorLocator)
        {
        }

        
    }
}
