using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chalkable.Data.Common;
using Chalkable.Data.School.Model;

namespace Chalkable.Data.School.DataAccess.AnnouncementsDataAccess
{
    public class AnnouncementForStudentDataAccess : AnnouncementDataAccess
    {
        public AnnouncementForStudentDataAccess(UnitOfWork unitOfWork) : base(unitOfWork)
        {
        }
        
        private const string GET_STUDENT_ANNOUNCEMENTS = "spGetStudentAnnouncements";
        private const string GRADED_ONLY_PARAM = "gradedOnly";

        public override AnnouncementQueryResult GetAnnouncements(AnnouncementsQuery query)
        {
            var parameters = new Dictionary<string, object> { { GRADED_ONLY_PARAM, query.GradedOnly } };
            return GetAnnouncementsComplex(GET_STUDENT_ANNOUNCEMENTS, parameters, query);
        }
        protected override void BuildConditionForGetSimpleAnnouncement(Common.Orm.DbQuery dbQuery, int role, int callerId)
        {
            dbQuery.Sql.Append(@" and (
                                        (Announcement.MarkingPeriodClassRef in (select mpc.Id from MarkingPeriodClass mpc
                                                                                join ClassPerson cp on cp.ClassRef = mpc.ClassRef
                                                                                where cp.PersonRef = @callerId)
                                        ) or (AnnouncementTypeRef = @adminType 
                                                    and Announcement.Id in (select ar.AnnouncementRef from AnnouncementRecipient ar 
                                                                            where ar.ToAll = 1 or ar.PersonRef = @callerId or ar.RoleRef = @roleId
                                                                                or ar.GradeLevelRef in (select GradeLevelRef from StudentInfo where Id = @callerId)
                                                                        )
                                              )
                                       )");

            dbQuery.Parameters.Add("callerId", callerId);
            dbQuery.Parameters.Add("adminType", (int)SystemAnnouncementType.Admin);
            dbQuery.Parameters.Add("@roleId", role);
        }
    }
}
