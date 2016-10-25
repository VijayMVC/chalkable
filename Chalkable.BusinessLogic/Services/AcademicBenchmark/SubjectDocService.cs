using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chalkable.Data.AcademicBenchmark.DataAccess;
using Chalkable.Data.AcademicBenchmark.Model;

namespace Chalkable.BusinessLogic.Services.AcademicBenchmark
{
    public interface ISubjectDocService : IAcademicBenchmarkServiceBase<SubjectDoc, Guid>
    {
        IList<SubjectDoc> GetForStandards(Guid? authorityId, Guid? documentId);
        IList<SubjectDoc> GetForTopics();
    }

    public class SubjectDocService : AcademicBenchmarkServiceBase<SubjectDoc, Guid>, ISubjectDocService
    {
        public SubjectDocService(IAcademicBenchmarkServiceLocator locator) : base(locator)
        {
        }


        public IList<SubjectDoc> GetForStandards(Guid? authorityId, Guid? documentId)
        {
            return DoRead(u => new SubjectDocDataAccess(u).Get(authorityId, documentId));
        }

        public IList<SubjectDoc> GetForTopics()
        {
            return DoRead(u => new SubjectDocDataAccess(u).Get(null, null, true));
        }
    }
}
