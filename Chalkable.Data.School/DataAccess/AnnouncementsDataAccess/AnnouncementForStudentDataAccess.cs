﻿using System.Collections.Generic;
using System.Linq;
using Chalkable.Common;
using Chalkable.Data.Common;

namespace Chalkable.Data.School.DataAccess.AnnouncementsDataAccess
{
    public class AnnouncementForStudentDataAccess : AnnouncementDataAccess
    {
        public AnnouncementForStudentDataAccess(UnitOfWork unitOfWork, int? schoolId) : base(unitOfWork, schoolId)
        {
        }
        
        private const string GET_STUDENT_ANNOUNCEMENTS = "spGetStudentAnnouncements";
        private const string GRADED_ONLY_PARAM = "gradedOnly";
        private const string SIS_ACTIVITY_IDS_PARAM = "sisActivitiesIds";

        public override AnnouncementQueryResult GetAnnouncements(AnnouncementsQuery query)
        {
            var parameters = new Dictionary<string, object>
                {
                    { GRADED_ONLY_PARAM, query.GradedOnly },
                    { SIS_ACTIVITY_IDS_PARAM, query.SisActivitiesIds != null ? query.SisActivitiesIds.Select(x => x.ToString()).JoinString(",") : null}
                };
            return GetAnnouncementsComplex(GET_STUDENT_ANNOUNCEMENTS, parameters, query);
        }
        protected override void BuildConditionForGetSimpleAnnouncement(Common.Orm.DbQuery dbQuery, int role, int callerId)
        {
            dbQuery.Sql.Append(@" and (
                                        (Announcement.ClassRef in (select cp.ClassRef from ClassPerson cp where cp.PersonRef = @callerId)
                                        ) or (ClassAnnouncementTypeRef is null
                                                    and Announcement.Id in (select ar.AnnouncementRef from AnnouncementRecipient ar 
                                                                            where ar.ToAll = 1 or ar.PersonRef = @callerId or ar.RoleRef = @roleId
                                                                                or ar.GradeLevelRef in (select GradeLevelRef from StudentSchoolYear where StudentRef = @callerId)
                                                                        )
                                              )
                                       )");

            dbQuery.Parameters.Add("callerId", callerId);
            dbQuery.Parameters.Add("@roleId", role);
        }
    }
}
