using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using Chalkable.Common;
using Chalkable.Data.Common;
using Chalkable.Data.Common.Orm;
using Chalkable.Data.School.Model;

namespace Chalkable.Data.School.DataAccess
{
    public class AnnouncementAssignedAttributeDataAccess : DataAccessBase<AnnouncementAssignedAttribute, int>
    {
        public AnnouncementAssignedAttributeDataAccess(UnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public override AnnouncementAssignedAttribute GetById(int key)
        {
            var dbQuery = new DbQuery();
            var idField = string.Format("{0}_{1}", typeof (AnnouncementAssignedAttribute).Name, AnnouncementAssignedAttribute.ID_FIELD);
            dbQuery.Sql.AppendFormat(Orm.SELECT_FORMAT, "*", AnnouncementAssignedAttribute.VW_ANNOUNCEMENT_ASSIGNED_ATTRIBUTE);
            var conds = new AndQueryCondition {{idField, key}};
            conds.BuildSqlWhere(dbQuery, AnnouncementAssignedAttribute.VW_ANNOUNCEMENT_ASSIGNED_ATTRIBUTE);
            return Read(dbQuery, ReadAttributes).First();
        }

        public IList<AnnouncementAssignedAttribute> GetAttributesByIds(IList<int> ids)
        {
            var annIdsStr = ids.Select(x => x.ToString()).JoinString(",");
            var dbQuery = new DbQuery();
            dbQuery.Sql.AppendFormat(Orm.SELECT_FORMAT, "*", typeof(AnnouncementAssignedAttribute).Name)
                       .AppendFormat(" Where {0} in ({1}) ", AnnouncementAssignedAttribute.ID_FIELD, annIdsStr);

            return ReadMany<AnnouncementAssignedAttribute>(dbQuery);
        } 
        

        public IList<AnnouncementAssignedAttribute> GetLastListByAnnIds(IList<int> toAnnouncementIds, int count)
        {
            var annIdsStr = toAnnouncementIds.Select(x => x.ToString()).JoinString(",");
            var dbQuery = new DbQuery();
            var annRefField = $"{typeof (AnnouncementAssignedAttribute).Name}_{AnnouncementAssignedAttribute.ANNOUNCEMENT_REF_FIELD}";
            dbQuery.Sql.AppendFormat(Orm.SELECT_FORMAT, "*", AnnouncementAssignedAttribute.VW_ANNOUNCEMENT_ASSIGNED_ATTRIBUTE)
                       .AppendFormat(" Where {0} in ({1}) ", annRefField, annIdsStr);

            return  Read(dbQuery, ReadAttributes).OrderByDescending(x => x.Id).Take(count).OrderBy(x => x.Id).ToList();
        }

        public static IList<AnnouncementAssignedAttribute> ReadAttributes(DbDataReader reader)
        {
            var res = new List<AnnouncementAssignedAttribute>();
            while (reader.Read())
            {
                var attribute = reader.Read<AnnouncementAssignedAttribute>(true);
                if (attribute.AttachmentRef.HasValue)
                    attribute.Attachment = reader.Read<Attachment>(true);
                res.Add(attribute);
            }
            return res;
        }

        public IList<AnnouncementAssignedAttribute> GetLastListByAnnId(int announcementId, int count)
        {
            return GetLastListByAnnIds(new List<int> {announcementId}, count);
        }

        public IList<AnnouncementAssignedAttribute> GetListByAnntId(int announcementId)
        {
            return GetLastListByAnnId(announcementId, int.MaxValue).OrderBy(x => x.Id).ToList();
        }
        
    }
}
