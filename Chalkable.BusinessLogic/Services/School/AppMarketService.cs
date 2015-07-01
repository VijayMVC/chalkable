﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Chalkable.BusinessLogic.Model;
using Chalkable.BusinessLogic.Security;
using Chalkable.Common;
using Chalkable.Common.Exceptions;
using Chalkable.Data.Common;
using Chalkable.Data.Common.Orm;
using Chalkable.Data.Master.Model;
using Chalkable.Data.School.DataAccess;
using Chalkable.Data.School.Model;
using Chalkable.Data.School.Model.ApplicationInstall;

namespace Chalkable.BusinessLogic.Services.School
{
    public interface IAppMarketService
    {
        IDictionary<Guid, int> GetNotInstalledStudentCountPerApp(int staffId, int classId, int markingPeriodId);
 
        IList<ApplicationInstall> ListInstalledAppInstalls(int personId);
        IList<ApplicationInstall> ListInstalledByAppId(Guid applicationId);
        ApplicationInstallAction Install(Guid applicationId, int? personId, IList<int> roleIds, IList<int> classIds, IList<Guid> departmentIds, IList<int> gradeLevelIds, int schoolYearId, DateTime dateTime);
        IList<ApplicationInstall> GetInstallations(Guid applicationId, int personId, bool owners = true);
        ApplicationInstall GetInstallationForPerson(Guid applicationId, int personId);
        ApplicationInstall GetInstallationById(int applicationInstallId);
        bool IsPersonForInstall(Guid applicationId);
        void Uninstall(int applicationInstallationId);
        void Uninstall(IList<int> applicationInstallIds);
        bool CanInstall(Guid applicationId, int? schoolPersonId, IList<int> roleIds, IList<int> classIds, IList<int> gradelevelIds, IList<Guid> departmentIds);

        IList<PersonsForApplicationInstallCount> GetPersonsForApplicationInstallCount(Guid applicationId, int? personId, IList<int> roleIds, IList<int> classIds, IList<Guid> departmentIds, IList<int> gradeLevelIds);
        IList<StudentCountToAppInstallByClass> GetStudentCountToAppInstallByClass(int schoolYearId, Guid applicationId);
        
        ApplicationTotalPriceInfo GetApplicationTotalPrice(Guid applicationId, int? schoolPerson, IList<int> roleids, IList<int> classids, IList<int> gradelevelids, IList<Guid> departmentids);
        IList<ApplicationInstallHistory> GetApplicationInstallationHistory(Guid applicationId);
    }

    public class AppMarketService : SchoolServiceBase, IAppMarketService
    {
        private const string APP_INSTALLED_FOR_FMT = "{0} was installed for : \n";
        private const string APP_CLASS_FMT = "Class {0} \n";
        private const string APP_GRADE_LEVEL_FMT = "Grade Level {0} \n";
        private const string APP_DEPARTMENT_FMT = "Department {0} \n";
        private const string APP_ROLE_FMT = "Role {0} \n";

        public AppMarketService(IServiceLocatorSchool serviceLocator) : base(serviceLocator)
        {
        }

        public IList<ApplicationInstall> ListInstalledAppInstalls(int personId)
        {
            int? syId = Context.SchoolYearId;
            if (!syId.HasValue)
                syId = ServiceLocator.SchoolYearService.GetCurrentSchoolYear().Id;
            using (var uow = Read())
            {
                var da = new ApplicationInstallDataAccess(uow);
                return da.GetInstalled(personId, syId.Value);
            }
        }
        
        public IList<ApplicationInstall> ListInstalledByAppId(Guid applicationId)
        {
            if (!Context.SchoolYearId.HasValue)
                throw new ChalkableException(ChlkResources.ERR_CANT_DETERMINE_SCHOOL_YEAR);
            using (var uow = Read())
            {
                var da = new ApplicationInstallDataAccess(uow);
                return da.GetInstalledByAppId(applicationId, Context.SchoolYearId.Value);
            }
        }

        public ApplicationInstallAction Install(Guid applicationId, int? personId, IList<int> roleIds, IList<int> classIds,
                                                IList<Guid> departmentIds, IList<int> gradeLevelIds, int schoolYearId, DateTime dateTime)
        {
            if (!Context.SCEnabled)
                throw new StudyCenterDisabledException();
            if(!Context.PersonId.HasValue)
                throw new UnassignedUserException();

            if (!CanInstall(applicationId, personId, roleIds, classIds, gradeLevelIds, departmentIds))
                throw new ChalkableException(ChlkResources.ERR_APP_NOT_ENOUGH_MONEY);

            var app = ServiceLocator.ServiceLocatorMaster.ApplicationService.GetApplicationById(applicationId);
            using (var uow = Update())
            {
                var da = new ApplicationInstallDataAccess(uow);
                var persons = da.GetPersonsForApplicationInstall(applicationId, Context.PersonId.Value, personId, roleIds, departmentIds, gradeLevelIds, classIds, Context.Role.Id
                                                   , app.HasAdminMyApps, app.HasTeacherMyApps, app.HasStudentMyApps, app.CanAttach, schoolYearId);
                var spIds = persons.Select(x => x.PersonId).Distinct().ToList();
                var schoolYear = ServiceLocator.SchoolYearService.GetSchoolYearById(schoolYearId);
                var res = RegisterInstallAction(uow, app, personId, roleIds, classIds, departmentIds, gradeLevelIds, schoolYearId, dateTime);
                var appInstalls = new List<ApplicationInstall>();

                foreach (var spId in spIds)
                {
                    appInstalls.Add(new ApplicationInstall
                    {
                        ApplicationRef = app.Id,
                        PersonRef = spId,
                        OwnerRef = Context.PersonId.Value,
                        Active = true,
                        SchoolYearRef = schoolYear.Id,
                        InstallDate = dateTime,
                        AppInstallActionRef = res.Id
                    });
                }
                da.Insert(appInstalls);
                res.ApplicationInstalls = appInstalls;
                uow.Commit();
                return res;
            }
        }

        private ApplicationInstallAction RegisterInstallAction(UnitOfWork uow, Application app, int? personId, IList<int> roleids, IList<int> classids, IList<Guid> departmentids
            , IList<int> gradelevelids, int schoolYearId, DateTime dateTime)
        {
            Trace.Assert(Context.PersonId.HasValue);
            Trace.Assert(Context.SchoolYearId.HasValue);
            var res = new ApplicationInstallAction
            {
                ApplicationRef = app.Id,
                PersonRef = personId,
                Description = string.Empty,
                OwnerRef = Context.PersonId.Value,
                SchoolYearRef = schoolYearId,
                InstallDate = dateTime,
                OwnerRoleId = Context.RoleId
            };
            var ada = new ApplicationInstallActionDataAccess(uow);
            ada.Insert(res);
            res = ada.GetLastAppInstallAction(app.Id, Context.PersonId.Value);
            var descriptionBuilder = new StringBuilder();
            descriptionBuilder.AppendFormat(APP_INSTALLED_FOR_FMT, app.Name);
            if (Context.Role.Id == CoreRoles.TEACHER_ROLE.Id && classids != null)
            {

                var teacherClasses = new ClassDataAccess(uow).GetTeacherClasses(Context.SchoolYearId.Value, Context.PersonId.Value, null);
                teacherClasses = teacherClasses.Where(x => classids.Contains(x.Id)).ToList();
                var da = new ApplicationInstallActionClassesDataAccess(uow);
                var appInstallAcClasses = new List<ApplicationInstallActionClasses>();
                foreach (var teacherClass in teacherClasses)
                {
                    descriptionBuilder.AppendFormat(APP_CLASS_FMT, teacherClass.Name);
                    appInstallAcClasses.Add(new ApplicationInstallActionClasses
                    {
                        AppInstallActionRef = res.Id,
                        ClassRef = teacherClass.Id
                    });
                }
                da.Insert(appInstallAcClasses);
            }

            if (BaseSecurity.IsDistrictAdmin(Context))
            {
                if (gradelevelids != null)
                {
                    var gradeLevels = new DataAccessBase<GradeLevel>(uow).GetAll()
                        .Where(x => app.GradeLevels.Any(y => y.GradeLevel == x.Number))
                        .Where(x => gradelevelids.Contains(x.Id));
                    var da = new ApplicationInstallActionGradeLevelDataAccess(uow);
                    var aiaGls = new List<ApplicationInstallActionGradeLevel>();
                    foreach (var gradeLevel in gradeLevels)
                    {
                        descriptionBuilder.AppendFormat(APP_GRADE_LEVEL_FMT, gradeLevel.Name);
                        aiaGls.Add(new ApplicationInstallActionGradeLevel
                        {
                            AppInstallActionRef = res.Id,
                            GradeLevelRef = gradeLevel.Id
                        });
                    }
                    da.Insert(aiaGls);
                }
                if (departmentids != null)
                {
                    var departments = ServiceLocator.ServiceLocatorMaster.ChalkableDepartmentService.GetChalkableDepartments();
                    departments = departments.Where(x => departmentids.Contains(x.Id)).ToList();
                    var da = new ApplicationInstallActionDepartmentDataAccess(uow);
                    var aiaDepartments = new List<ApplicationInstallActionDepartment>();
                    foreach (var department in departments)
                    {
                        descriptionBuilder.AppendFormat(APP_DEPARTMENT_FMT, department.Name);
                        aiaDepartments.Add(new ApplicationInstallActionDepartment
                        {
                            AppInstallActionRef = res.Id,
                            DepartmentRef = department.Id,
                        });
                    }
                    da.Insert(aiaDepartments);
                }
                if (roleids != null)
                {
                    var roles = roleids.Select(CoreRoles.GetById).ToList();
                    var da = new ApplicationInstallActionRoleDataAccess(uow);
                    var appInstallActionRoles = new List<ApplicationInstallActionRole>();
                    foreach (var role in roles)
                    {
                        descriptionBuilder.AppendFormat(APP_ROLE_FMT, role.Name);
                        appInstallActionRoles.Add(new ApplicationInstallActionRole
                        {
                            AppInstallActionRef = res.Id,
                            RoleId = role.Id
                        });
                    }
                    da.Insert(appInstallActionRoles);
                }
            }
            res.Description = descriptionBuilder.ToString();
            ada.Update(res);
            return res;
        }

        public IList<ApplicationInstall> GetInstallations(Guid applicationId, int personId, bool owners = true)
        {
            if (!BaseSecurity.IsDistrictAdmin(Context) && Context.PersonId != personId)
                throw new ChalkableSecurityException();
            using (var uow = Read())
            {
                var da = new ApplicationInstallDataAccess(uow);
                var ps = new AndQueryCondition
                    {
                        {ApplicationInstall.APPLICATION_REF_FIELD, applicationId},
                        {ApplicationInstall.ACTIVE_FIELD, true}
                    };
                if (owners)
                    ps.Add(ApplicationInstall.OWNER_REF_FIELD, personId);
                else
                    ps.Add(ApplicationInstall.PERSON_REF_FIELD, personId);
                return da.GetAll(ps);
            }
        }

        public ApplicationInstall GetInstallationForPerson(Guid applicationId, int personId)
        {
            //TODO: security

            var sy = ServiceLocator.SchoolYearService.GetCurrentSchoolYear();
            using (var uow = Read())
            {
                var da = new ApplicationInstallDataAccess(uow);
                var ps = new AndQueryCondition
                    {
                        {ApplicationInstall.APPLICATION_REF_FIELD, applicationId},
                        {ApplicationInstall.PERSON_REF_FIELD, personId},
                        {ApplicationInstall.ACTIVE_FIELD, true},
                        {ApplicationInstall.SCHOOL_YEAR_REF_FIELD, sy.Id}
                    };
                return da.GetAll(ps).FirstOrDefault();
            }
        }

        public ApplicationInstall GetInstallationById(int applicationInstallId)
        {
            using (var uow = Read())
            {
                var da = new ApplicationInstallDataAccess(uow);
                return da.GetById(applicationInstallId);
            }
        }

        public bool IsPersonForInstall(Guid applicationId)
        {
            var r = GetPersonsForApplicationInstallCount(applicationId, null, null, null, null, null).ToList();
            return r.First(x => x.Type == PersonsForAppInstallTypeEnum.Total).Count > 0;
        }

        public void Uninstall(int applicationInstallationId)
        {
            Uninstall(new List<int> {applicationInstallationId});
        }

        public void Uninstall(IList<int> applicationInstallIds)
        {
            using (var uow = Update())
            {
                var da = new ApplicationInstallDataAccess(uow);
                var appInstalls = da.GetByIds(applicationInstallIds);
                foreach (var applicationInstall in appInstalls)
                {
                    if (!ApplicationSecurity.CanUninstall(Context, applicationInstall))
                        throw new ChalkableSecurityException();
                    applicationInstall.Active = false;
                }
                da.Update(appInstalls);
                uow.Commit();
            }
        }


        public bool CanInstall(Guid applicationId, int? schoolPersonId, IList<int> roleIds, IList<int> classIds, IList<int> gradelevelIds, IList<Guid> departmentIds)
        {
            if (!Context.SchoolLocalId.HasValue)
                throw new UnassignedUserException();
            var priceData = GetApplicationTotalPrice(applicationId, schoolPersonId, roleIds, classIds, gradelevelIds, departmentIds);
            var cnt = priceData.TotalCount;//priceData.ApplicationInstallCountInfo.First(x => x.Type == PersonsForAppInstallTypeEnum.Total).Count.Value;
            var bugetBalance = 0; // todo : implement fund service ServiceLocator.ServiceLocatorMaster.FundService.GetUserBalance(Context.UserId);
            return (bugetBalance - priceData.TotalPrice >= 0 || priceData.TotalPrice == 0) && cnt > 0;
        }

        public IList<StudentCountToAppInstallByClass> GetStudentCountToAppInstallByClass(int schoolYearId, Guid applicationId)
        {
            if (!BaseSecurity.IsDistrictOrTeacher(Context))
                throw new ChalkableSecurityException();
            var app = ServiceLocator.ServiceLocatorMaster.ApplicationService.GetApplicationById(applicationId);
            var res = new List<StudentCountToAppInstallByClass>();
            if (app.HasStudentMyApps || app.CanAttach)
            {
                using (var uow = Read())
                {
                    var da = new ApplicationInstallDataAccess(uow);
                    res.AddRange(da.GetStudentCountToAppInstallByClass(applicationId, schoolYearId, Context.PersonId ?? 0, Context.Role.Id));
                }
            }
            return res;
        }

        public ApplicationTotalPriceInfo GetApplicationTotalPrice(Guid applicationId, int? schoolPerson, IList<int> roleids, IList<int> classids, IList<int> gradelevelids, IList<Guid> departmentids)
        {
            var isForAll = !(schoolPerson.HasValue || (roleids != null && roleids.Count > 0) || (classids != null && classids.Count > 0) ||
                                   (gradelevelids != null && gradelevelids.Count > 0) || (departmentids != null && departmentids.Count > 0));
            //var r = GetPersonsForApplicationInstallCount(applicationId, schoolPerson, roleids, classids, departmentids, gradelevelids).ToList();

            var app = ServiceLocator.ServiceLocatorMaster.ApplicationService.GetApplicationById(applicationId);
            var persons = GetPersonsForApplicationInstall(app, schoolPerson, roleids, classids, departmentids, gradelevelids);
            var totalPrice = GetApplicationTotalPrice(app, persons, isForAll);
            var totalCount = persons.GroupBy(x => x.PersonId).Select(x => x.Key).Count();
            return ApplicationTotalPriceInfo.Create(totalPrice, totalCount);
        }

        public IList<ApplicationInstallHistory> GetApplicationInstallationHistory(Guid applicationId)
        {
            BaseSecurity.EnsureDistrictAdmin(Context);
            return DoRead(u => new ApplicationInstallActionDataAccess(u).GetApplicationInstallationHistory(applicationId));
        }

        private decimal GetApplicationTotalPrice(Application app, IEnumerable<PersonsForApplicationInstall> applicationInstallCount, bool isForAll)
        {

            decimal totalPrice = 0;
            if (app.Price != 0)
            {
                var totalCount = applicationInstallCount.GroupBy(x=>x.PersonId).Select(x=>x.Key).Count();
                if (BaseSecurity.IsDistrictAdmin(Context))
                {
                    totalPrice = app.Price * totalCount;
                    return totalPrice > app.PricePerSchool && isForAll && app.PricePerSchool.HasValue ? app.PricePerSchool.Value : totalPrice;
                }
                if (Context.Role.Id == CoreRoles.TEACHER_ROLE.Id)
                {
                    var personsPerClassComplex = applicationInstallCount.Where(x => x.Type == PersonsForAppInstallTypeEnum.Class).ToList();
                    //var totalInClassCount = countPerClassComplex.Sum(x => x.Count);
                    var personids = new HashSet<int>();
                    var classPersonsDic = personsPerClassComplex.GroupBy(x => x.GroupId).ToDictionary(x => x.Key, x => x.ToList());
                    foreach (var classPersons in classPersonsDic)
                    {
                        var notAddPersons = classPersons.Value.Where(x => !personids.Contains(x.PersonId)).Select(x=>x.PersonId).ToList();
                        foreach (var notAddPerson in notAddPersons)
                            personids.Add(notAddPerson);
                        
                        decimal price = notAddPersons.Count * app.Price;
                        totalPrice += app.PricePerClass.HasValue && price > app.PricePerClass.Value ? app.PricePerClass.Value : price;
                    }
                    var otherPersons = applicationInstallCount.GroupBy(x=>x.PersonId).Count(x => !personids.Contains(x.Key));
                    totalPrice += app.Price * otherPersons;
                    return totalPrice;
                }
                return totalCount * app.Price;
            }
            return totalPrice;
        }

        private IList<PersonsForApplicationInstall> GetPersonsForApplicationInstall(Application app, int? personId, IList<int> roleIds,
                                                                 IList<int> classIds, IList<Guid> departmentIds, IList<int> gradeLevelIds)
        {
            Trace.Assert(Context.SchoolLocalId.HasValue);
            Trace.Assert(Context.PersonId.HasValue);
            using (var uow = Read())
            {
                var da = new ApplicationInstallDataAccess(uow);
                var sy = new SchoolYearDataAccess(uow).GetByDate(Context.NowSchoolYearTime.Date, Context.SchoolLocalId.Value);
                return da.GetPersonsForApplicationInstall(app.Id, Context.PersonId.Value, personId, roleIds, departmentIds, gradeLevelIds, classIds, Context.Role.Id
                                                   , app.HasAdminMyApps, app.HasTeacherMyApps, app.HasStudentMyApps, app.CanAttach, sy.Id);
            }
        }

        public IList<PersonsForApplicationInstallCount> GetPersonsForApplicationInstallCount(Guid applicationId, int? personId, IList<int> roleIds, IList<int> classIds,
                                                          IList<Guid> departmentIds, IList<int> gradeLevelIds)
        {
            Trace.Assert(Context.SchoolLocalId.HasValue);
            var app = ServiceLocator.ServiceLocatorMaster.ApplicationService.GetApplicationById(applicationId);
            using (var uow = Read())
            {
                var da = new ApplicationInstallDataAccess(uow);
                var sy = new SchoolYearDataAccess(uow).GetByDate(Context.NowSchoolYearTime.Date, Context.SchoolLocalId.Value);
                return da.GetPersonsForApplicationInstallCount(applicationId, Context.PersonId ?? 0, personId, roleIds, departmentIds, gradeLevelIds, classIds, Context.Role.Id
                                                   , app.HasAdminMyApps, app.HasTeacherMyApps, app.HasStudentMyApps, app.CanAttach, sy.Id);
            }
        }


        public IDictionary<Guid, int> GetNotInstalledStudentCountPerApp(int staffId, int classId, int markingPeriodId)
        {
            using (var uow = Read())
            {
                return  new ApplicationInstallDataAccess(uow).GetNotInstalledStudentsCountPerApplication(staffId, classId, markingPeriodId);
            }
        }
    }
}