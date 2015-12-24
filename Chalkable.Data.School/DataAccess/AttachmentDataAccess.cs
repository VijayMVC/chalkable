using System;
using System.Collections.Generic;
using System.Linq;
using Chalkable.Common;
using Chalkable.Data.Common;
using Chalkable.Data.Common.Orm;
using Chalkable.Data.School.Model;

namespace Chalkable.Data.School.DataAccess
{
    public class AttachmentDataAccess : DataAccessBase<Attachment, int>
    {
        public AttachmentDataAccess(UnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public Attachment GetById(int attachmentId, int callerId, int roleId)
        {
            var dbQuery = new DbQuery();
            var conds = new AndQueryCondition();
            conds.Add(Attachment.ID_FIELD, attachmentId);

            throw new NotImplementedException();
        }

        
        public IList<Attachment> GetLastList(int personId, int count = int.MaxValue)
        {
            var conds = new AndQueryCondition {{Attachment.PERSON_REF_FIELD, personId}};
            return GetAll(conds).OrderByDescending(x => x.Id).Take(count).ToList();
        }

        public Attachment GetLast(int personId)
        {
            return GetLastList(personId, 1).First();
        }

        public IList<Attachment> GetBySisAttachmentId(int sisAttachmentId)
        {
            var conds = new AndQueryCondition {{Attachment.SIS_ATTACHMENT_ID_FIELD, sisAttachmentId}};
            return GetAll(conds);
        } 

        public IList<Attachment> GetBySisAttachmentIds(IList<int> sisAttachmentIds)
        {
            var str = sisAttachmentIds.JoinString(",");
            var res = Orm.SimpleSelect(typeof (Attachment).Name, null);
            res.Sql.AppendFormat(" Where {0} in ({1})", Attachment.SIS_ATTACHMENT_ID_FIELD, str);
            return ReadMany<Attachment>(res);
        } 

        public PaginatedList<Attachment> GetPaginatedAttachments(int personId, string filter, string orderByColumn, Orm.OrderType orderType, int start, int count)
        {
            var conds = new AndQueryCondition {{Attachment.PERSON_REF_FIELD, personId}};
            var dbQuery = new DbQuery();
            var attachmentTName = typeof (Attachment).Name;
            dbQuery.Sql.AppendFormat(Orm.SELECT_FORMAT, "*", attachmentTName);
            conds.BuildSqlWhere(dbQuery, attachmentTName);
            if (!string.IsNullOrEmpty(filter))
            {
                dbQuery.Parameters.Add("@filter", string.Format(FILTER_FORMAT, filter));
                dbQuery.Sql.AppendFormat(" and {0} like @{1} ", Attachment.NAME_FIELD, "filter");
            }
            return PaginatedSelect<Attachment>(dbQuery, orderByColumn, start, count, orderType);
        }  
    }
}
