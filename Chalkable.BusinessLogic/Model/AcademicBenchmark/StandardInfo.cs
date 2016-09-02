using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chalkable.Common;

namespace Chalkable.BusinessLogic.Model.AcademicBenchmark
{
    public class StandardInfo : ShortStandard
    {
        public Data.AcademicBenchmark.Model.Authority Authority { get; set; }
        public Data.AcademicBenchmark.Model.Document Document { get; set; }

        public static StandardInfo Create(Data.AcademicBenchmark.Model.Standard model,
            Data.AcademicBenchmark.Model.Authority authority,
            Data.AcademicBenchmark.Model.Document document)
        {
            return new StandardInfo
            {
                Id = model.Id,
                Description = model.Description,
                Code = model.Number,
                IsActive = model.IsActive,
                IsDeepest = model.IsDeepest,
                Level = model.Level,
                ParentId = model.ParentRef,
                Authority = authority,
                Document = document
            };
        }

        public static IList<StandardInfo> Create(IList<Data.AcademicBenchmark.Model.Standard> models,
            IList<Data.AcademicBenchmark.Model.Authority> authorities,
            IList<Data.AcademicBenchmark.Model.Document> documents)
        {
            var result = new List<StandardInfo>();
            foreach (var model in models)
            {
                var authority = authorities.First(x => x.Id == model.AuthorityRef);
                var document = documents.First(x => x.Id == model.DocumentRef);
                result.Add(Create(model, authority, document));
            }

            return result;
        }

        public static PaginatedList<StandardInfo> Create(PaginatedList<Data.AcademicBenchmark.Model.Standard> models,
            IList<Data.AcademicBenchmark.Model.Authority> authorities,
            IList<Data.AcademicBenchmark.Model.Document> documents)
        {
            return models.Transform(x =>
            {
                var authority = authorities.First(y => y.Id == x.AuthorityRef);
                var document = documents.First(y => y.Id == x.DocumentRef);
                return Create(x, authority, document);
            });
        } 
    }
}
