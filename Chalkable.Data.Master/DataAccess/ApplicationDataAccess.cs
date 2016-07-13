using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
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
            app.Pictures = SelectMany<ApplicationPicture>(new AndQueryCondition { { nameof(ApplicationPicture.ApplicationRef), app.Id } });
            app.Permissions = SelectMany<ApplicationPermission>(new AndQueryCondition { { nameof(ApplicationPermission.ApplicationRef), app.Id } });
            app.Categories = SelectMany<ApplicationCategory>(new AndQueryCondition { { nameof(ApplicationCategory.ApplicationRef), app.Id } });
            app.GradeLevels = SelectMany<ApplicationGradeLevel>(new AndQueryCondition { { nameof(ApplicationGradeLevel.ApplicationRef), app.Id } });
            app.ApplicationStandards = SelectMany<ApplicationStandard>(new AndQueryCondition { {nameof(ApplicationStandard.ApplicationRef), app.Id} });
        }

        public Application GetApplicationById(Guid id)
        {
            var res = GetById(id);
            LoadApplicationData(res);
            return res;
        }

        public Application GetLiveApplicationByUrl(string url)
        {
            var res = SelectOne<Application>(new AndQueryCondition
                    {
                        {nameof(Application.Url), url},
                        {nameof(Application.State), ApplicationStateEnum.Live}
                    });

            LoadApplicationData(res);
            return res;
        }

        public Application GetDraftApplicationByUrl(string url)
        {
            var res = SelectOne<Application>(new AndQueryCondition
                    {
                        {nameof(Application.Url), url},
                        {nameof(Application.State), ApplicationStateEnum.Live, ConditionRelation.NotEqual}
                    });

            LoadApplicationData(res);
            return res;
        }

        public Application GetApplication(ApplicationQuery query)
        {
            query.Count = 1;
            var q = BuildGetApplicationsQuery(query);
            var res = Read(q, r=> {     r.Read();
                                        return ReadApplication(r);
                                    });
            LoadApplicationData(res);
            return res;
        }

        private Application ReadApplication(DbDataReader reader)
        {
            var res = reader.Read<Application>();
            res.Ban = SqlTools.ReadBoolNull(reader, nameof(Application.Ban));
            return res;
        }

        private DbQuery BuildSelectApplicationQuery(ApplicationQuery query)
        {
            var q = new DbQuery();

            var banColumn = $@"Case When @schoolId is not null 
                               Then IsNull({nameof(ApplicationSchoolOption.Banned)}, 0) Else null End";

            var mainSelect = $@"Select 
	                                [{nameof(Application)}].*,
	                                ({banColumn})       As Ban
                                From 
	                                [{nameof(Application)}]
	                                left join {nameof(ApplicationSchoolOption)}
		                            On [{nameof(Application.Id)}] = {nameof(ApplicationSchoolOption.ApplicationRef)} 
		                                And {nameof(ApplicationSchoolOption.SchoolRef)} = @schoolId";

            q.Sql.Append(mainSelect);
            q.Parameters.Add("schoolId", query.SchoolId);

            return q;
        }

        private void AddSimpleParamsToApplicationQuery(DbQuery result, ApplicationQuery query)
        {
            var conditions = new AndQueryCondition();

            if (query.Role != CoreRoles.SUPER_ADMIN_ROLE.Id && query.Role != CoreRoles.APP_TESTER_ROLE.Id)
            {
                if (!query.IncludeInternal)
                    conditions.Add(nameof(Application.IsInternal), 1, ConditionRelation.NotEqual);

                if (query.MyApps.HasValue)
                {
                    if (query.Role == CoreRoles.STUDENT_ROLE.Id)
                        conditions.Add(nameof(Application.HasStudentMyApps), query.MyApps, ConditionRelation.Equal);
                    if (query.Role == CoreRoles.TEACHER_ROLE.Id)
                        conditions.Add(nameof(Application.HasTeacherMyApps), query.MyApps, ConditionRelation.Equal);
                    if (query.Role == CoreRoles.DISTRICT_ADMIN_ROLE.Id)
                        conditions.Add(nameof(Application.HasAdminMyApps), query.MyApps, ConditionRelation.Equal);
                }
            }

            if (query.CanAttach.HasValue)
                conditions.Add(nameof(Application.CanAttach), query.CanAttach, ConditionRelation.Equal);

            if (query.DeveloperId.HasValue)
                conditions.Add(nameof(Application.DeveloperRef), query.DeveloperId, ConditionRelation.Equal);

            if (query.Id.HasValue)
                conditions.Add(nameof(Application.Id), query.Id, ConditionRelation.Equal);

            if (query.Live.HasValue || query.State.HasValue)
            {
                var relation = ConditionRelation.Equal;
                var state = (int?)query.State;

                if (query.Live.HasValue)
                {
                    if (!query.Live.Value)
                        relation = ConditionRelation.NotEqual;

                    state = (int?)ApplicationStateEnum.Live;
                }

                conditions.Add(nameof(Application.State), state, relation);
            }

            conditions.BuildSqlWhere(result, nameof(Application));
        }

        private DbQuery BuildGetApplicationsQuery(ApplicationQuery query)
        {
            var res = BuildSelectApplicationQuery(query);
            
            AddSimpleParamsToApplicationQuery(res, query);

            //Adding complex params to query
            if (query.SchoolId.HasValue && query.Ban.HasValue)
            {
                res.Sql.Append($" And IsNull({nameof(ApplicationSchoolOption.Banned)},0) = @banned");
                res.Parameters.Add("banned", query.Ban);
            }

                if (query.GradeLevels != null && query.GradeLevels.Count > 0)
            {
                var gradeLevelsPredicate = $@"Exists(
                        Select * 
                        From {nameof(ApplicationGradeLevel)} 
                        Where 
                            {nameof(ApplicationGradeLevel.ApplicationRef)} = {nameof(Application)}.Id 
                            And {nameof(ApplicationGradeLevel.GradeLevel)} in (Select * From @gradeLevels))";

                res.Sql.Append($" And {gradeLevelsPredicate}");
                res.Parameters.Add("gradeLevels", query.GradeLevels);
            }

            if (query.CategoryIds != null && query.CategoryIds.Count > 0)
            {
                var categoryPredicate = $@"Exists(
                        Select * 
                        From {nameof(ApplicationCategory)} 
                        Where 
                            {nameof(ApplicationCategory.ApplicationRef)} = Application.Id 
                            And {nameof(ApplicationCategory.CategoryRef)} in (Select * from @categoryIds))";

                res.Sql.Append($"And {categoryPredicate}");
                res.Parameters.Add("categoryIds", query.CategoryIds);
            }

            if (string.IsNullOrEmpty(query.Filter))
                return res;

            var keyWords = query.Filter.Split(new [] {' ', '.', ',', ';'}, StringSplitOptions.RemoveEmptyEntries);
            var keyWordsParams = new List<string>();

            for (var i = 0; i < keyWords.Length; ++i)
            {
                if (string.IsNullOrEmpty(keyWords[i]))
                    continue;

                var keyWordParam = "@keyWord_" + i;
                res.Parameters.Add(keyWordParam, $"%{keyWords[i]}%");
                keyWordsParams.Add(" like " + keyWordParam);
            }
            if (keyWordsParams.Count <= 0)
                return res;

            var appCategory = $@"Select {nameof(ApplicationCategory.Id)}
                                 From   {nameof(ApplicationCategory)} join {nameof(Category)}
                                        On  {nameof(ApplicationCategory)}.CategoryRef = {nameof(Category)}.Id
                                 Where  {nameof(ApplicationCategory.ApplicationRef)} = [Application].Id
                                        And (Lower({nameof(Category)}.Name) {keyWordsParams.JoinString(" or LOWER(Category.Name) ")} )" ;

            var devQuery = $@"Select * From {nameof(Developer)} 
                              Where {nameof(Developer.Id)} = {nameof(Application.DeveloperRef)}
                                    And (   Lower({nameof(Developer.Name)})    {keyWordsParams.JoinString(" or Lower(Name) ")}
                                         or Lower({nameof(Developer.WebSite)}) {keyWordsParams.JoinString(" or Lower(WebSite) ")} )";
            var searchQuery = 
                $@" And (   Lower({nameof(Application.Name)})        {keyWordsParams.JoinString(" or Lower (Name) ")}
                         or Lower({nameof(Application.Description)}) {keyWordsParams.JoinString(" or Lower (Description) ")}
                         or Exists({appCategory})
                         or Exists({devQuery}) )";

            res.Sql.Append(searchQuery);

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
            q = Orm.PaginationSelect(q, query.OrderBy, query.OrderDesc ? Orm.OrderType.Desc : Orm.OrderType.Asc, query.Start, query.Count);
            var paginatedApps = ReadPaginatedResult(q, query.Start, query.Count, r =>
            {
                var res = new List<Application>();
                while (r.Read())
                {
                    res.Add(ReadApplication(r));
                }
                return res;
            });
            return PreparePicturesData(paginatedApps) as PaginatedList<Application>;
        }  

        public IList<ApplicationCategory> UpdateCategories(Guid id, IList<Guid> categories)
        {
            SimpleDelete<ApplicationCategory>(new AndQueryCondition { { nameof(ApplicationCategory.ApplicationRef), id } });
            IList<ApplicationCategory> appCategories = new List<ApplicationCategory>();
            if (categories != null)
            {
                foreach (var category in categories)
                {
                    appCategories.Add(new ApplicationCategory { Id = Guid.NewGuid(), ApplicationRef = id, CategoryRef = category });
                }
            }
            SimpleInsert(appCategories);
            return SelectMany<ApplicationCategory>(new AndQueryCondition { { nameof(ApplicationCategory.ApplicationRef), id } });
        }

        public IList<ApplicationPicture> UpdatePictures(Guid id, IList<Guid> picturesId)
        {
            SimpleDelete<ApplicationPicture>(new AndQueryCondition { { nameof(ApplicationPicture.ApplicationRef), id } });
            IList<ApplicationPicture> appPictures = new List<ApplicationPicture>();
            if (picturesId != null)
            {
                foreach (var picture in picturesId)
                {
                    appPictures.Add(new ApplicationPicture { ApplicationRef = id, Id = picture });
                }
            }
            SimpleInsert(appPictures);
            return SelectMany<ApplicationPicture>(new AndQueryCondition { { nameof(ApplicationPicture.ApplicationRef), id } });
        }

        public IList<ApplicationGradeLevel> UpdateGradeLevels(Guid id, IList<int> gradeLevels)
        {
            SimpleDelete<ApplicationGradeLevel>(new AndQueryCondition { { nameof(ApplicationGradeLevel.ApplicationRef), id } });
            IList<ApplicationGradeLevel> appGradeLevels = new List<ApplicationGradeLevel>();
            if (gradeLevels != null)
            {
                foreach (var gradeLevel in gradeLevels)
                {
                    appGradeLevels.Add(new ApplicationGradeLevel { ApplicationRef = id, GradeLevel = gradeLevel, Id = Guid.NewGuid() });
                }
            }
            SimpleInsert(appGradeLevels);
            return SelectMany<ApplicationGradeLevel>(new AndQueryCondition { { nameof(ApplicationGradeLevel.ApplicationRef), id } });
        }

        public IList<ApplicationPermission> UpdatePermissions(Guid id, IList<AppPermissionType> permissionIds)
        {
            SimpleDelete<ApplicationPermission>(new AndQueryCondition { { nameof(ApplicationPermission.ApplicationRef), id } });
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
            return SelectMany<ApplicationPermission>(new AndQueryCondition { { nameof(ApplicationPermission.ApplicationRef), id } });
        }

        public IList<ApplicationStandard> UpdateApplicationStandards(Guid id, IList<Guid> standardsIds)
        {
            SimpleDelete<ApplicationStandard>(new AndQueryCondition{{nameof(ApplicationStandard.ApplicationRef), id}});
            IList<ApplicationStandard> applicationStandards = new List<ApplicationStandard>();
            if (standardsIds != null)
            {
                foreach (var standardId in standardsIds)
                {
                    applicationStandards.Add(new ApplicationStandard
                    {
                        ApplicationRef = id,
                        StandardRef = standardId
                    });
                }
            }
            SimpleInsert(applicationStandards);
            return SelectMany<ApplicationStandard>(new AndQueryCondition { { nameof(ApplicationPermission.ApplicationRef), id } });
        } 

        public bool AppExists(Guid? currentApplicationId, string name, string url)
        {
            var query = new DbQuery();
            query.Sql.Append("Select * from Application a where ");
            query.Sql.Append("(").Append(string.Format("[{0}] = @{0} or ([{1}] = @{1} and [{1}] is not null)", nameof(Application.Name), nameof(Application.Url))).Append(")");
            query.Sql.Append(" and (").Append(string.Format("[{0}] is null or [{0}] <> @{0}", nameof(Application.OriginalRef))).Append(")");
            query.Sql.AppendFormat(" and a.[Id] <> @{0}", nameof(Application.Id));
            query.Sql.Append(string.Format(" and not exists (select * from Application where [{0}] = a.[{1}] and [{1}] = @{1})", nameof(Application.OriginalRef), nameof(Application.Id)));
            query.Parameters.Add(nameof(Application.Id), currentApplicationId);
            query.Parameters.Add(nameof(Application.Name), name);
            query.Parameters.Add(nameof(Application.Url), url);
            query.Parameters.Add(nameof(Application.OriginalRef), currentApplicationId);
            return Exists(query);
        }

        public override void Delete(Guid id)
        {
            var query = new DbQuery(new List<DbQuery>
                {
                    Orm.SimpleDelete<ApplicationSchoolOption>(new AndQueryCondition { {nameof(ApplicationSchoolOption.ApplicationRef), id} }),
                    Orm.SimpleDelete<ApplicationPermission>(  new AndQueryCondition { {nameof(ApplicationPermission.ApplicationRef), id} }),
                    Orm.SimpleDelete<ApplicationCategory>(    new AndQueryCondition { {nameof(ApplicationCategory.ApplicationRef), id} }),
                    Orm.SimpleDelete<ApplicationPicture>(     new AndQueryCondition { {nameof(ApplicationPicture.ApplicationRef), id} }),
                    Orm.SimpleDelete<ApplicationGradeLevel>(  new AndQueryCondition { {nameof(ApplicationGradeLevel.ApplicationRef), id} }),
                    Orm.SimpleDelete<ApplicationStandard>(    new AndQueryCondition { {nameof(ApplicationStandard.ApplicationRef), id} }),
                    Orm.SimpleDelete<Application>(            new AndQueryCondition { {nameof(Application.Id), id} }),

                });
            ExecuteNonQueryParametrized(query.Sql.ToString(), query.Parameters);
        }

        public override IList<Application> GetByIds(IList<Guid> ids)
        {
            if (ids == null || ids.Count == 0)
                return new List<Application>();
            var tableName = typeof(Application).Name;
            var dbQuery = new DbQuery();
            dbQuery.Sql.AppendFormat(@"select * from [{0}]", tableName);
            dbQuery.Sql.AppendFormat(" where [{0}].[{1}] in ({2})", tableName, nameof(Application.Id)
                    , ids.Select(x => "'" + x.ToString() + "'").JoinString(","));
            var res = ReadMany<Application>(dbQuery);
            if (res.Count == 0) return res;
            return PreparePicturesData(res);
        }

        public IList<Application> GetSuggestedApplications(IList<Guid> abIds, int start, int count)
        {
            IDictionary<string, object> parameters = new Dictionary<string, object>
                {
                    {"start", start},
                    {"count", count},
                    {"academicBenchmarkIds", abIds?? new List<Guid>()}
                };
            IList<Application> res;
            using (var reader = ExecuteStoredProcedureReader("spGetSuggestedApplications", parameters))
            {
                res = reader.ReadList<Application>();
            }
            return PreparePicturesData(res);
        }
    }

    public class ApplicationQuery
    {
        public Guid? Id { get; set; }
        public int Role { get; set; }
        public bool IncludeInternal { get; set; }
        public IList<Guid> CategoryIds { get; set; }
        public IList<int> GradeLevels { get; set; }
        public Guid? DeveloperId { get; set; }
        public string OrderBy { get; set; }
        public bool OrderDesc { get; set; }
        public Guid? SchoolId { get; set; }
        public bool? Ban { get; set; }
        public bool? MyApps { get; set; }

        public int Start { get; set; }
        public int Count { get; set; }

        public ApplicationStateEnum? State { get; set; }

        public bool? Live { get; set; }

        public string Filter { get; set; }
        public bool? CanAttach { get; set; }

        public ApplicationQuery()
        {
            Start = 0;
            Count = int.MaxValue;
            OrderBy = nameof(Application.Id);
            OrderDesc = true;
            IncludeInternal = false;
            Live = null;
            CanAttach = null;
        }

    }
}