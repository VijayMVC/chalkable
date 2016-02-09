using System;
using System.Net;
using System.Threading.Tasks;

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
            return await Connector.Call<bool>($"{url}?id={standardId}&score={score}", method: WebRequestMethods.Http.Post);
        }
    }
}