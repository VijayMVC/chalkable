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
        public async Task<SchoolPerson> GetStudentInfo(int personId)
        {
            return await Connector.Get<SchoolPerson>($"/Student/Info.json?personId={personId}");
        }
    }
}