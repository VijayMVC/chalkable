namespace Chalkable.StiConnector.Connectors
{
    public class ConnectorLocator
    {
        public ConnectorLocator(string userName, string password, string baseUrl)
        {
            SchoolConnector = new SchoolConnector(userName, password, baseUrl);
            AcadSessionConnector = new AcadSessionConnector(userName, password, baseUrl);
            StudentConnector = new StudentConnector(userName, password, baseUrl);
            GradeLevelConnector = new GradeLevelConnector(userName, password, baseUrl);
            GenderConnector = new GenderConnector(userName, password, baseUrl);
            ContactConnector = new ContactConnector(userName, password, baseUrl);
        }

        public SchoolConnector SchoolConnector { get; private set; }
        public AcadSessionConnector AcadSessionConnector { get; private set; }
        public StudentConnector StudentConnector { get; private set; }
        public GradeLevelConnector GradeLevelConnector { get; private set; }
        public GenderConnector GenderConnector { get; private set; }
        public ContactConnector ContactConnector { get; private set; }
    }
}