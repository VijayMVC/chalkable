using System.Threading.Tasks;
using Chalkable.API.Models;

namespace Chalkable.API.Endpoints
{
    public class PersonEndpoint : Base
    {
        public PersonEndpoint(IConnector connector) : base(connector)
        {
        }

        public async Task<SchoolPerson> GetMe()
        {
            return await Connector.Get<SchoolPerson>("/Person/Me.json");
        }
    }
}