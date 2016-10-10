using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chalkable.Data.AcademicBenchmark.Model;
using Chalkable.Data.Common;

namespace Chalkable.Data.AcademicBenchmark.DataAccess
{
    public class GradeLevelDataAccess : DataAccessBase<GradeLevel>
    {
        public GradeLevelDataAccess(UnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public IList<GradeLevel> Get(Guid? authorityId, Guid? documentId, Guid? subjectDocId)
        {
            var @params = new Dictionary<string, object>
            {
                ["authorityId"] = authorityId,
                ["documentId"] = documentId,
                ["subjectDocId"] = subjectDocId
            };

            return ExecuteStoredProcedureList<GradeLevel>("spGetGradeLevels", @params);
        } 
    }
}
