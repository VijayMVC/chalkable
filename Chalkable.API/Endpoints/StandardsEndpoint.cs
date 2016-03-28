using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Chalkable.API.Helpers;
using Chalkable.API.Models.AcademicBenchmark;

namespace Chalkable.API.Endpoints
{
    public class StandardsEndpoint : Base
    {
        public StandardsEndpoint(IConnector connector) : base(connector)
        {
        }
        //TODO: remove Oauth from these endpoints  
        public async Task<IList<Standard>> GetStandardsByIds(IList<Guid> standardsIds)
        {
            var url = "/AcademicBenchmark/StandardsByIds.json";
            var strIds = standardsIds.JoinString(",", x => x.ToString());
            return await Connector.Get<IList<Standard>>($"{url}?standardsIds={strIds}");
        }

        public async Task<IList<StandardRelations>> GetListOfStandardRelations(IList<Guid> standardsIds)
        {
            var url = "/AcademicBenchmark/ListOfStandardRelationsByIds.json";
            var strIds = standardsIds.JoinString(",", x => x.ToString());
            return await Connector.Get<IList<StandardRelations>>($"{url}?standardsIds={strIds}");
        }

        public async Task<IList<Topic>> GetTopicsByIds(IList<Guid> topicsIds)
        {
            var url = "/AcademicBenchmark/TopicsByIds.json";
            var strIds = topicsIds.JoinString(",", x => x.ToString());
            return await Connector.Get<IList<Topic>>($"{url}?topicsIds={strIds}");
        }
    }
}
