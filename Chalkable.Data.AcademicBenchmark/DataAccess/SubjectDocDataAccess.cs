using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chalkable.Data.AcademicBenchmark.Model;
using Chalkable.Data.Common;

namespace Chalkable.Data.AcademicBenchmark.DataAccess
{
    public class SubjectDocDataAccess : DataAccessBase<SubjectDoc, Guid>
    {
        public SubjectDocDataAccess(UnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public IList<SubjectDoc> Get(Guid? authorityId, Guid? documentId, bool forTopics = false)
        {
            var @params = new Dictionary<string, object>
            {
                ["authorityId"] = authorityId,
                ["documentId"] = documentId,
                ["forTopics"] = forTopics
            };

            return ExecuteStoredProcedureList<SubjectDoc>("spGetSubjectDocs", @params);
        }
    }
}
