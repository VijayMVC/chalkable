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
            var @params = new Dictionary<string, object>
            {
                ["standardIds"] = new List<Guid> { key }
            };
            ExecuteStoredProcedure("spDeleteStandards", @params);
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
                if (!reader.Read())
                    return null;

                relations.Standard = reader.Read<Standard>();
                reader.NextResult();
                relations.Origins = reader.ReadList<Standard>();
                reader.NextResult();
                relations.Derivatives = reader.ReadList<Standard>();
                reader.NextResult();
                relations.RelatedDerivatives = reader.ReadList<Standard>();
            }

            return relations;
        }

        //public IList<StandardRelations> GetStandardsRelations(IList<Guid> ids)
        //{
        //    var @params = new Dictionary<string, object>
        //    {
        //        ["standardId"] = ids
        //    };

        //    var relations = new StandardRelations();

        //    using (var reader = ExecuteStoredProcedureReader("spGetStandardRelations", @params))
        //    {
        //        if (!reader.Read())
        //            return null;

        //        var standards = reader.ReadList<Standard>();
        //        reader.NextResult();
        //        relations.Origins = reader.ReadList<Standard>();
        //        reader.NextResult();
        //        relations.Derivatives = reader.ReadList<Standard>();
        //        reader.NextResult();
        //        relations.RelatedDerivatives = reader.ReadList<Standard>();
        //    }

        //    return relations;
        //}

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
            Guid? parentId, Guid? courseId, bool firstLevelOnly = false)
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

            if(firstLevelOnly)
                conditions.Add(nameof(Standard.Level), 1, ConditionRelation.Equal);
            
            var query = Orm.SimpleSelect(nameof(Standard), conditions);

            if (!string.IsNullOrWhiteSpace(gradeLevelCode))
            {
                query.Sql.Append(@" And (Cast(
	                 Case When GradeLevelLoRef = 'K' Then 0  
		             When GradeLevelLoRef = 'PK' Then -1 
		             Else GradeLevelLoRef End as int) <= @gradeId AND

                     Cast( Case When GradeLevelHiRef = 'K' Then 0  
		             When GradeLevelHiRef = 'PK' Then -1 
		             Else GradeLevelHiRef End as int) >= @gradeId)");

                query.Parameters.Add("gradeId", GradeCodeToInt(gradeLevelCode));
            }

            return ReadMany<Standard>(query);
        }

        protected int GradeCodeToInt(string gradeCode)
        {
            if (gradeCode == "PK")
                return -1;
            if (gradeCode == "K")
                return 0;

            return int.Parse(gradeCode);
        }
    }
}
