using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chalkable.Data.AcademicBenchmark.Model;
using Chalkable.Data.Common;

namespace Chalkable.Data.AcademicBenchmark.DataAccess
{
    public class DocumentDataAccess : DataAccessBase<Document, Guid>
    {
        public DocumentDataAccess(UnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public IList<Document> GetByAuthorityId(Guid authorityId)
        {
            return ExecuteStoredProcedureList<Document>("spGetDocuments", new Dictionary<string, object>
            {
                ["authorityId"] = authorityId
            });
        } 
    }
}
