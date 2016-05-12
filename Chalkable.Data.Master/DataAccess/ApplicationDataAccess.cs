﻿using System;
using System.Collections.Generic;
using System.Data.Common;
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
            res.Ban = SqlTools.ReadBoolNull(reader, ApplicationDistrictOption.BAN_FIELD);
            return res;
        }

        private DbQuery BuildGetApplicationsQuery(ApplicationQuery query)
        {
            var res = new DbQuery();
            if (query.DistrictId.HasValue)
            {
                res.Sql.AppendFormat(
                    @"select Application.*, 
                        (select avg(rating) from ApplicationRating where ApplicationRef = Application.Id) as Avg,
                        isnull(ApplicationDistrictOption.{1}, 0) as {1}
                    from 
                        Application 
                        left join ApplicationDistrictOption on Application.Id = ApplicationDistrictOption.ApplicationRef and ApplicationDistrictOption.{0} = @{0}
                    where 
                        1 = 1", ApplicationDistrictOption.DISTRICT_REF_FIELD, ApplicationDistrictOption.BAN_FIELD);
                res.Parameters.Add(ApplicationDistrictOption.DISTRICT_REF_FIELD, query.DistrictId);
                if (query.Ban.HasValue)
                {
                    res.Sql.AppendFormat(" and isnull(ApplicationDistrictOption.{0}, 0) = @{0}", ApplicationDistrictOption.BAN_FIELD);
                    res.Parameters.Add(ApplicationDistrictOption.BAN_FIELD, query.Ban);
                }
            }
            else
                res.Sql.Append(@"select Application.*, (select avg(rating) from ApplicationRating where ApplicationRef = Application.Id) as Avg, null as Ban from Application where 1 = 1");
            
            if (query.Role != CoreRoles.SUPER_ADMIN_ROLE.Id && query.Role != CoreRoles.APP_TESTER_ROLE.Id)
            {
                if (!query.IncludeInternal)
                {
                    res.Sql.Append($" and [{nameof(Application.IsInternal)}] <> 1");
                }
                if (query.OnlyForInstall)
                {
                    if (query.Role == CoreRoles.TEACHER_ROLE.Id)
                        res.Sql.Append($" and ([{nameof(Application.HasStudentMyApps)}] = 1 or [{nameof(Application.HasTeacherMyApps)}] = 1 or [{nameof(Application.CanAttach)}] = 1 or [{nameof(Application.HasTeacherExternalAttach)}] = 1 or [{nameof(Application.HasStudentExternalAttach)}] = 1)");
                    if (query.Role == CoreRoles.STUDENT_ROLE.Id)
                        res.Sql.Append($" and ([{nameof(Application.HasStudentMyApps)}] = 1 or [{nameof(Application.HasStudentExternalAttach)}] = 1)");
                }
            }

            if (query.DeveloperId.HasValue)
            {
                res.Sql.Append(string.Format(" and [{0}] = @{0}", nameof(Application.DeveloperRef)));
                res.Parameters.Add(nameof(Application.DeveloperRef), query.DeveloperId);
            }
            if (query.Id.HasValue)
            {
                res.Sql.AppendFormat(" and [{0}] = @{0}", nameof(Application.Id));
                res.Parameters.Add(nameof(Application.Id), query.Id.Value);
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
                res.Sql.Append(string.Format(" and [{0}] {1} @{0}", nameof(Application.State), sqlOperator));
                res.Parameters.Add(nameof(Application.State), state);
            }
            
            if (query.Free.HasValue)
            {
                res.Sql.AppendFormat(" and [{0}] {1} 0", nameof(Application.Price), query.Free.Value ? "=" : ">");
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
            q = Orm.PaginationSelect(q, query.OrderBy, Orm.OrderType.Desc, query.Start, query.Count);
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
                    Orm.SimpleDelete<ApplicationDistrictOption>(new AndQueryCondition { {ApplicationDistrictOption.APPLICATION_REF_FIELD, id} }),
                    Orm.SimpleDelete<ApplicationPermission>(new AndQueryCondition { {nameof(ApplicationPermission.ApplicationRef), id} }),
                    Orm.SimpleDelete<ApplicationCategory>(new AndQueryCondition { {nameof(ApplicationCategory.ApplicationRef), id} }),
                    Orm.SimpleDelete<ApplicationPicture>(new AndQueryCondition { {nameof(ApplicationPicture.ApplicationRef), id} }),
                    Orm.SimpleDelete<ApplicationRating>(new AndQueryCondition { {nameof(ApplicationRating.ApplicationRef), id} }),
                    Orm.SimpleDelete<ApplicationGradeLevel>(new AndQueryCondition { {nameof(ApplicationGradeLevel.ApplicationRef), id} }),
                    Orm.SimpleDelete<ApplicationStandard>(new AndQueryCondition { {nameof(ApplicationStandard.ApplicationRef), id} }),
                    Orm.SimpleDelete<Application>(new AndQueryCondition { {nameof(Application.Id), id} }),

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

        public void SetDistrictOption(Guid applicationId, Guid districtId, bool ban)
        {
            IDictionary<string, object> ps = new Dictionary<string, object>
            {
                {"@applicationId", applicationId},
                {"@districtId", districtId},
                {"@ban", ban},
            };
            ExecuteStoredProcedure("spSetApplicationDistrictOption", ps);
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
        public Guid? DistrictId { get; set; }
        public bool? Ban { get; set; }

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
            OrderBy = nameof(Application.Id);
            IncludeInternal = false;
            Live = null;
            Free = null;
            OnlyForInstall = true;
        }

    }
}