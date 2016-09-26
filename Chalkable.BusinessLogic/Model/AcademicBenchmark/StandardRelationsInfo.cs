using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chalkable.BusinessLogic.Model.AcademicBenchmark
{
    public class StandardRelationsInfo
    {
        public StandardInfo Standard { get; set; }
        public IList<StandardInfo> Origins { get; set; } 
        public IList<StandardInfo> Derivatives { get; set; }
        public IList<StandardInfo> RelatedDerivatives { get; set; }


        public static StandardRelationsInfo Create(
            Data.AcademicBenchmark.Model.StandardRelations relations,
            IList<Data.AcademicBenchmark.Model.Authority> authorities,
            IList<Data.AcademicBenchmark.Model.Document> documents)
        {
            if(relations == null)
                return new StandardRelationsInfo();

            var document = documents.First(x => x.Id == relations.Standard.DocumentRef);
            var authority = authorities.First(x => x.Id == relations.Standard.AuthorityRef);

            return new StandardRelationsInfo
            {
                Standard = StandardInfo.Create(relations.Standard, authority, document),
                Derivatives = StandardInfo.Create(relations.Derivatives, authorities, documents),
                Origins = StandardInfo.Create(relations.Origins, authorities, documents),
                RelatedDerivatives = StandardInfo.Create(relations.RelatedDerivatives, authorities, documents)
            };
        }
    }
}
