using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chalkable.Data.AcademicBenchmark.DataAccess;
using Chalkable.Data.AcademicBenchmark.Model;

namespace Chalkable.BusinessLogic.Services.AcademicBenchmark
{
    public interface IGradeLevelService : IAcademicBenchmarkServiceBase<GradeLevel, string>
    {
        IList<GradeLevel> Get(Guid? authorityId, Guid? documentId, Guid? subjectDocId);
    }
    public class GradeLevelService : AcademicBenchmarkServiceBase<GradeLevel, string>, IGradeLevelService
    {
        public GradeLevelService(IAcademicBenchmarkServiceLocator locator) : base(locator)
        {
        }

        public IList<GradeLevel> Get(Guid? authorityId, Guid? documentId, Guid? subjectDocId)
        {
            if (!authorityId.HasValue && !documentId.HasValue && !subjectDocId.HasValue)
                return GetAll();

            return DoRead(u => new GradeLevelDataAccess(u).Get(authorityId, documentId, subjectDocId));
        }
    }
}
