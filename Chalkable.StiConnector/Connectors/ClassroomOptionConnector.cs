using Chalkable.StiConnector.Connectors.Model;

namespace Chalkable.StiConnector.Connectors
{
    public class ClassroomOptionConnector : ConnectorBase
    {
        public ClassroomOptionConnector(ConnectorLocator locator) : base(locator)
        {
        }

        public void UpdateClassroomOption(int sectionId, ClassroomOption classroomOption)
        {
            Put(string.Format("{0}chalkable/sections/{1}/options", BaseUrl, sectionId), classroomOption);
        }

        public ClassroomOption GetClassroomOption(int sectionId)
        {
            return Call<ClassroomOption>(string.Format("{0}chalkable/sections/{1}/options", BaseUrl, sectionId));
        }
    }
}
