using System;
using System.Collections.Generic;
using System.Linq;
using Chalkable.Common;
using Chalkable.Data.AcademicBenchmark.Model;
using Chalkable.Data.Common;
using Chalkable.Data.Common.Orm;

namespace Chalkable.Data.AcademicBenchmark.DataAccess
{
    public class StandardDataAccess : DataAccessBase<Standard, Guid>
    {
        public StandardDataAccess(UnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public override void Delete(IList<Standard> entities)
        {
            var @params = new Dictionary<string, object>
            {
                ["standardIds"] = entities.Select(x => x.Id)
            };
            ExecuteStoredProcedure("spDeleteStandards", @params);
        }

        public override void Delete(Guid key)
        {
            this.Delete(new List<Standard> {new Standard() {Id = key} });
        }

        public StandardRelations GetStandardRelations(Guid id)
        {
            var @params = new Dictionary<string, object>
            {
                ["standardId"] = id
            };

            var relations = new StandardRelations();

            using (var reader = ExecuteStoredProcedureReader("spGetStandardRelations", @params))
            {
                relations.Origins = reader.ReadList<Standard>();
                relations.Derivatives = reader.ReadList<Standard>();
                relations.RelatedDerivatives = reader.ReadList<Standard>();
            }

            return relations;
        }

        public PaginatedList<Standard> Search(string searchQuery, bool? deepest, int start, int count)
        {
            var @params = new Dictionary<string, object>
            {
                ["searchQuery"] = searchQuery,
                ["deepest"] = deepest,
                ["start"] = start,
                ["count"] = count
            };

            return ExecuteStoredProcedurePaginated<Standard>("spSearchStandards", @params, start, count);     
        }

        public IList<Standard> Get(Guid? authorityId, Guid? documentId, Guid? subjectDocId, string gradeLevelCode,
            Guid? parentId, Guid? courseId, bool? deepest)
        {
            AndQueryCondition conditions = new AndQueryCondition();
            if(authorityId.HasValue)
                conditions.Add(nameof(Standard.AuthorityRef), authorityId, ConditionRelation.Equal);

            if(documentId.HasValue)
                conditions.Add(nameof(Standard.DocumentRef), documentId, ConditionRelation.Equal);

            if(subjectDocId.HasValue)
                conditions.Add(nameof(Standard.SubjectDocRef), subjectDocId, ConditionRelation.Equal);
            
            if(parentId.HasValue)
                conditions.Add(nameof(Standard.ParentRef), parentId, ConditionRelation.Equal);

            if(courseId.HasValue)
                conditions.Add(nameof(Standard.CourseRef), courseId, ConditionRelation.Equal);

            if(deepest.HasValue)
                conditions.Add(nameof(Standard.IsDeepest), deepest, ConditionRelation.Equal);

            //add grade level code condition!!!
            var query = Orm.SimpleSelect(nameof(Standard), conditions);
            
            return ReadMany<Standard>(query);
        }
    }
}
