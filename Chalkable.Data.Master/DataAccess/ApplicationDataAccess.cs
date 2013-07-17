using System;
using System.Collections.Generic;
using System.Text;
using Chalkable.Common;
using Chalkable.Data.Common;
using Chalkable.Data.Master.Model;

namespace Chalkable.Data.Master.DataAccess
{
    public class ApplicationDataAccess : DataAccessBase<Application>
    {
        public ApplicationDataAccess(UnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        private void LoadApplicationData(Application app)
        {
            app.Developer = SelectOne<Developer>(new Dictionary<string, object> { { "Id", app.DeveloperRef } });
            app.Pictures = SelectMany<ApplicationPicture>(new Dictionary<string, object> { { ApplicationPicture.APPLICATION_REF_FIELD, app.DeveloperRef } });
            app.Permissions = SelectMany<ApplicationPermission>(new Dictionary<string, object> { { ApplicationPermission.APPLICATION_REF_FIELD, app.DeveloperRef } });
        }

        public Application GetApplicationById(Guid id)
        {
            var res = GetById(id);
            LoadApplicationData(res);
            return res;
        }

        public Application GetApplicationByUrl(string url)
        {
            var res = SelectOne<Application>(new Dictionary<string, object>
                    {
                        {Application.URL_FIELD, url},
                        {Application.ORIGINAL_REF_FIELD, null}
                    });
            
            LoadApplicationData(res);
            return res;
        }

        public IList<Application> GetApplications(Guid userId, int role, string orderBy = null, bool includeInternal = false, Guid? categoryId = null)
        {
            var sql = new StringBuilder();
            sql.Append("select Application.*, (select avg(rating) from ApplicationRating where ApplicationRef = Application.Id) as Avg from Application where 1 = 1");
            var ps = new Dictionary<string, object>();

            if (role == CoreRoles.SUPER_ADMIN_ROLE.Id)
            {
                //TODO: do nothing
            }
            else 
            {
                if (role == CoreRoles.DEVELOPER_ROLE.Id)
                {
                    sql.Append(string.Format(" and [{0}] = @{0}", Application.DEVELOPER_REF_FIELD));
                    ps.Add(Application.DEVELOPER_REF_FIELD, userId);
                    sql.Append(string.Format(" and [{0}] <> @{0}", Application.STATE_FIELD));
                }
                else
                    sql.Append(string.Format(" and [{0}] == @{0}", Application.STATE_FIELD));
                ps.Add(Application.STATE_FIELD, (int)ApplicationStateEnum.Live);

                if (!includeInternal)
                {
                    sql.Append(string.Format(" and [{0}] <> 1", Application.IS_INTERNAL_FIELD));
                }

                if (role == CoreRoles.TEACHER_ROLE.Id)
                    sql.Append(string.Format(" and ([{0}] = 1 or [{1}] = 1 or [{2}] = 1)", Application.HAS_STUDENT_MY_APPS_FIELD, Application.HAS_TEACHER_MY_APPS_FIELD, Application.CAN_ATTACH_FIELD));
                if (role == CoreRoles.STUDENT_ROLE.Id)
                    sql.Append(string.Format(" and [{0}] = 1", Application.HAS_STUDENT_MY_APPS_FIELD));
                    
            }
            if (categoryId.HasValue)
            {
                sql.Append(string.Format(" and exists (select * from ApplicationCategory where ApplicationRef = Application.Id and CategoryRef = @{0})", "categoryId"));
                ps.Add("categoryId", categoryId);
            }
            orderBy = orderBy ?? Application.ID_FIELD;
            var q = Orm.PaginationSelect(new DbQuery { Sql = sql.ToString(), Parameters = ps }, orderBy,
                                                  Orm.OrderType.Desc, 0, int.MaxValue);


            using (var reader = ExecuteReaderParametrized(q.Sql, q.Parameters))
            {
                return reader.ReadList<Application>();
            }
        }
    }
}