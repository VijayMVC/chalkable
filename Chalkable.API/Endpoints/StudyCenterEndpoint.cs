using System;
using System.Threading.Tasks;
using System.Web;

namespace Chalkable.API.Endpoints
{
    public class StudyCenterEndpoint : Base
    {
        public StudyCenterEndpoint(IConnector connector) : base(connector)
        {
        }

        public async Task<bool> SetPracticeGrade(Guid standardId, string score)
        {
            var url = "/StudyCenter/SetPracticeGrade.json";
            
            var nvc = HttpUtility.ParseQueryString(string.Empty);
            nvc.Add("id", standardId.ToString());
            nvc.Add("score", score);

            return await Connector.Post<bool>(url, nvc);
        }
    }
}