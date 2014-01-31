using System.Collections.Generic;
using System.Linq;
using Chalkable.StiConnector.Connectors.Model;

namespace Chalkable.StiConnector.Connectors
{
    public class SchoolConnector : ConnectorBase
    {
        public SchoolConnector(ConnectorLocator locator) : base(locator)
        {
        }

        public List<School> GetSchools()
        {
            return Call<School[]>(BaseUrl + "schools").ToList();
        }

        public School GetSchoolDetails(int id)
        {
            return Call<School>(BaseUrl + "schools/" + id);
        }
    }
}