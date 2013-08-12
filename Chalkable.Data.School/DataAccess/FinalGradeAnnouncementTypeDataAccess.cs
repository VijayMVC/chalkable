using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chalkable.Data.Common;
using Chalkable.Data.Common.Orm;
using Chalkable.Data.School.Model;

namespace Chalkable.Data.School.DataAccess
{
    public class FinalGradeAnnouncementTypeDataAccess : DataAccessBase<FinalGradeAnnouncementType>
    {
        public FinalGradeAnnouncementTypeDataAccess(UnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        private Dictionary<string, object> BuildConditions(FinalGradeAnnouncementTypeQuery query)
        {
            var res = new Dictionary<string, object>();
            if(query.Id.HasValue)
                res.Add(FinalGradeAnnouncementType.ID_FIELD, query.Id);
            if (query.FinalGradeId.HasValue)
                res.Add(FinalGradeAnnouncementType.FINAL_GRADE_ID_FIELD, query.FinalGradeId);
            if (query.AnnouncementTypeId.HasValue)
                res.Add(FinalGradeAnnouncementType.ANNOUNCEMENT_TYPE_ID_FIELD, query.AnnouncementTypeId);
            return res;
        }
        private DbQuery BuildSelectQuery(Dictionary<string, object> conds)
        {
            var b = new StringBuilder();
            b.Append(@"select * from vwFinalGradeAnnouncementType ");
            var type = typeof (FinalGradeAnnouncementType);
            var fieldMapping = conds.Keys.ToDictionary(x => x, x => string.Format("{0}_{1}", type.Name, x)); //TODO: think about this 
            b = Orm.BuildSqlWhere(b, "vwFinalGradeAnnouncementType", conds, fieldMapping);
            return new DbQuery {Parameters = conds, Sql = b.ToString()};
        }
        private DbQuery BuildSelectQuery(FinalGradeAnnouncementTypeQuery query)
        {
            return BuildSelectQuery(BuildConditions(query));
        }

        public IList<FinalGradeAnnouncementType> GetList(FinalGradeAnnouncementTypeQuery query)
        {
            return ReadMany<FinalGradeAnnouncementType>(BuildSelectQuery(query), true);
        }
        public FinalGradeAnnouncementType GetOne(FinalGradeAnnouncementTypeQuery query)
        {
            return ReadOne<FinalGradeAnnouncementType>(BuildSelectQuery(query), true);
        }
        public FinalGradeAnnouncementType GetOneOrNull(FinalGradeAnnouncementTypeQuery query)
        {
            return ReadOneOrNull<FinalGradeAnnouncementType>(BuildSelectQuery(query), true);
        }
    }

    public class  FinalGradeAnnouncementTypeQuery
    {
        public Guid? Id { get; set; }
        public Guid? FinalGradeId { get; set; }
        public Guid? AnnouncementTypeId { get; set; }
    }
}
