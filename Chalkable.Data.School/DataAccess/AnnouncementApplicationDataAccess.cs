using System;
using System.Collections.Generic;
using System.Linq;
using Chalkable.Common;
using Chalkable.Data.Common;
using Chalkable.Data.Common.Orm;
using Chalkable.Data.School.Model;

namespace Chalkable.Data.School.DataAccess
{
    public class AnnouncementApplicationDataAccess : DataAccessBase<AnnouncementApplication, int>
    {
        public AnnouncementApplicationDataAccess(UnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public override void Delete(int key)
        {
            var queries = new List<DbQuery>
                {
                    Orm.SimpleDelete<AutoGrade>(new AndQueryCondition 
                        {
                            {AutoGrade.ANNOUNCEMENT_APPLICATION_REF_FIELD, key}
                        }),
                    Orm.SimpleDelete<StudentAnnouncementApplicationMeta>(new AndQueryCondition
                        {
                            {StudentAnnouncementApplicationMeta.ANNOUNCEMENT_APPLICATION_REF_FIELD, key }
                        }),
                    Orm.SimpleDelete<AnnouncementApplication>(new AndQueryCondition
                        {
                            {nameof(AnnouncementApplication.Id), key}
                        })

                };
            var dbQuery = new DbQuery(queries);
            ExecuteNonQueryParametrized(dbQuery.Sql.ToString(), dbQuery.Parameters);
        }

        public void DeleteByAnnouncementId(int announcementId)
        {
            var query1 = new DbQuery();
            query1.Sql.AppendFormat(Orm.DELETE_FORMAT, typeof (AutoGrade).Name).Append(" ")
                  .Append(" where ").AppendFormat(" [{0}] in (", AutoGrade.ANNOUNCEMENT_APPLICATION_REF_FIELD)
                  .AppendFormat(Orm.SELECT_FORMAT, nameof(AnnouncementApplication.Id), typeof(AnnouncementApplication).Name)
                  .Append(" where ").AppendFormat(" [{0}] = @{0}", nameof(AnnouncementApplication.AnnouncementRef))
                  .Append(")");
            query1.Parameters.Add(nameof(AnnouncementApplication.AnnouncementRef), announcementId);
            var queries = new List<DbQuery>
                {   
                    query1,
                    Orm.SimpleDelete<AnnouncementApplication>(new AndQueryCondition
                        {
                            {nameof(AnnouncementApplication.AnnouncementRef), announcementId}
                        })

                };
            var dbQuery = new DbQuery(queries);
            ExecuteNonQueryParametrized(dbQuery.Sql.ToString(), dbQuery.Parameters);
        }

        public IList<AnnouncementApplication> GetAnnouncementApplicationsbyAnnIds(IList<int> announcementIds)
        {
            var tableName = "AnnouncementApplication";
            var dbQuery = new DbQuery();
            dbQuery.Sql.AppendFormat(@"select * from [{0}]", tableName);
            if (announcementIds != null && announcementIds.Count > 0)
                dbQuery.Sql.AppendFormat(" where [{0}].[{1}] in ({2})", tableName,nameof(AnnouncementApplication.AnnouncementRef),
                                         announcementIds.Select(x => x.ToString()).JoinString(","));
            return ReadMany<AnnouncementApplication>(dbQuery);
        } 

        public IList<AnnouncementApplication> GetAnnouncementApplicationsByPerson(int personId, bool onlyActive)
        {
            var sql = string.Format(@"select AnnouncementApplication.* from 
                                        AnnouncementApplication
                                        join Announcement on AnnouncementApplication.AnnouncementRef = Announcement.Id
                                        where 
	                                        Announcement.PersonRef = @{0}
	                                        or exists(select * from ClassPerson where PersonRef = @{0} and ClassRef = Announcement.ClassRef)
	                                        
                                        ", "personId");
            if (onlyActive)
                sql += " and AnnouncementApplication.Active = 1";
            var ps = new Dictionary<string, object> {{"personId", personId}};
            return ReadMany<AnnouncementApplication>(new DbQuery (sql, ps));
        }

        public IList<AnnouncementApplicationRecipient> GetAnnouncementApplicationRecipients(int? studentId, int? teacherId, int? adminId, Guid appId, int schoolYear)
        {
            IDictionary<string, object> ps = new Dictionary<string, object>
            {
                {"@teacherId", teacherId},
                {"@studentId", studentId},
                {"@adminId", adminId},
                {"@schoolYearId", schoolYear},
                {"@appId", appId}
            };
            IList<AnnouncementApplicationRecipient> res;
            using (var reader = ExecuteStoredProcedureReader("spGetAnnouncementApplicationRecipients", ps))
            {
                res = reader.ReadList<AnnouncementApplicationRecipient>();
            }
            return res;
        }
    }
}