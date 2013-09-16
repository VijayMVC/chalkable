using System;
using System.Collections.Generic;
using Chalkable.Data.Common;
using Chalkable.Data.Common.Orm;
using Chalkable.Data.School.Model;

namespace Chalkable.Data.School.DataAccess
{
    public class AnnouncementRecipientDataAccess : DataAccessBase<AnnouncementRecipient>
    {
        public AnnouncementRecipientDataAccess(UnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public IList<AnnouncementRecipient> GetList(Guid announcementId)
        {
            var conds = new AndQueryCondition { { AnnouncementRecipient.ANNOUNCEMENT_REF_FIELD, announcementId } };
            var dbQuery = new DbQuery();
            var types = new List<Type> {typeof (AnnouncementRecipient), typeof (Person)};
            var resSetQuery = Orm.ComplexResultSetQuery(types);
            dbQuery.Sql.AppendFormat(@"select {0} from AnnouncementRecipient 
                                     join Person on Person.Id = AnnouncementRecipient.PersonRef", resSetQuery); 
            conds.BuildSqlWhere(dbQuery, types[0].Name);
            return ReadMany<AnnouncementRecipient>(dbQuery, true);
        }

        public void DeleteByAnnouncementId(Guid announcementId)
        {
            var conds = new AndQueryCondition { { AnnouncementRecipient.ANNOUNCEMENT_REF_FIELD, announcementId } };
            SimpleDelete<AnnouncementRecipient>(conds);
        }
 
    }
}
