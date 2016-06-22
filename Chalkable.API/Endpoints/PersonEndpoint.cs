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
        public async Task<StudentInfo> GetStudentInfo(int personId)
        {
            return await Connector.Get<StudentInfo>($"/Student/Info.json?personId={personId}");
        }
    }
}