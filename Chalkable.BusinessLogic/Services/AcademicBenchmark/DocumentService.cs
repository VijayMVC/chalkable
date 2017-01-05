using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chalkable.Data.AcademicBenchmark.DataAccess;
using Chalkable.Data.AcademicBenchmark.Model;

namespace Chalkable.BusinessLogic.Services.AcademicBenchmark
{
    public interface IDocumentService : IAcademicBenchmarkServiceBase<Document, Guid>
    {
        IList<Document> GetByAuthority(Guid? authorityId);
    }

    public class DocumentService: AcademicBenchmarkServiceBase<Document, Guid>, IDocumentService
    {
        public DocumentService(IAcademicBenchmarkServiceLocator locator) : base(locator)
        {
        }


        public IList<Document> GetByAuthority(Guid? authorityId)
        {
            if (!authorityId.HasValue)
                return GetAll();

            return DoRead(u => new DocumentDataAccess(u).GetByAuthorityId(authorityId.Value));
        }
    }
}
