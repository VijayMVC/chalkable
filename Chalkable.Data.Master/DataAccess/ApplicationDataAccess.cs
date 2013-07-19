﻿using System;
using System.Collections.Generic;
using System.Text;
using Chalkable.Common;
using Chalkable.Data.Common;
using Chalkable.Data.Common.Enums;
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
            app.Pictures = SelectMany<ApplicationPicture>(new Dictionary<string, object> { { ApplicationPicture.APPLICATION_REF_FIELD, app.Id } });
            app.Permissions = SelectMany<ApplicationPermission>(new Dictionary<string, object> { { ApplicationPermission.APPLICATION_REF_FIELD, app.Id } });
            app.Categories = SelectMany<ApplicationCategory>(new Dictionary<string, object> { {ApplicationCategory.APPLICATION_REF_FIELD, app.Id} });
            app.GradeLevels = SelectMany<ApplicationGradeLevel>(new Dictionary<string, object> { { ApplicationGradeLevel.APPLICATION_REF_FIELD, app.Id } });
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

        public IList<ApplicationCategory> UpdateCategories(Guid id, IList<Guid> categories)
        {
            SimpleDelete<ApplicationCategory>(new Dictionary<string, object>{{ApplicationCategory.APPLICATION_REF_FIELD, id}});
            foreach (var category in categories)
            {
                SimpleInsert(new ApplicationCategory {ApplicationRef = id, CategoryRef = category});
            }
            return SelectMany<ApplicationCategory>(new Dictionary<string, object> { { ApplicationCategory.APPLICATION_REF_FIELD, id } });
        }

        public IList<ApplicationPicture> UpdatePictures(Guid id, IList<Guid> picturesId)
        {
            SimpleDelete<ApplicationPicture>(new Dictionary<string, object> { { ApplicationCategory.APPLICATION_REF_FIELD, id } });
            foreach (var picture in picturesId)
            {
                SimpleInsert(new ApplicationPicture{ApplicationRef = id, Id = picture});
            }
            return SelectMany<ApplicationPicture>(new Dictionary<string, object> { { ApplicationPicture.APPLICATION_REF_FIELD, id } });
        }

        public IList<ApplicationGradeLevel> UpdateGradeLevels(Guid id, IList<int> gradeLevels)
        {
            SimpleDelete<ApplicationGradeLevel>(new Dictionary<string, object> { { ApplicationGradeLevel.APPLICATION_REF_FIELD, id } });
            foreach (var gradeLevel in gradeLevels)
            {
                SimpleInsert(new ApplicationGradeLevel { ApplicationRef = id, GradeLevel = gradeLevel, Id = Guid.NewGuid()});
            }
            return SelectMany<ApplicationGradeLevel>(new Dictionary<string, object> { { ApplicationGradeLevel.APPLICATION_REF_FIELD, id } });
        }

        public IList<ApplicationPermission> UpdatePermissions(Guid id, IList<AppPermissionType> permissionIds)
        {
            return SelectMany<ApplicationPermission>(new Dictionary<string, object> { { ApplicationPermission.APPLICATION_REF_FIELD, id } });
        }

        public bool AppExists(Guid? currentApplicationId, string name, string url)
        {
            var sql = new StringBuilder();
            sql.Append("Select count(*) as [Count] from Application a where ");
            sql.Append("(").Append(string.Format("[{0}] = @{0} or ([{1}] = @{1} and [{1}] is not null)", Application.NAME_FIELD, Application.URL_FIELD)).Append(")");
            sql.Append(" and (").Append(string.Format("[{0}] is null or [{0}] <> @{0}", Application.ORIGINAL_REF_FIELD)).Append(")");
            sql.Append(string.Format(" and not exists (select * from Application where [{0}] = a.[{1}] and [{1}] = @{1})", Application.ORIGINAL_REF_FIELD, Application.ID_FIELD));
            var ps = new Dictionary<string, object> {
                                                        {Application.ID_FIELD, currentApplicationId},
                                                        {Application.NAME_FIELD, name},
                                                        {Application.URL_FIELD, url},
                                                        {Application.ORIGINAL_REF_FIELD, currentApplicationId},
                                                    };
            return Exists(new DbQuery {Parameters = ps, Sql = sql.ToString()});
        }

        public override void Delete(Guid id)
        {
            SimpleDelete<ApplicationPermission>(new Dictionary<string, object>{{ApplicationPermission.APPLICATION_REF_FIELD, id}});
            SimpleDelete<ApplicationCategory>(new Dictionary<string, object> { { ApplicationCategory.APPLICATION_REF_FIELD, id } });
            SimpleDelete<ApplicationPicture>(new Dictionary<string, object> { { ApplicationPicture.APPLICATION_REF_FIELD, id } });
            SimpleDelete<ApplicationRating>(new Dictionary<string, object> { { ApplicationRating.APPLICATION_REF_FIELD, id } });
            SimpleDelete<ApplicationGradeLevel>(new Dictionary<string, object> { { ApplicationGradeLevel.APPLICATION_REF_FIELD, id } });
            base.Delete(id);
        }
    }
}