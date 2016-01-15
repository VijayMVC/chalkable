using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chalkable.Data.Common;
using Chalkable.Data.Common.Orm;
using Chalkable.Data.School.Model;
using Chalkable.Data.School.Model.Announcements;

namespace Chalkable.Data.School.DataAccess.AnnouncementsDataAccess
{
    public class ClassAnnouncementForAdminDataAccess : ClassAnnouncementDataAccess
    {
        public ClassAnnouncementForAdminDataAccess(UnitOfWork unitOfWork, int schoolYearId) : base(unitOfWork, schoolYearId)
        {
        }

        public override ClassAnnouncement GetLastDraft(int personId)
        {
            var conds = new AndQueryCondition
                {
                    {Announcement.STATE_FIELD, AnnouncementState.Draft},
                    {ClassAnnouncement.SCHOOL_SCHOOLYEAR_REF_FIELD, schoolYearId}
                };
            var dbQuery = SeletClassAnnouncements(ClassAnnouncement.VW_CLASS_ANNOUNCEMENT_NAME, personId);
            conds.BuildSqlWhere(dbQuery, ClassAnnouncement.VW_CLASS_ANNOUNCEMENT_NAME);
            FilterClassAnnouncementByCaller(dbQuery, personId);
            Orm.OrderBy(dbQuery, ClassAnnouncement.VW_CLASS_ANNOUNCEMENT_NAME, Announcement.CREATED_FIELD, Orm.OrderType.Desc);
            return ReadOneOrNull<ClassAnnouncement>(dbQuery); ;
        }

        protected override DbQuery SeletClassAnnouncements(string tableName, int callerId)
        {
            var dbQuery = new DbQuery();
            var selectSet = $"{tableName}.*, cast(0 as bit) as IsOwner";
            dbQuery.Sql.AppendFormat(Orm.SELECT_FORMAT, selectSet, tableName);
            return dbQuery;
        }

        protected override DbQuery FilterClassAnnouncementByCaller(DbQuery dbQuery, int callerId)
        {
            return dbQuery;
        }
    }
}
