using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
        private static string BuildComplexFieldName(string fieldName, string tableName)
        {
            return string.Format("{0}_{1}", tableName, fieldName); 
        }

        private DbQuery BuildSelectQuery(FinalGradeAnnouncementTypeQuery query)
        {
            var res = new DbQuery();
            const string vwFgAnnTypeName = "vwFinalGradeAnnouncementType";
            res.Sql.AppendFormat(@"select * from {0}", vwFgAnnTypeName);
            var type = typeof (FinalGradeAnnouncementType);

            var qConds = new AndQueryCondition();
            if (query.Id.HasValue)
                qConds.Add(BuildComplexFieldName(FinalGradeAnnouncementType.ID_FIELD, type.Name), query.Id);
            if (query.FinalGradeId.HasValue)
                qConds.Add(BuildComplexFieldName(FinalGradeAnnouncementType.FINAL_GRADE_ID_FIELD, type.Name), query.FinalGradeId);
            if (query.AnnouncementTypeId.HasValue)
                qConds.Add(BuildComplexFieldName(FinalGradeAnnouncementType.ANNOUNCEMENT_TYPE_ID_FIELD, type.Name), query.AnnouncementTypeId);

            qConds.BuildSqlWhere(res, vwFgAnnTypeName);
            return res;
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
        public int? AnnouncementTypeId { get; set; }
    }
}
