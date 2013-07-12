using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chalkable.Data.Common;
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
                res.Add("Id", query.Id);
            if (query.FinalGradeId.HasValue)
                res.Add("FinalGradeRef", query.FinalGradeId);
            if (query.AnnouncementTypeId.HasValue)
                res.Add("AnnouncementTypeRef", query.AnnouncementTypeId);
            return res;
        }
        private DbQuery BuildSelectQuery(Dictionary<string, object> conds)
        {
            var sql = @"select {0} from FinalGradeAnnouncementType 
                        join AnnouncementType on AnnouncementType.Id = FinalGradeAnnouncementType.AnnouncementTypeRef";
            var b = new StringBuilder();
            var types = new List<Type> {typeof (FinalGradeAnnouncementType), typeof (AnnouncementType)};
            b.AppendFormat(sql, Orm.ComplexResultSetQuery(types));
            b = Orm.BuildSqlWhere(b, types[0], conds);
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
