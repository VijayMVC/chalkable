using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Chalkable.API.Helpers;
using Chalkable.API.Models;

namespace Chalkable.API.Endpoints
{
    public class AcademicBenchmarkEndpoint : Base
    {
        public AcademicBenchmarkEndpoint(IConnector connector) : base(connector)
        {
        }

        public async Task<IList<AcademicBenchmarkStandard>> GetStandardsByIds(IList<Guid> standardsIds)
        {
            var url = "/AcademicBenchmark/StandatdsIds.json";
            var strIds = standardsIds.Select(x => x.ToString()).JoinString(",");
            return await Connector.Get<IList<AcademicBenchmarkStandard>>($"{url}?standardsIds={strIds}");
        }

        public async Task<IList<AcademicBenchmarkStandardRelations>> GetListOfStandardRelastions(IList<Guid> standardsIds)
        {
            var url = "/AcademicBenchmark/ListOfStandardRelationsByIds.json";
            var strIds = standardsIds.Select(x => x.ToString()).JoinString(",");
            return await Connector.Get<IList<AcademicBenchmarkStandardRelations>>($"{url}?standardsIds={strIds}");
        }
    }
}
