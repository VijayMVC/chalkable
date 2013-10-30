using System;
using System.Collections.Generic;
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
        IList<Application> ListInstalled(int personId, bool owner);
        IList<ApplicationInstall> ListInstalledAppInstalls(int personId);
        IList<ApplicationInstall> ListInstalledForClass(int classId);
        IList<Application> ListInstalledAppsForClass(int classId);
        IList<ApplicationInstall> ListInstalledByAppId(Guid applicationId);
        ApplicationInstallAction Install(Guid applicationId, int? personId, IList<int> roleIds, IList<int> classIds, IList<Guid> departmentIds, IList<int> gradeLevelIds, int schoolYearId, DateTime dateTime);
        IList<ApplicationInstall> GetInstallations(Guid applicationId, int personId, bool owners = true);
        ApplicationInstall GetInstallationForPerson(Guid applicationId, int personId);
        ApplicationInstall GetInstallationById(int applicationInstallId);
        bool IsPersonForInstall(Guid applicationId);
        void Uninstall(int applicationInstallationId);
        bool CanInstall(Guid applicationId, int? schoolPersonId, IList<int> roleIds, IList<int> classIds, IList<int> gradelevelIds, IList<Guid> departmentIds);

        IList<PersonsForApplicationInstallCount> GetPersonsForApplicationInstallCount(Guid applicationId, int? personId, IList<int> roleIds, IList<int> classIds, IList<Guid> departmentIds, IList<int> gradeLevelIds);
        IList<StudentCountToAppInstallByClass> GetStudentCountToAppInstallByClass(int schoolYearId, Guid applicationId);


        ApplicationTotalPriceInfo GetApplicationTotalPrice(Guid applicationId, int? schoolPerson, IList<int> roleids, IList<int> classids, IList<int> gradelevelids, IList<Guid> departmentids);

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

        public IList<Application> ListInstalled(int personId, bool owner)
        {
            var installed = ListInstalledAppInstalls(personId);
            var all = ServiceLocator.ServiceLocatorMaster.ApplicationService.GetApplications();
            return all.Where(x => installed.Any(y => y.ApplicationRef == x.Id)).ToList();
        }

        public IList<ApplicationInstall> ListInstalledAppInstalls(int personId)
        {
            using (var uow = Read())
            {
                var da = new ApplicationInstallDataAccess(uow);
                return da.GetInstalled(personId);
            }
        }

        public IList<ApplicationInstall> ListInstalledForClass(int classId)
        {
            var clazz = ServiceLocator.ClassService.GetClassById(classId);
            if (!BaseSecurity.IsAdminViewerOrClassTeacher(clazz, Context))
                throw new ChalkableSecurityException();
            using (var uow = Read())
            {
                var da = new ApplicationInstallDataAccess(uow);
                return da.GetInstalledForClass(clazz);
            }
        }

        public IList<Application> ListInstalledAppsForClass(int classId)
        {
            var installed = ListInstalledForClass(classId);
            var all = ServiceLocator.ServiceLocatorMaster.ApplicationService.GetApplications();
            return all.Where(x => installed.Any(y => y.ApplicationRef == x.Id)).ToList();
        }

        public IList<ApplicationInstall> ListInstalledByAppId(Guid applicationId)
        {
            var sy = ServiceLocator.SchoolYearService.GetCurrentSchoolYear();
            using (var uow = Read())
            {
                var da = new ApplicationInstallDataAccess(uow);
                return da.GetInstalledByAppId(applicationId, sy.Id);
            }
        }

        public ApplicationInstallAction Install(Guid applicationId, int? personId, IList<int> roleIds, IList<int> classIds,
                                                IList<Guid> departmentIds, IList<int> gradeLevelIds, int schoolYearId, DateTime dateTime)
        {

            if(!Context.UserLocalId.HasValue)
                throw new UnassignedUserException();

            if (!CanInstall(applicationId, personId, roleIds, classIds, gradeLevelIds, departmentIds))
                throw new ChalkableException(ChlkResources.ERR_APP_NOT_ENOUGH_MONEY);

            var app = ServiceLocator.ServiceLocatorMaster.ApplicationService.GetApplicationById(applicationId);
            using (var uow = Update())
            {
                var da = new ApplicationInstallDataAccess(uow);
                var persons = da.GetPersonsForApplicationInstall(applicationId, Context.UserLocalId.Value, personId, roleIds, departmentIds, gradeLevelIds, classIds, Context.Role.Id
                                                   , app.HasAdminMyApps, app.HasTeacherMyApps, app.HasStudentMyApps, app.CanAttach);
                var spIds = persons.Select(x => x.PersonId).Distinct().ToList();
                var schoolYear = ServiceLocator.SchoolYearService.GetSchoolYearById(schoolYearId);
                var res = RegistarationInstallationAction(uow, app, personId, roleIds, classIds, departmentIds, gradeLevelIds);
                var appInstalls = new List<ApplicationInstall>();

                foreach (var spId in spIds)
                {
                    appInstalls.Add(new ApplicationInstall
                    {
                        ApplicationRef = app.Id,
                        PersonRef = spId,
                        OwnerRef = Context.UserLocalId.Value,
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

        private ApplicationInstallAction RegistarationInstallationAction(UnitOfWork uow, Application app, int? schoolPersonId, IList<int> roleids, IList<int> classids, IList<Guid> departmentids, IList<int> gradelevelids)
        {
            var res = new ApplicationInstallAction
            {
                ApplicationRef = app.Id,
                PersonRef = schoolPersonId,
                Description = string.Empty,
                OwnerRef = Context.UserLocalId.Value
            };
            var ada = new ApplicationInstallActionDataAccess(uow);
            ada.Insert(res);

            var descriptionBuilder = new StringBuilder();
            descriptionBuilder.AppendFormat(APP_INSTALLED_FOR_FMT, app.Name);
            if (Context.Role.Id == CoreRoles.TEACHER_ROLE.Id && classids != null)
            {
                var teacherClasses = new ClassDataAccess(uow, Context.SchoolLocalId)
                    .GetAll(new AndQueryCondition { { Class.TEACHER_REF_FIELD, schoolPersonId } });
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

            if (BaseSecurity.IsAdminViewer(Context))
            {
                if (gradelevelids != null)
                {
                    var gradeLevels = new GradeLevelDataAccess(uow).GetAll()
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
            if (!BaseSecurity.IsAdminViewer(Context) && Context.UserLocalId != personId)
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
            using (var uow = Read())
            {
                var da = new ApplicationInstallDataAccess(uow);
                var ps = new AndQueryCondition
                    {
                        {ApplicationInstall.APPLICATION_REF_FIELD, applicationId},
                        {ApplicationInstall.PERSON_REF_FIELD, personId},
                        {ApplicationInstall.ACTIVE_FIELD, true}
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
            return r.First(x => x.Type == PersonsFroAppInstallTypeEnum.Total).Count > 0;
        }

        public void Uninstall(int applicationInstallationId)
        {
            using (var uow = Update())
            {
                var da = new ApplicationInstallDataAccess(uow);
                var appInst = da.GetById(applicationInstallationId);
                if (!ApplicationSecurity.CanUninstall(Context, appInst))
                    throw new ChalkableSecurityException();
                appInst.Active = false;
                da.Update(appInst);
                uow.Commit();
            }
        }

        public bool CanInstall(Guid applicationId, int? schoolPersonId, IList<int> roleIds, IList<int> classIds, IList<int> gradelevelIds, IList<Guid> departmentIds)
        {
            if (!Context.SchoolId.HasValue)
                throw new UnassignedUserException();
            var priceData = GetApplicationTotalPrice(applicationId, schoolPersonId, roleIds, classIds, gradelevelIds, departmentIds);
            var cnt = priceData.ApplicationInstallCountInfo.First(x => x.Type == PersonsFroAppInstallTypeEnum.Total).Count.Value;
            var bugetBalance = ServiceLocator.ServiceLocatorMaster.FundService.GetUserBalance(Context.UserId);
            return (bugetBalance - priceData.TotalPrice >= 0 || priceData.TotalPrice == 0) && cnt > 0;
        }

        public IList<StudentCountToAppInstallByClass> GetStudentCountToAppInstallByClass(int schoolYearId, Guid applicationId)
        {
            if (!BaseSecurity.IsAdminOrTeacher(Context))
                throw new ChalkableSecurityException();
            var app = ServiceLocator.ServiceLocatorMaster.ApplicationService.GetApplicationById(applicationId);
            var res = new List<StudentCountToAppInstallByClass>();
            if (app.HasStudentMyApps || app.CanAttach)
            {
                using (var uow = Read())
                {
                    var da = new ApplicationInstallDataAccess(uow);
                    res.AddRange(da.GetStudentCountToAppInstallByClass(applicationId, schoolYearId, Context.UserLocalId ?? 0, Context.Role.Id));
                }
            }
            return res;
        }

        public ApplicationTotalPriceInfo GetApplicationTotalPrice(Guid applicationId, int? schoolPerson, IList<int> roleids, IList<int> classids, IList<int> gradelevelids, IList<Guid> departmentids)
        {
            var isForAll = !(schoolPerson.HasValue || (roleids != null && roleids.Count > 0) || (classids != null && classids.Count > 0) ||
                                   (gradelevelids != null && gradelevelids.Count > 0) || (departmentids != null && departmentids.Count > 0));
            var r = GetPersonsForApplicationInstallCount(applicationId, schoolPerson, roleids, classids, departmentids, gradelevelids).ToList();
            var totalPrice = GetApplicationTotalPrice(applicationId, r, isForAll);
            return ApplicationTotalPriceInfo.Create(totalPrice, r);
        }

        private decimal GetApplicationTotalPrice(Guid applicationId, IEnumerable<PersonsForApplicationInstallCount> applicationInstallCount, bool isForAll)
        {
            var app = ServiceLocator.ServiceLocatorMaster.ApplicationService.GetApplicationById(applicationId);
            decimal totalPrice = 0;
            if (app.Price != 0)
            {
                var totalCount = applicationInstallCount.First(x => x.Type == PersonsFroAppInstallTypeEnum.Total).Count.Value;
                if (BaseSecurity.IsAdminViewer(Context))
                {
                    totalPrice = app.Price * totalCount;
                    return totalPrice > app.PricePerSchool && isForAll && app.PricePerSchool.HasValue ? app.PricePerSchool.Value : totalPrice;
                }
                if (Context.Role.Id == CoreRoles.TEACHER_ROLE.Id)
                {
                    var countPerClassComplex = applicationInstallCount.Where(x => x.Type == PersonsFroAppInstallTypeEnum.Class).ToList();
                    var totalInClassCount = countPerClassComplex.Sum(x => x.Count);
                    foreach (var countComplex in countPerClassComplex)
                    {
                        decimal price = countComplex.Count.Value * app.Price;
                        totalPrice += app.PricePerClass.HasValue && price > app.PricePerClass.Value ? app.PricePerClass.Value : price;
                    }
                    totalPrice += app.Price * (totalCount - totalInClassCount.Value);
                    return totalPrice;
                }
                return totalCount * app.Price;
            }
            return totalPrice;
        }

        public IList<PersonsForApplicationInstallCount> GetPersonsForApplicationInstallCount(Guid applicationId, int? personId, IList<int> roleIds, IList<int> classIds,
                                                          IList<Guid> departmentIds, IList<int> gradeLevelIds)
        {
            var app = ServiceLocator.ServiceLocatorMaster.ApplicationService.GetApplicationById(applicationId);
            using (var uow = Read())
            {
                var da = new ApplicationInstallDataAccess(uow);
                return da.GetPersonsForApplicationInstallCount(applicationId, Context.UserLocalId ?? 0, personId, roleIds, departmentIds, gradeLevelIds, classIds, Context.Role.Id
                                                   , app.HasAdminMyApps, app.HasTeacherMyApps, app.HasStudentMyApps, app.CanAttach);
            }
        }
    }
}