using Chalkable.Common.Exceptions;
using Chalkable.Data.Common;
using Chalkable.Data.Common.Orm;
using Chalkable.Data.School.Model.Announcements;

namespace Chalkable.Data.School.DataAccess.AnnouncementsDataAccess
{

    public class AnnouncementDataAccess : DataAccessBase<Announcement>
    {
        public AnnouncementDataAccess(UnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public AnnouncementTypeEnum GetAnnouncementType(int announcementId)
        {
            var dbQuery = new DbQuery();
            dbQuery.Sql.AppendFormat(@"select  LessonPlan.Id as LessonPlan_Id,
		                                       AdminAnnouncement.Id as AdminAnnouncement_Id,
		                                       ClassAnnouncement.Id as ClassAnnouncement_Id,
											   SupplementalAnnouncement.Id as SupplementalAnnouncement_Id
                                        from Announcement
                                        left join LessonPlan on LessonPlan.Id = Announcement.Id
                                        left join AdminAnnouncement on AdminAnnouncement.Id = Announcement.Id
                                        left join ClassAnnouncement on ClassAnnouncement.Id = Announcement.Id
                                        left join SupplementalAnnouncement on SupplementalAnnouncement.Id = Announcement.Id
                                    ");
            new AndQueryCondition { { Announcement.ID_FIELD, announcementId } }.BuildSqlWhere(dbQuery, typeof(Announcement).Name);
            return Read(dbQuery, reader =>
            {
                reader.Read();
                if (!reader.IsDBNull(reader.GetOrdinal("LessonPlan_Id")))
                    return AnnouncementTypeEnum.LessonPlan;
                if (!reader.IsDBNull(reader.GetOrdinal("AdminAnnouncement_Id")))
                    return AnnouncementTypeEnum.Admin;
                if (!reader.IsDBNull(reader.GetOrdinal("ClassAnnouncement_Id")))
                    return AnnouncementTypeEnum.Class;
                if (!reader.IsDBNull(reader.GetOrdinal("SupplementalAnnouncement_Id")))
                    return AnnouncementTypeEnum.Supplemental;
                throw new NoAnnouncementException();
            });
        }
    }
}
