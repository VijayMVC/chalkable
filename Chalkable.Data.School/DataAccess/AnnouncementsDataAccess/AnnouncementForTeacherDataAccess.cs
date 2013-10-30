using System;
using System.Collections.Generic;
using Chalkable.Data.Common;
using Chalkable.Data.School.Model;

namespace Chalkable.Data.School.DataAccess.AnnouncementsDataAccess
{
    public class AnnouncementForTeacherDataAccess : AnnouncementDataAccess
    {
        public AnnouncementForTeacherDataAccess(UnitOfWork unitOfWork, int? schoolId) : base(unitOfWork, schoolId)
        {
        }
        private const string GET_TEACHER_ANNOUNCEMENTS = "spGetTeacherAnnouncements";
        private const string GRADED_ONLY_PARAM = "gradedOnly";
        private const string ALL_SCHOOL_ITEMS_PARAM = "allSchoolItems";

        public override AnnouncementQueryResult GetAnnouncements(AnnouncementsQuery query)
        {
            var parameters = new Dictionary<string, object>
                {
                    {GRADED_ONLY_PARAM, query.GradedOnly},
                    {ALL_SCHOOL_ITEMS_PARAM, query.AllSchoolItems}
                };
            return GetAnnouncementsComplex(GET_TEACHER_ANNOUNCEMENTS, parameters, query);
        }
        protected override void BuildConditionForGetSimpleAnnouncement(Common.Orm.DbQuery dbQuery, int role, int callerId)
        {
            dbQuery.Sql.Append(" and ");
            dbQuery.Sql.Append(@" (Announcement.PersonRef = @callerId 
                                        or (AnnouncementTypeRef = @adminType and Announcement.Id in (select ar.AnnouncementRef from AnnouncementRecipient ar 
                                                                                                      where ar.ToAll = 1 or ar.PersonRef = @callerId or ar.RoleRef = @roleId))
                                  )");
            dbQuery.Parameters.Add("callerId", callerId);
            dbQuery.Parameters.Add("adminType", (int)SystemAnnouncementType.Admin);
            dbQuery.Parameters.Add("@roleId", role);
        }
    }
}
