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
            Put($"{BaseUrl}chalkable/sections/{sectionId}/options", classroomOption);
        }

        
        public ClassroomOption GetClassroomOption(int sectionId)
        {
            return Call<ClassroomOption>($"{BaseUrl}chalkable/sections/{sectionId}/options");
        }
    }
}
