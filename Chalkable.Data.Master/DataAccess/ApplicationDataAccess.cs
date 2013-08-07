using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
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
            var dvQuery = DeveloperDataAccess.BuildGetDeveloperQuery(new Dictionary<string, object> { {Developer.ID_FIELD, app.DeveloperRef}});
            app.Developer = ReadOne<Developer>(dvQuery, true);
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
            var sql = new StringBuilder();
            sql.Append(@"select Application.*, (select avg(rating) from ApplicationRating where ApplicationRef = Application.Id) as Avg from Application where 1 = 1");
            var ps = new Dictionary<string, object>();

            if (query.Role == CoreRoles.SUPER_ADMIN_ROLE.Id)
            {
                //TODO: do nothing
            }
            else
            {
                if (query.Role == CoreRoles.DEVELOPER_ROLE.Id)
                {
                    sql.Append(string.Format(" and [{0}] = @{0}", Application.DEVELOPER_REF_FIELD));
                    ps.Add(Application.DEVELOPER_REF_FIELD, query.UserId);
                }
                else
                {
                    query.Live = true;
                    if (query.DeveloperId.HasValue)
                    {
                        query.Live = false;
                        sql.Append(string.Format(" and [{0}] = @{0}", Application.DEVELOPER_REF_FIELD));
                        ps.Add(Application.DEVELOPER_REF_FIELD, query.DeveloperId);
                    }
                }
                
                if (!query.IncludeInternal)
                {
                    sql.Append(string.Format(" and [{0}] <> 1", Application.IS_INTERNAL_FIELD));
                }
                if (query.Role == CoreRoles.TEACHER_ROLE.Id)
                    sql.Append(string.Format(" and ([{0}] = 1 or [{1}] = 1 or [{2}] = 1)", Application.HAS_STUDENT_MY_APPS_FIELD, Application.HAS_TEACHER_MY_APPS_FIELD, Application.CAN_ATTACH_FIELD));
                if (query.Role == CoreRoles.STUDENT_ROLE.Id)
                    sql.Append(string.Format(" and [{0}] = 1", Application.HAS_STUDENT_MY_APPS_FIELD));
    
            }
            if (query.Id.HasValue)
            {
                sql.AppendFormat(" and [{0}] = @{0}", Application.ID_FIELD);
                ps.Add(Application.ID_FIELD, query.Id.Value);
            }
            if (query.Live.HasValue)
            {
                var sqlOperator = query.Live.Value ? " = " : " <> ";
                sql.Append(string.Format(" and [{0}] {1} @{0}", Application.STATE_FIELD, sqlOperator));
                ps.Add(Application.STATE_FIELD, (int)ApplicationStateEnum.Live);
            }

            if (query.Free.HasValue)
            {
                sql.AppendFormat(" and [{0}] {1} 0", Application.PRICE_FIELD, query.Free.Value ? "=" : ">");
            }
            
            if (query.GradeLevels != null && query.GradeLevels.Count > 0)
            {
                sql.AppendFormat(" and exists(select * from ApplicationGradeLevel where ApplicationRef = Application.Id and GradeLevel in ({0}))"
                    , query.GradeLevels.Select(x => x.ToString()).JoinString(","));
            }
            if (query.CategoryIds != null && query.CategoryIds.Count > 0)
            {
                var categoriesParam = new List<string>();
                for (int i = 0; i < query.CategoryIds.Count; i++)
                {
                    var categoryParam = "@categoryId_" + i;
                    categoriesParam.Add(categoryParam);
                    ps.Add(categoryParam, query.CategoryIds[i]);
                }
                sql.AppendFormat(" and exists (select * from ApplicationCategory where ApplicationRef = Application.Id and CategoryRef in ({0}))"
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
                        ps.Add(keyWordParam, "@%" + keyWords[i] + "%");
                        keyWordsParams.Add(" like " + keyWordParam);
                    }
                }
                if (keyWordsParams.Count > 0)
                {
                    sql.Append(" and (");
                    sql.AppendFormat(" LOWER(Name) {0} ", keyWordsParams.JoinString(" or LOWER(Name) "));
                    sql.AppendFormat(" or LOWER(Description) {0} ", keyWordsParams.JoinString(" or LOWER(Description) "));
                    sql.AppendFormat(@" or exists(select ac.Id from ApplicationCategory ac
                                                  join Category c on Category c.Id = ac.CategoryRef
                                                  where ApplicationRef = Application.Id and (LOWER(c.Name) {0})
                                                 )", keyWords.JoinString(" or LOWER(c.Name) "));
                    sql.Append(" or exists(select * from Developer where Id = Application.DeveloperRef and (");
                    sql.AppendFormat(" LOWER(Name) {0}", keyWordsParams.JoinString(" or LOWER(Name) "));
                    sql.AppendFormat(" or LOWER(WebSite) {0} ", keyWordsParams.JoinString(" or LOWER(WebSite)"));
                    sql.Append(")))");
                }
            }

            return new DbQuery {Sql = sql.ToString(), Parameters = ps};

        }

        private IList<Application> PreparePicturesData(IList<Application> applications)
        {
            var appIdParams = new List<string>();
            var conds = new Dictionary<string, object>();
            for (int i = 0; i < applications.Count; i++)
            {
                var appIdParam = "@applicationId_" + i;
                conds.Add(appIdParam, applications[i].Id);
                appIdParams.Add(appIdParam);
            }
            var sql = "select * from ApplicationPicture where ApplicationRef in ({0})";
            sql = string.Format(sql, appIdParams.JoinString(","));

            if (conds.Count > 0)
            {
                var pictures = ReadMany<ApplicationPicture>(new DbQuery { Sql = sql, Parameters = conds });
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
            SimpleDelete<ApplicationCategory>(new Dictionary<string, object>{{ApplicationCategory.APPLICATION_REF_FIELD, id}});
            var appCategories = new List<ApplicationCategory>();
            foreach (var category in categories)
            {
                appCategories.Add(new ApplicationCategory { ApplicationRef = id, CategoryRef = category });
            }
            SimpleInsert(appCategories);
            return SelectMany<ApplicationCategory>(new Dictionary<string, object> { { ApplicationCategory.APPLICATION_REF_FIELD, id } });
        }

        public IList<ApplicationPicture> UpdatePictures(Guid id, IList<Guid> picturesId)
        {
            SimpleDelete<ApplicationPicture>(new Dictionary<string, object> { { ApplicationCategory.APPLICATION_REF_FIELD, id } });
            var appPictures = new List<ApplicationPicture>();
            foreach (var picture in picturesId)
            {
                appPictures.Add(new ApplicationPicture {ApplicationRef = id, Id = picture});
            }
            SimpleInsert(appPictures);
            return SelectMany<ApplicationPicture>(new Dictionary<string, object> { { ApplicationPicture.APPLICATION_REF_FIELD, id } });
        }

        public IList<ApplicationGradeLevel> UpdateGradeLevels(Guid id, IList<int> gradeLevels)
        {
            SimpleDelete<ApplicationGradeLevel>(new Dictionary<string, object> { { ApplicationGradeLevel.APPLICATION_REF_FIELD, id } });
            var appGradeLevels = new List<ApplicationGradeLevel>();
            foreach (var gradeLevel in gradeLevels)
            {
                appGradeLevels.Add(new ApplicationGradeLevel { ApplicationRef = id, GradeLevel = gradeLevel, Id = Guid.NewGuid()});
            }
            SimpleInsert(appGradeLevels);
            return SelectMany<ApplicationGradeLevel>(new Dictionary<string, object> { { ApplicationGradeLevel.APPLICATION_REF_FIELD, id } });
        }

        public IList<ApplicationPermission> UpdatePermissions(Guid id, IList<AppPermissionType> permissionIds)
        {
            return SelectMany<ApplicationPermission>(new Dictionary<string, object> { { ApplicationPermission.APPLICATION_REF_FIELD, id } });
        }

        public bool AppExists(Guid? currentApplicationId, string name, string url)
        {
            var sql = new StringBuilder();
            sql.Append("Select * from Application a where ");
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

        public bool? Live { get; set; }
        public bool? Free { get; set; }

        public string Filter { get; set; } 

        public ApplicationQuery()
        {
            Start = 0;
            Count = int.MaxValue;
            OrderBy = Application.ID_FIELD;
            IncludeInternal = false;
            Live = null;
            Free = null;
        }

    }
}