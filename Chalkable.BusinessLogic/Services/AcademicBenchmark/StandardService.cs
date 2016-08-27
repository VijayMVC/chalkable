using System;
using System.Collections.Generic;
using Chalkable.BusinessLogic.Security;
using Chalkable.Common;
using Chalkable.Data.AcademicBenchmark.DataAccess;
using Chalkable.Data.AcademicBenchmark.Model;

namespace Chalkable.BusinessLogic.Services.AcademicBenchmark
{
    public interface IStandardService : IAcademicBenchmarkServiceBase<Chalkable.Data.AcademicBenchmark.Model.Standard, Guid>
    {
        StandardRelations GetStandardRelations(Guid id);
        PaginatedList<Standard> Search(string searchQuery, bool? deepest = null, int start = 0, int count = int.MaxValue);
        IList<Standard> Get(Guid? authorityId, Guid? documentId, Guid? subjectDocId, string gradeLevelCode, Guid? parentId, Guid? courseId, bool? deepest);
    }

    public class StandardService : AcademicBenchmarkServiceBase<Standard, Guid>, IStandardService 
    {
        public StandardService(IAcademicBenchmarkServiceLocator locator) : base(locator)
        {
        }

        public override void Delete(IList<Standard> models)
        {
            BaseSecurity.EnsureSysAdmin(Context);
            DoUpdate(u => new StandardDataAccess(u).Delete(models));
        }

        public StandardRelations GetStandardRelations(Guid id)
        {
            return DoRead(u => new StandardDataAccess(u).GetStandardRelations(id));
        }

        public PaginatedList<Chalkable.Data.AcademicBenchmark.Model.Standard> Search(string searchQuery, bool? deepest = null, int start = 0, int count = int.MaxValue)
        {
            return DoRead(u => new StandardDataAccess(u).Search(searchQuery, deepest, start, count));
        }

        public IList<Chalkable.Data.AcademicBenchmark.Model.Standard> Get(Guid? authorityId, Guid? documentId, Guid? subjectDocId, string gradeLevelCode, Guid? parentId, Guid? courseId,
            bool? deepest)
        {
            return DoRead(u => new StandardDataAccess(u).Get(authorityId, documentId, subjectDocId, gradeLevelCode, parentId, courseId, deepest));
        }
    }
}
