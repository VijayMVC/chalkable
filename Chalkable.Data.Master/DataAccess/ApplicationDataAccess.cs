using System;
using System.Collections.Generic;
using System.Linq;
using Chalkable.Common;
using Chalkable.Data.Common;
using Chalkable.Data.Common.Enums;
using Chalkable.Data.Common.Orm;
using Chalkable.Data.Master.Model;

namespace Chalkable.Data.Master.DataAccess
{
    public class ApplicationDataAccess : DataAccessBase<Application, Guid>
    {
        public ApplicationDataAccess(UnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        private void LoadApplicationData(Application app)
        {
            var dvQuery = DeveloperDataAccess.BuildGetDeveloperQuery(new AndQueryCondition { { Developer.ID_FIELD, app.DeveloperRef } });
            app.Developer = ReadOne<Developer>(dvQuery, true);
            app.Pictures = SelectMany<ApplicationPicture>(new AndQueryCondition { { ApplicationPicture.APPLICATION_REF_FIELD, app.Id } });
            app.Permissions = SelectMany<ApplicationPermission>(new AndQueryCondition { { ApplicationPermission.APPLICATION_REF_FIELD, app.Id } });
            app.Categories = SelectMany<ApplicationCategory>(new AndQueryCondition { { ApplicationCategory.APPLICATION_REF_FIELD, app.Id } });
            app.GradeLevels = SelectMany<ApplicationGradeLevel>(new AndQueryCondition { { ApplicationGradeLevel.APPLICATION_REF_FIELD, app.Id } });
        }

        public Application GetApplicationById(Guid id)
        {
            var res = GetById(id);
            LoadApplicationData(res);
            return res;
        }

        public Application GetApplicationByUrl(string url)
        {
            var res = SelectOne<Application>(new AndQueryCondition
                    {
                        {Application.URL_FIELD, url},
                        {Application.ORIGINAL_REF_FIELD, null}
                    });
            
            LoadApplicationData(res);
            return res;
        }

        public Application GetApplication(ApplicationQuery query)
        {
            query.Count = 1;
            var q = BuildGetApplicationsQuery(query);
            var res = ReadOne<Application>(q);
            LoadApplicationData(res);
            return res;
        }


        private DbQuery BuildGetApplicationsQuery(ApplicationQuery query)
        {
            var res = new DbQuery();
            res.Sql.Append(@"select Application.*, (select avg(rating) from ApplicationRating where ApplicationRef = Application.Id) as Avg from Application where 1 = 1");
            if (query.Role == CoreRoles.SUPER_ADMIN_ROLE.Id)
            {
                //TODO: do nothing
                if (query.DeveloperId.HasValue)
                {
                    res.Sql.Append(string.Format(" and [{0}] = @{0}", Application.DEVELOPER_REF_FIELD));
                    res.Parameters.Add(Application.DEVELOPER_REF_FIELD, query.DeveloperId);                    
                }
            }
            else
            {
                if (query.Role == CoreRoles.DEVELOPER_ROLE.Id)
                {
                    res.Sql.Append(string.Format(" and [{0}] = @{0}", Application.DEVELOPER_REF_FIELD));
                    res.Parameters.Add(Application.DEVELOPER_REF_FIELD, query.UserId);
                }
                else
                {
                    query.Live = true;
                    if (query.DeveloperId.HasValue)
                    {
                        query.Live = false;
                        res.Sql.Append(string.Format(" and [{0}] = @{0}", Application.DEVELOPER_REF_FIELD));
                        res.Parameters.Add(Application.DEVELOPER_REF_FIELD, query.DeveloperId);
                    }
                }
                
                if (!query.IncludeInternal)
                {
                    res.Sql.Append(string.Format(" and [{0}] <> 1", Application.IS_INTERNAL_FIELD));
                }
                if (query.OnlyForInstall)
                {
                    if (query.Role == CoreRoles.TEACHER_ROLE.Id)
                        res.Sql.Append(string.Format(" and ([{0}] = 1 or [{1}] = 1 or [{2}] = 1)", Application.HAS_STUDENT_MY_APPS_FIELD, Application.HAS_TEACHER_MY_APPS_FIELD, Application.CAN_ATTACH_FIELD));
                    if (query.Role == CoreRoles.STUDENT_ROLE.Id)
                        res.Sql.Append(string.Format(" and [{0}] = 1", Application.HAS_STUDENT_MY_APPS_FIELD));
                }
    
            }
            if (query.Id.HasValue)
            {
                res.Sql.AppendFormat(" and [{0}] = @{0}", Application.ID_FIELD);
                res.Parameters.Add(Application.ID_FIELD, query.Id.Value);
            }
            if (query.Live.HasValue || query.State.HasValue)
            {
                string sqlOperator = " = ";
                int? state = (int?) query.State;
                if (query.Live.HasValue)
                {
                    if(!query.Live.Value) sqlOperator = " <> ";
                    state = (int?) ApplicationStateEnum.Live;
                }
                res.Sql.Append(string.Format(" and [{0}] {1} @{0}", Application.STATE_FIELD, sqlOperator));
                res.Parameters.Add(Application.STATE_FIELD, state);
            }
            
            if (query.Free.HasValue)
            {
                res.Sql.AppendFormat(" and [{0}] {1} 0", Application.PRICE_FIELD, query.Free.Value ? "=" : ">");
            }
            
            if (query.GradeLevels != null && query.GradeLevels.Count > 0)
            {
                res.Sql.AppendFormat(" and exists(select * from ApplicationGradeLevel where ApplicationRef = Application.Id and GradeLevel in ({0}))"
                    , query.GradeLevels.Select(x => x.ToString()).JoinString(","));
            }
            if (query.CategoryIds != null && query.CategoryIds.Count > 0)
            {
                var categoriesParam = new List<string>();
                for (int i = 0; i < query.CategoryIds.Count; i++)
                {
                    var categoryParam = "@categoryId_" + i;
                    categoriesParam.Add(categoryParam);
                    res.Parameters.Add(categoryParam, query.CategoryIds[i]);
                }
                res.Sql.AppendFormat(" and exists (select * from ApplicationCategory where ApplicationRef = Application.Id and CategoryRef in ({0}))"
                    , categoriesParam.JoinString(","));

            }
            if (!string.IsNullOrEmpty(query.Filter))
            {
                var keyWords = query.Filter.Split(new [] {' ', '.', ','}, StringSplitOptions.RemoveEmptyEntries);
                var keyWordsParams = new List<string>();
                for (int i = 0; i < keyWords.Length; i++)
                {
                    if (!string.IsNullOrEmpty(keyWords[i]))
                    {
                        var keyWordParam = "@keyWord_" + i;
                        res.Parameters.Add(keyWordParam, "%" + keyWords[i] + "%");
                        keyWordsParams.Add(" like " + keyWordParam);
                    }
                }
                if (keyWordsParams.Count > 0)
                {
                    res.Sql.Append(" and (");
                    res.Sql.AppendFormat(" LOWER(Name) {0} ", keyWordsParams.JoinString(" or LOWER(Name) "));
                    res.Sql.AppendFormat(" or LOWER(Description) {0} ", keyWordsParams.JoinString(" or LOWER(Description) "));
                    res.Sql.AppendFormat(@" or exists(select ac.Id from ApplicationCategory ac
                                                  join Category c on c.Id = ac.CategoryRef
                                                  where ApplicationRef = [Application].Id and (LOWER(c.Name) {0})
                                                 )", keyWordsParams.JoinString(" or LOWER(c.Name) "));
                    res.Sql.Append(" or exists(select * from Developer where Id = [Application].DeveloperRef and (");
                    res.Sql.AppendFormat(" LOWER(Name) {0}", keyWordsParams.JoinString(" or LOWER(Name) "));
                    res.Sql.AppendFormat(" or LOWER(WebSite) {0} ", keyWordsParams.JoinString(" or LOWER(WebSite)"));
                    res.Sql.Append(")))");
                }
            }

            return res;

        }

        private IList<Application> PreparePicturesData(IList<Application> applications)
        {
            var appIdParams = new List<string>();
            var query = new DbQuery();
            for (int i = 0; i < applications.Count; i++)
            {
                var appIdParam = "@applicationId_" + i;
                query.Parameters.Add(appIdParam, applications[i].Id);
                appIdParams.Add(appIdParam);
            }
            var sql = "select * from ApplicationPicture where ApplicationRef in ({0})";
            sql = string.Format(sql, appIdParams.JoinString(","));
            query.Sql.Append(sql);

            if (query.Parameters.Count > 0)
            {
                var pictures = ReadMany<ApplicationPicture>(query);
                foreach (var app in applications)
                {
                    app.Pictures = pictures.Where(x => x.ApplicationRef == app.Id).ToList();
                }
            }
            
            return applications;
        }
        
        public PaginatedList<Application> GetPaginatedApplications(ApplicationQuery query)
        {
            var q = BuildGetApplicationsQuery(query);
            var paginatedApps = PaginatedSelect<Application>(q, query.OrderBy, query.Start, query.Count, Orm.OrderType.Desc);
            return PreparePicturesData(paginatedApps) as PaginatedList<Application>;
        }  

        public IList<ApplicationCategory> UpdateCategories(Guid id, IList<Guid> categories)
        {
            SimpleDelete<ApplicationCategory>(new AndQueryCondition { { ApplicationCategory.APPLICATION_REF_FIELD, id } });
            IList<ApplicationCategory> appCategories = new List<ApplicationCategory>();
            if (categories != null)
            {
                foreach (var category in categories)
                {
                    appCategories.Add(new ApplicationCategory { Id = Guid.NewGuid(), ApplicationRef = id, CategoryRef = category });
                }
            }
            SimpleInsert(appCategories);
            return SelectMany<ApplicationCategory>(new AndQueryCondition { { ApplicationCategory.APPLICATION_REF_FIELD, id } });
        }

        public IList<ApplicationPicture> UpdatePictures(Guid id, IList<Guid> picturesId)
        {
            SimpleDelete<ApplicationPicture>(new AndQueryCondition { { ApplicationPicture.APPLICATION_REF_FIELD, id } });
            IList<ApplicationPicture> appPictures = new List<ApplicationPicture>();
            if (picturesId != null)
            {
                foreach (var picture in picturesId)
                {
                    appPictures.Add(new ApplicationPicture { ApplicationRef = id, Id = picture });
                }
            }
            SimpleInsert(appPictures);
            return SelectMany<ApplicationPicture>(new AndQueryCondition { { ApplicationPicture.APPLICATION_REF_FIELD, id } });
        }

        public IList<ApplicationGradeLevel> UpdateGradeLevels(Guid id, IList<int> gradeLevels)
        {
            SimpleDelete<ApplicationGradeLevel>(new AndQueryCondition { { ApplicationGradeLevel.APPLICATION_REF_FIELD, id } });
            IList<ApplicationGradeLevel> appGradeLevels = new List<ApplicationGradeLevel>();
            if (gradeLevels != null)
            {
                foreach (var gradeLevel in gradeLevels)
                {
                    appGradeLevels.Add(new ApplicationGradeLevel { ApplicationRef = id, GradeLevel = gradeLevel, Id = Guid.NewGuid() });
                }
            }
            SimpleInsert(appGradeLevels);
            return SelectMany<ApplicationGradeLevel>(new AndQueryCondition { { ApplicationGradeLevel.APPLICATION_REF_FIELD, id } });
        }

        public IList<ApplicationPermission> UpdatePermissions(Guid id, IList<AppPermissionType> permissionIds)
        {
            SimpleDelete<ApplicationPermission>(new AndQueryCondition { { ApplicationPermission.APPLICATION_REF_FIELD, id } });
            IList<ApplicationPermission> applicationPermissions = new List<ApplicationPermission>();
            if (permissionIds != null)
            {
                foreach (var permissionId in permissionIds)
                {
                    applicationPermissions.Add(new ApplicationPermission
                    {
                        Id = Guid.NewGuid(),
                        ApplicationRef = id,
                        Permission = permissionId
                    });
                }
            }
            SimpleInsert(applicationPermissions);
            return SelectMany<ApplicationPermission>(new AndQueryCondition { { ApplicationPermission.APPLICATION_REF_FIELD, id } });
        }

        public bool AppExists(Guid? currentApplicationId, string name, string url)
        {
            var query = new DbQuery();
            query.Sql.Append("Select * from Application a where ");
            query.Sql.Append("(").Append(string.Format("[{0}] = @{0} or ([{1}] = @{1} and [{1}] is not null)", Application.NAME_FIELD, Application.URL_FIELD)).Append(")");
            query.Sql.Append(" and (").Append(string.Format("[{0}] is null or [{0}] <> @{0}", Application.ORIGINAL_REF_FIELD)).Append(")");
            query.Sql.AppendFormat(" and a.[Id] <> @{0}", Application.ID_FIELD);
            query.Sql.Append(string.Format(" and not exists (select * from Application where [{0}] = a.[{1}] and [{1}] = @{1})", Application.ORIGINAL_REF_FIELD, Application.ID_FIELD));
            query.Parameters.Add(Application.ID_FIELD, currentApplicationId);
            query.Parameters.Add(Application.NAME_FIELD, name);
            query.Parameters.Add(Application.URL_FIELD, url);
            query.Parameters.Add(Application.ORIGINAL_REF_FIELD, currentApplicationId);
            return Exists(query);
        }

        public override void Delete(Guid id)
        {
            SimpleDelete<ApplicationPermission>(new AndQueryCondition{{ApplicationPermission.APPLICATION_REF_FIELD, id}});
            SimpleDelete<ApplicationCategory>(new AndQueryCondition { { ApplicationCategory.APPLICATION_REF_FIELD, id } });
            SimpleDelete<ApplicationPicture>(new AndQueryCondition { { ApplicationPicture.APPLICATION_REF_FIELD, id } });
            SimpleDelete<ApplicationRating>(new AndQueryCondition { { ApplicationRating.APPLICATION_REF_FIELD, id } });
            SimpleDelete<ApplicationGradeLevel>(new AndQueryCondition { { ApplicationGradeLevel.APPLICATION_REF_FIELD, id } });
            base.Delete(id);
        }

        public IList<Application> GetByIds(IList<Guid> ids)
        {
            if (ids == null || ids.Count == 0)
                return new List<Application>();
            var tableName = typeof(Application).Name;
            var dbQuery = new DbQuery();
            dbQuery.Sql.AppendFormat(@"select * from [{0}]", tableName);
            dbQuery.Sql.AppendFormat(" where [{0}].[{1}] in ({2})", tableName, Application.ID_FIELD
                    , ids.Select(x => "'" + x.ToString() + "'").JoinString(","));
            return ReadMany<Application>(dbQuery);
        } 
    }

    public class ApplicationQuery
    {
        public Guid? Id { get; set; }
        public Guid UserId { get; set; }
        public int Role { get; set; }
        public bool IncludeInternal { get; set; }
        public IList<Guid> CategoryIds { get; set; }
        public IList<int> GradeLevels { get; set; }
        public Guid? DeveloperId { get; set; }
        public string OrderBy { get; set; }

        public int Start { get; set; }
        public int Count { get; set; }

        public ApplicationStateEnum? State { get; set; }

        public bool? Live { get; set; }
        public bool? Free { get; set; }

        public string Filter { get; set; }

        public bool OnlyForInstall { get; set; }

        public ApplicationQuery()
        {
            Start = 0;
            Count = int.MaxValue;
            OrderBy = Application.ID_FIELD;
            IncludeInternal = false;
            Live = null;
            Free = null;
            OnlyForInstall = true;
        }

    }
}