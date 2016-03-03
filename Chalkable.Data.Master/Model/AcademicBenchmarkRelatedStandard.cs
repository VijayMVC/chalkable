using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chalkable.Data.Master.Model
{
    public class AcademicBenchmarkRelatedStandard
    {
        public Guid Id { get; set; }
        public string Description { get; set; }
        public int AuthorityId { get; set; }
        public int DocumentId { get; set; }
        public int ParentId { get; set; }
        public IList<AcademicBenchmarkStandard> RelatedStandard { get; set; }
    }
}
