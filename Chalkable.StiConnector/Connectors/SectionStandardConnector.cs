using System.Collections.Generic;
using Chalkable.StiConnector.Connectors.Model;

namespace Chalkable.StiConnector.Connectors
{
    public class SectionStandardConnector : ConnectorBase
    {
        public SectionStandardConnector(ConnectorLocator locator) : base(locator)
        {
        }
        
        public IList<ActivityStandard> GetStandards(int sectionId)
        {
            return Call<IList<ActivityStandard>>($"{BaseUrl}chalkable/sections/{sectionId}/standards");
        } 
    }
}
