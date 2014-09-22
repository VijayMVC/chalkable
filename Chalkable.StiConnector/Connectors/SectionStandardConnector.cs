using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            return Call<IList<ActivityStandard>>(string.Format(BaseUrl + "chalkable/sections/{0}/standards", sectionId));
        } 
    }
}
