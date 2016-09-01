﻿using System;
using System.Collections.Generic;
using System.Linq;
using Chalkable.BusinessLogic.Model.AcademicBenchmark;
using Chalkable.BusinessLogic.Security;
using Chalkable.Common;
using Chalkable.Data.AcademicBenchmark.DataAccess;
using Chalkable.Data.Common;
using Standard = Chalkable.Data.AcademicBenchmark.Model.Standard;
using StandardRelations = Chalkable.Data.AcademicBenchmark.Model.StandardRelations;

namespace Chalkable.BusinessLogic.Services.AcademicBenchmark
{
    public interface IStandardService : IAcademicBenchmarkServiceBase<Chalkable.Data.AcademicBenchmark.Model.Standard, Guid>
    {
        StandardRelations GetStandardRelations(Guid id);
        PaginatedList<StandardInfo> Search(string searchQuery, bool? deepest = null, int start = 0, int count = int.MaxValue);
        IList<StandardInfo> Get(Guid? authorityId, Guid? documentId, Guid? subjectDocId, string gradeLevelCode, Guid? parentId, Guid? courseId, bool firstLevelOnly = false);

        IList<StandardInfo> GetStandardInfosByIds(IList<Guid> ids);
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

        public PaginatedList<StandardInfo> Search(string searchQuery, bool? deepest = null, int start = 0, int count = int.MaxValue)
        {
            var standards = DoRead(u => new StandardDataAccess(u).Search(searchQuery, deepest, start, count));
            var docs = DoRead(u => new DocumentDataAccess(u).GetByIds(standards.Select(x => x.DocumentRef).ToList()));
            var authorities = DoRead(u => new DataAccessBase<Data.AcademicBenchmark.Model.Authority, Guid>(u)
                .GetByIds(standards.Select(x => x.AuthorityRef).ToList()));

            return StandardInfo.Create(standards, authorities, docs);
        }

        public IList<StandardInfo> Get(Guid? authorityId, Guid? documentId, Guid? subjectDocId, string gradeLevelCode, Guid? parentId, Guid? courseId,
            bool firstLevelOnly = false)
        {
            var standards = DoRead(u => new StandardDataAccess(u).Get(authorityId, documentId, subjectDocId, gradeLevelCode, parentId, courseId, firstLevelOnly));
            var docs = DoRead(u => new DocumentDataAccess(u).GetByIds(standards.Select(x => x.DocumentRef).ToList()));
            var authorities = DoRead(u => new DataAccessBase<Data.AcademicBenchmark.Model.Authority, Guid>(u)
                .GetByIds(standards.Select(x => x.AuthorityRef).ToList()));

            return StandardInfo.Create(standards, authorities, docs);
        }

        public IList<StandardInfo> GetStandardInfosByIds(IList<Guid> ids)
        {
            using (var uow = Read())
            {
                var standards = new StandardDataAccess(uow).GetByIds(ids);

                var authoritiesIds = standards.Select(x => x.AuthorityRef).ToList();
                var authorities = new DataAccessBase<Data.AcademicBenchmark.Model.Authority, Guid>(uow).GetByIds(authoritiesIds);

                var documentsIds = standards.Select(x => x.DocumentRef).ToList();
                var documents = new DataAccessBase<Data.AcademicBenchmark.Model.Document, Guid>(uow).GetByIds(documentsIds);

                return StandardInfo.Create(standards, authorities, documents);
            }
        }
    }
}
