using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Chalkable.BusinessLogic.Model;
using Chalkable.BusinessLogic.Security;
using Chalkable.BusinessLogic.Services.School;
using Chalkable.Common;
using Chalkable.Common.Exceptions;
using Chalkable.Data.Master.Model;
using Chalkable.Data.School.Model;
using Chalkable.Data.School.Model.ApplicationInstall;

namespace Chalkable.BusinessLogic.Services.DemoSchool
{
    public class DemoApplicationInstallActionClassesStorage : BaseDemoGuidStorage<ApplicationInstallActionClasses>
    {
        public DemoApplicationInstallActionClassesStorage()
            : base(x => x.Id)
        {
        }
    }

    public class DemoApplicationInstallActionStorage : BaseDemoIntStorage<ApplicationInstallAction>
    {
        public DemoApplicationInstallActionStorage()
            : base(x => x.Id, true)
        {
        }

        public ApplicationInstallAction GetLastAppInstallAction(Guid id, int userId)
        {
            return data.OrderByDescending(x => x.Value.Id).First(x => x.Value.ApplicationRef == id && x.Value.OwnerRef == userId).Value;
        }
    }

    public class DemoApplicationInstallActionRoleStorage : BaseDemoIntStorage<ApplicationInstallActionRole>
    {
        public DemoApplicationInstallActionRoleStorage()
            : base(x => x.Id, true)
        {
        }
    }

    public class DemoApplicationInstallActionGradeLevelStorage : BaseDemoIntStorage<ApplicationInstallActionGradeLevel>
    {
        public DemoApplicationInstallActionGradeLevelStorage()
            : base(x => x.Id, true)
        {
        }
    }

    public class DemoApplicationInstallActionDepartmentStorage : BaseDemoIntStorage<ApplicationInstallActionDepartment>
    {
        public DemoApplicationInstallActionDepartmentStorage()
            : base(x => x.Id, true)
        {
        }
    }

    public class DemoApplicationInstallStorage : BaseDemoIntStorage<ApplicationInstall>
    {
        public DemoApplicationInstallStorage()
            : base(x => x.Id, true)
        {
        }

        public IList<ApplicationInstall> GetAll(int personId)
        {
            return data.Where(x => x.Value.PersonRef == personId).Select(x => x.Value).ToList();
        }

        
        public IList<ApplicationInstall> GetInstalledByAppId(Guid applicationId, int schoolYearId)
        {
            return
                data.Where(x => x.Value.ApplicationRef == applicationId && x.Value.SchoolYearRef == schoolYearId)
                    .Select(x => x.Value)
                    .ToList();
        }

        public bool Exists(Guid applicationRef, int personId)
        {
            return data.Count(x => x.Value.ApplicationRef == applicationRef && x.Value.PersonRef == personId) > 0;
        }

        public IList<ApplicationInstall> GetInstalled(int personId)
        {
            return
                data.Where(x => (x.Value.OwnerRef == personId || x.Value.PersonRef == personId) && x.Value.Active)
                    .Select(x => x.Value)
                    .ToList();
        }

        public IList<ApplicationInstall> GetAll(Guid applicationId, bool active)
        {
            return
                data.Where(
                    x => x.Value.ApplicationRef == applicationId && x.Value.Active == active)
                    .Select(x => x.Value)
                    .ToList();
        }

        public IList<ApplicationInstall> GetAll(Guid applicationId, int personId, bool active, bool ownersOnly = false)
        {
            var apps = data.Where(x => x.Value.ApplicationRef == applicationId && x.Value.Active)
                .Select(x => x.Value);
            apps = ownersOnly ? apps.Where(x => x.OwnerRef == personId) : apps.Where(x => x.PersonRef == personId);
            return apps.ToList();

        }
    }

    public class DemoAppMarketService : DemoSchoolServiceBase, IAppMarketService
    {
        private const string APP_INSTALLED_FOR_FMT = "{0} was installed for : \n";
        private const string APP_CLASS_FMT = "Class {0} \n";
        private const string APP_GRADE_LEVEL_FMT = "Grade Level {0} \n";
        private const string APP_DEPARTMENT_FMT = "Department {0} \n";
        private const string APP_ROLE_FMT = "Role {0} \n";


        private DemoApplicationInstallStorage ApplicationInstallStorage { get; set; }
        private DemoApplicationInstallActionStorage ApplicationInstallActionStorage { get; set; }
        private DemoApplicationInstallActionClassesStorage ApplicationInstallActionClassesStorage { get; set; }
        private DemoApplicationInstallActionGradeLevelStorage ApplicationInstallActionGradeLevelStorage { get; set; }
        public DemoApplicationInstallActionRoleStorage ApplicationInstallActionRoleStorage { get; set; }
        public DemoApplicationInstallActionDepartmentStorage ApplicationInstallActionDepartmentStorage { get; set; }

        public IEnumerable<StudentCountToAppInstallByClass> GetStudentCountToAppInstallByClass(Guid applicationId, int schoolYearId, int userId, int roleId)
        {

            var classes = ServiceLocator.ClassService.GetTeacherClasses(schoolYearId, userId);
            var csps = ((DemoClassService) ServiceLocator.ClassService).GetByClasses(classes);
            
            var appInstalls = ApplicationInstallStorage.GetAll()
                .Where(x => x.ApplicationRef == applicationId && x.Active && x.SchoolYearRef == schoolYearId).ToList();

            return classes.Select(c => new StudentCountToAppInstallByClass
            {
                ClassId = c.Id,
                ClassName = c.Name,
                NotInstalledStudentCount = csps.Count(cp => cp.ClassRef == c.Id && 
                    (appInstalls.Count > 0 || appInstalls.All(install => install.PersonRef != cp.PersonRef)))
            }).ToList();
        }

        private void PrepareDepartmentInstalls(ICollection<Guid> departmentIds, IEnumerable<KeyValuePair<int, int>> personsForInstall, List<PersonsForApplicationInstall> result)
        {
            if (departmentIds == null) return;
            var personRefs = personsForInstall.Select(x => x.Key).ToList();
            var mpId = ServiceLocator.MarkingPeriodService.GetMarkingPeriodByDate(Context.NowSchoolTime).Id;
            var filtered =
                ((DemoClassService)ServiceLocator.ClassService).GetClassPersons().Where(x =>
                {
                    var cls = ServiceLocator.ClassService.GetById(x.ClassRef);
                    return personRefs.Contains(x.PersonRef) && cls.ChalkableDepartmentRef != null &&
                           departmentIds.Contains(cls.ChalkableDepartmentRef.Value) && x.MarkingPeriodRef == mpId;
                }).Select(x =>
                {
                    var chalkableDepartmentRef = ServiceLocator.ClassService.GetById(x.ClassRef).ChalkableDepartmentRef;
                    return chalkableDepartmentRef != null
                        ? new
                        {
                            x.PersonRef,
                            DepartmentName = ServiceLocator.ServiceLocatorMaster.ChalkableDepartmentService
                            .GetChalkableDepartmentById(chalkableDepartmentRef.Value).Name
                        }
                        : new { x.PersonRef, DepartmentName = "" };
                });

            result.AddRange(filtered.Select(x => new PersonsForApplicationInstall
            {
                GroupId = x.DepartmentName,
                PersonId = x.PersonRef,
                Type = PersonsForAppInstallTypeEnum.Department
            }));
        }

        private void PrepareClassInstalls(IList<int> classIds, IEnumerable<KeyValuePair<int, int>> personsForInstall,
            List<PersonsForApplicationInstall> result, int callerRoleId)
        {
            if (classIds == null) return;
            var ids = personsForInstall.Select(x => x.Key).ToList();
            var mpId = ServiceLocator.MarkingPeriodService.GetMarkingPeriodByDate(Context.NowSchoolTime).Id;
            foreach (var classId in classIds)
            {
                result.AddRange(
                    ((DemoClassService)ServiceLocator.ClassService).GetClassPersons()
                        .Where(x => x.ClassRef == classId && ids.Contains(x.PersonRef) && x.MarkingPeriodRef == mpId)
                        .Distinct()
                        .Select(x => new PersonsForApplicationInstall
                        {
                            GroupId = classId.ToString(CultureInfo.InvariantCulture),
                            PersonId = x.PersonRef,
                            Type = PersonsForAppInstallTypeEnum.Class
                        }));
            }

            if (callerRoleId == CoreRoles.TEACHER_ROLE.Id) return;
            foreach (var classId in classIds)
            {
                result.AddRange(
                    ServiceLocator.ClassService.GetAll()
                        .Where(cls => cls.Id == classId && cls.PrimaryTeacherRef != null && ids.Contains(cls.PrimaryTeacherRef.Value))
                        .Select(x => new PersonsForApplicationInstall
                        {
                            GroupId = classId.ToString(CultureInfo.InvariantCulture),
                            PersonId = x.PrimaryTeacherRef.Value,
                            Type = PersonsForAppInstallTypeEnum.Class
                        }));
            }
        }

        public IList<PersonsForApplicationInstallCount> GetPersonsForApplicationInstallCount(Guid applicationId, int userId, int? personId, IList<int> roleIds, IList<Guid> departmentIds, IList<int> gradeLevelIds, IList<int> classIds, int id, bool hasAdminMyApps, bool hasTeacherMyApps, bool hasStudentMyApps, bool canAttach, int schoolYearId)
        {
            var personsForAppInstall = GetPersonsForApplicationInstall(applicationId, userId, personId, roleIds, departmentIds,
                gradeLevelIds, classIds, id, hasAdminMyApps, hasTeacherMyApps, hasStudentMyApps, canAttach, schoolYearId);

            var res =
                (personsForAppInstall.GroupBy(p => new {p.Type, p.GroupId}).Select(gr => new
                {
                    gr.ToList().Count,
                    gr.Key.GroupId,
                    gr.Key.Type
                })).Union(new[] {new
                {
                    personsForAppInstall.Select(x => x.PersonId).Distinct().ToList().Count,
                    GroupId = "",
                    Type = PersonsForAppInstallTypeEnum.Total
                }});

            return res.Select(x => new PersonsForApplicationInstallCount
            {
                Type = x.Type,
                Count = x.Count,
                GroupId = x.GroupId
            }).ToList();
        }
        public DemoAppMarketService(IServiceLocatorSchool serviceLocator)
            : base(serviceLocator)
        {
            ApplicationInstallStorage = new DemoApplicationInstallStorage();
            ApplicationInstallActionStorage = new DemoApplicationInstallActionStorage();
            ApplicationInstallActionClassesStorage = new DemoApplicationInstallActionClassesStorage();
            ApplicationInstallActionGradeLevelStorage = new DemoApplicationInstallActionGradeLevelStorage();
            ApplicationInstallActionRoleStorage = new DemoApplicationInstallActionRoleStorage();
            ApplicationInstallActionDepartmentStorage = new DemoApplicationInstallActionDepartmentStorage();
        }

        private static bool CanInstall(bool hasAdminMyApps, bool hasStudentMyApps, int callerRoleId, bool canInstallForStudent,
            bool canInstallForTeacher)
        {
            var canInstall = callerRoleId == CoreRoles.STUDENT_ROLE.Id && hasStudentMyApps ||
                             (callerRoleId == CoreRoles.TEACHER_ROLE.Id &&
                              (canInstallForStudent || canInstallForTeacher))
                             ||
                             ((callerRoleId == CoreRoles.DISTRICT_ADMIN_ROLE.Id)
                              && (hasAdminMyApps || canInstallForStudent || canInstallForTeacher));
            return canInstall;
        }

        private static void PrepareRoleInstalls(ICollection<int> roleIds, List<PersonsForApplicationInstall> result, IEnumerable<KeyValuePair<int, int>> personsForInstall)
        {
            if (roleIds == null) return;
            result.AddRange(
                personsForInstall.Where(x => roleIds.Contains(x.Value)).Select(x => new PersonsForApplicationInstall
                {
                    GroupId = x.Key.ToString(CultureInfo.InvariantCulture),
                    PersonId = x.Key,
                    Type = PersonsForAppInstallTypeEnum.Role
                }));
        }

        private static void PreparePersonInstalls(int? personId, IList<int> roleIds, IList<Guid> departmentIds, IList<int> gradeLevelIds,
            IList<int> classIds, int callerRoleId, int? callerId, List<PersonsForApplicationInstall> result,
            List<KeyValuePair<int, int>> personsForInstall)
        {
            var isSinglePerson = false;

            if (callerRoleId == CoreRoles.TEACHER_ROLE.Id && !personId.HasValue)
            {
                personId = callerId;
                isSinglePerson = true;
            }

            if (personId.HasValue)
            {
                result.AddRange(personsForInstall.Where(x => x.Key == personId).Select(x => new PersonsForApplicationInstall
                {
                    GroupId = x.Key.ToString(CultureInfo.InvariantCulture),
                    PersonId = x.Key,
                    Type = PersonsForAppInstallTypeEnum.Person
                }));
            }

            if (roleIds == null && departmentIds == null && gradeLevelIds == null && classIds == null &&
                (isSinglePerson || !personId.HasValue))
            {
                result.AddRange(personsForInstall.Select(x => new PersonsForApplicationInstall
                {
                    GroupId = x.Key.ToString(CultureInfo.InvariantCulture),
                    PersonId = x.Key,
                    Type = PersonsForAppInstallTypeEnum.Person
                }));
            }
        }

        public IList<PersonsForApplicationInstall> GetPersonsForApplicationInstall(Guid applicationId, int value, int? personId, IList<int> roleIds, IList<Guid> departmentIds, IList<int> gradeLevelIds, IList<int> classIds, int id, bool hasAdminMyApps, bool hasTeacherMyApps, bool hasStudentMyApps, bool canAttach, int schoolYearId)
        {

            var callerRoleId = Context.RoleId;
            var callerId = Context.PersonId;

            var canInstallForTeacher = hasTeacherMyApps || canAttach;
            var canInstallForStudent = hasStudentMyApps || canAttach;

            var canInstall = CanInstall(hasAdminMyApps, hasStudentMyApps, callerRoleId, canInstallForStudent, canInstallForTeacher);

            var schoolId = ServiceLocator.SchoolYearService.GetSchoolYearById(schoolYearId).SchoolRef;

            var personsForInstall = new List<KeyValuePair<int, int>>();

            if (canInstall)
            {
                if (callerRoleId == CoreRoles.STUDENT_ROLE.Id)
                {
                    var sp = ServiceLocator.SchoolPersonService.GetAll()
                            .First(x => x.PersonRef == callerId && x.SchoolRef == schoolId && hasStudentMyApps);
                    personsForInstall.Add(new KeyValuePair<int, int>(sp.PersonRef, sp.RoleRef));
                }

                if (callerRoleId == CoreRoles.TEACHER_ROLE.Id)
                {
                    
                    var classes = ServiceLocator.ClassService.GetTeacherClasses(schoolYearId, callerId.Value);
                    var personRefs = ((DemoClassService)ServiceLocator.ClassService).GetClassPersons(classes).Select(x => x.PersonRef);
                    var sps =
                        ServiceLocator.SchoolPersonService.GetAll()
                            .Where(x => (personRefs.Contains(x.PersonRef) && canInstallForStudent || x.PersonRef == callerId && canInstallForTeacher)
                                && x.SchoolRef == schoolId);
                    personsForInstall.AddRange(sps.Select(schoolPerson => new KeyValuePair<int, int>(schoolPerson.PersonRef, schoolPerson.RoleRef)));
                }
            }

            var installed =
                ApplicationInstallStorage.GetAll()
                    .Where(x => x.Active && x.ApplicationRef == applicationId).Select(x => x.PersonRef)
                    .ToList();

            personsForInstall = personsForInstall.Where(x => !installed.Contains(x.Key)).ToList();

            var result = new List<PersonsForApplicationInstall>();
            PrepareRoleInstalls(roleIds, result, personsForInstall);
            PrepareDepartmentInstalls(departmentIds, personsForInstall, result);
            PrepareClassInstalls(classIds, personsForInstall, result, callerRoleId);
            PreparePersonInstalls(personId, roleIds, departmentIds, gradeLevelIds, classIds, callerRoleId, callerId, result, personsForInstall);
            if (gradeLevelIds != null && gradeLevelIds.Count > 0)
                throw new NotImplementedException();
            return result;
        }

        public IList<ApplicationInstall> GetInstalledForClass(ClassDetails clazz)
        {
            var persons = ((DemoPersonService) ServiceLocator.PersonService).GetByClassId(clazz.Id).Select(x => x.Id);
            return ApplicationInstallStorage.GetData().Where(x => persons.Contains(x.Value.PersonRef)).Select(x => x.Value).ToList();
        }

        public IList<ApplicationInstall> ListInstalledAppInstalls(int personId)
        {
            return ApplicationInstallStorage.GetInstalled(personId);
        }

        public IList<ApplicationInstall> ListInstalledByAppId(Guid applicationId)
        {
            var sy = ServiceLocator.SchoolYearService.GetCurrentSchoolYear();
            return ApplicationInstallStorage.GetInstalledByAppId(applicationId, sy.Id);
        }

        public ApplicationInstallAction Install(Guid applicationId, int? personId, IList<int> roleIds, IList<int> classIds,
                                                IList<Guid> departmentIds, IList<int> gradeLevelIds, int schoolYearId, DateTime dateTime)
        {
            if (!Context.PersonId.HasValue)
                throw new UnassignedUserException();

            if (!CanInstall(applicationId, personId, roleIds, classIds, gradeLevelIds, departmentIds))
                throw new ChalkableException(ChlkResources.ERR_APP_NOT_ENOUGH_MONEY);
            ChargeMoneyForAppInstall(applicationId, personId, roleIds, classIds, gradeLevelIds, departmentIds);

            var app = ServiceLocator.ServiceLocatorMaster.ApplicationService.GetApplicationById(applicationId);
            var persons = GetPersonsForApplicationInstall(applicationId, Context.PersonId.Value, personId
                                                , roleIds, departmentIds, gradeLevelIds, classIds, Context.Role.Id
                                               , app.HasAdminMyApps, app.HasTeacherMyApps, app.HasStudentMyApps, app.CanAttach, schoolYearId);
            var spIds = persons.Select(x => x.PersonId).Distinct().ToList();
            var schoolYear = ServiceLocator.SchoolYearService.GetSchoolYearById(schoolYearId);
            var res = RegisterApplicationInstallAction(app, personId, roleIds, classIds, departmentIds, gradeLevelIds);
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
            ApplicationInstallStorage.Add(appInstalls);

            

            res.ApplicationInstalls = appInstalls;
            return res;
        }

        private ApplicationInstallAction RegisterApplicationInstallAction(Application app, int? personId, IList<int> roleids, IList<int> classids, IList<Guid> departmentids, IList<int> gradelevelids)
        {
            
            var res = new ApplicationInstallAction
            {
                ApplicationRef = app.Id,
                PersonRef = personId,
                Description = String.Empty,
                OwnerRef = Context.PersonId.Value
            };

            ApplicationInstallActionStorage.Add(res);
            res = ApplicationInstallActionStorage.GetLastAppInstallAction(app.Id, Context.PersonId.Value);
            var descriptionBuilder = new StringBuilder();
            descriptionBuilder.AppendFormat(APP_INSTALLED_FOR_FMT, app.Name);
            if (Context.Role.Id == CoreRoles.TEACHER_ROLE.Id && classids != null)
            {
                var teacherClasses = ServiceLocator.ClassService.GetTeacherClasses(Context.SchoolYearId.Value, personId.Value, null);
                teacherClasses = teacherClasses.Where(x => classids.Contains(x.Id)).ToList();

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
                ApplicationInstallActionClassesStorage.Add(appInstallAcClasses);
            }

            if (BaseSecurity.IsDistrictAdmin(Context))
            {
                if (gradelevelids != null)
                {
                    var gradeLevels = ServiceLocator.GradeLevelService.GetGradeLevels()
                        .Where(x => app.GradeLevels.Any(y => y.GradeLevel == x.Number))
                        .Where(x => gradelevelids.Contains(x.Id));
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
                    ApplicationInstallActionGradeLevelStorage.Add(aiaGls);
                }
                if (departmentids != null)
                {
                    var departments = ServiceLocator.ServiceLocatorMaster.ChalkableDepartmentService.GetChalkableDepartments();
                    departments = departments.Where(x => departmentids.Contains(x.Id)).ToList();

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
                    ApplicationInstallActionDepartmentStorage.Add(aiaDepartments);
                }
                if (roleids != null)
                {
                    var roles = roleids.Select(CoreRoles.GetById).ToList();

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
                    ApplicationInstallActionRoleStorage.Add(appInstallActionRoles);
                }
            }
            res.Description = descriptionBuilder.ToString();
            ApplicationInstallActionStorage.Update(res);
            return res;
        }




        public IList<ApplicationInstall> GetInstallations(Guid applicationId, int personId, bool owners = true)
        {
            if (!BaseSecurity.IsDistrictAdmin(Context) && Context.PersonId != personId)
                throw new ChalkableSecurityException();

            return ApplicationInstallStorage.GetAll(applicationId, personId, true, owners);
        }

        public ApplicationInstall GetInstallationForPerson(Guid applicationId, int personId)
        {
            return ApplicationInstallStorage.GetAll(applicationId, personId, true).FirstOrDefault();
        }

        public ApplicationInstall GetInstallationById(int applicationInstallId)
        {
            return ApplicationInstallStorage.GetById(applicationInstallId);
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
            foreach (var applicationInstallId in applicationInstallIds)
            {
                var appInst = ApplicationInstallStorage.GetById(applicationInstallId);
                if (!ApplicationSecurity.CanUninstall(Context, appInst))
                    throw new ChalkableSecurityException();
                appInst.Active = false;
                ApplicationInstallStorage.Update(appInst);
            }
        }


        public bool CanInstall(Guid applicationId, int? schoolPersonId, IList<int> roleIds, IList<int> classIds, IList<int> gradelevelIds, IList<Guid> departmentIds)
        {
            if (!Context.PersonId.HasValue)
                throw new UnassignedUserException();
            var priceData = GetApplicationTotalPrice(applicationId, schoolPersonId, roleIds, classIds, gradelevelIds, departmentIds);
            var cnt = priceData.TotalCount;//ApplicationInstallCountInfo.First(x => x.Type == PersonsForAppInstallTypeEnum.Total).Count.Value;
            var budgetBalance = ServiceLocator.ServiceLocatorMaster.FundService.GetUserBalance(Context.PersonId.Value);
            return (budgetBalance - priceData.TotalPrice >= 0 || priceData.TotalPrice == 0) && cnt > 0;
        }

        private void ChargeMoneyForAppInstall(Guid applicationId, int? schoolPersonId, IList<int> roleIds, IList<int> classIds, IList<int> gradelevelIds, IList<Guid> departmentIds)
        {
            var priceData = GetApplicationTotalPrice(applicationId, schoolPersonId, roleIds, classIds, gradelevelIds, departmentIds);

            if (Context.PersonId.HasValue)
            {
                var budgetBalance = ServiceLocator.ServiceLocatorMaster.FundService.GetUserBalance(Context.PersonId.Value);
                var newBalance =  budgetBalance - priceData.TotalPrice;
                ServiceLocator.ServiceLocatorMaster.FundService.UpdateUserBalance(Context.PersonId.Value, newBalance);
            }
        }

        public IList<StudentCountToAppInstallByClass> GetStudentCountToAppInstallByClass(int schoolYearId, Guid applicationId)
        {
            if (!BaseSecurity.IsDistrictOrTeacher(Context))
                throw new ChalkableSecurityException();
            var app = ServiceLocator.ServiceLocatorMaster.ApplicationService.GetApplicationById(applicationId);
            var res = new List<StudentCountToAppInstallByClass>();
            if (app.HasStudentMyApps || app.CanAttach)
            {
                res.AddRange(GetStudentCountToAppInstallByClass(applicationId, schoolYearId, Context.PersonId ?? 0, Context.Role.Id));
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

        private decimal GetApplicationTotalPrice(Application app, IEnumerable<PersonsForApplicationInstall> applicationInstallCount, bool isForAll)
        {

            decimal totalPrice = 0;
            if (app.Price != 0)
            {
                var totalCount = applicationInstallCount.GroupBy(x => x.PersonId).Select(x => x.Key).Count();
                if (BaseSecurity.IsDistrictAdmin(Context))
                {
                    totalPrice = app.Price * totalCount;
                    return totalPrice > app.PricePerSchool && isForAll && app.PricePerSchool.HasValue ? app.PricePerSchool.Value : totalPrice;
                }
                if (Context.Role.Id == CoreRoles.TEACHER_ROLE.Id)
                {
                    var personstPerClassComplex = applicationInstallCount.Where(x => x.Type == PersonsForAppInstallTypeEnum.Class).ToList();
                    //var totalInClassCount = countPerClassComplex.Sum(x => x.Count);
                    var personids = new HashSet<int>();
                    var classPersonsDic = personstPerClassComplex.GroupBy(x => x.GroupId).ToDictionary(x => x.Key, x => x.ToList());
                    foreach (var classPersons in classPersonsDic)
                    {
                        var notAddPersons = classPersons.Value.Where(x => !personids.Contains(x.PersonId)).Select(x => x.PersonId).ToList();
                        foreach (var notAddPerson in notAddPersons)
                            personids.Add(notAddPerson);

                        decimal price = notAddPersons.Count * app.Price;
                        totalPrice += app.PricePerClass.HasValue && price > app.PricePerClass.Value ? app.PricePerClass.Value : price;
                    }
                    var otherPersons = applicationInstallCount.Where(x => x.Type == PersonsForAppInstallTypeEnum.Person)
                                                              .GroupBy(x => x.PersonId).Count();
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
            var sy = ((DemoSchoolYearService)ServiceLocator.SchoolYearService).GetByDate(Context.NowSchoolTime.Date);
            return GetPersonsForApplicationInstall(app.Id, Context.PersonId ?? 0
                                                   , personId, roleIds, departmentIds, gradeLevelIds, classIds, Context.Role.Id
                                                   , app.HasAdminMyApps, app.HasTeacherMyApps, app.HasStudentMyApps, app.CanAttach, sy.Id);
        }


        public IList<PersonsForApplicationInstallCount> GetPersonsForApplicationInstallCount(Guid applicationId, int? personId, IList<int> roleIds, IList<int> classIds,
                                                          IList<Guid> departmentIds, IList<int> gradeLevelIds)
        {
            var app = ServiceLocator.ServiceLocatorMaster.ApplicationService.GetApplicationById(applicationId);

            var sy = ((DemoSchoolYearService)ServiceLocator.SchoolYearService).GetByDate(Context.NowSchoolTime.Date);
            return GetPersonsForApplicationInstallCount(applicationId, Context.PersonId ?? 0, personId, roleIds, departmentIds, gradeLevelIds, classIds, Context.Role.Id
                                                   , app.HasAdminMyApps, app.HasTeacherMyApps, app.HasStudentMyApps, app.CanAttach, sy.Id);
        }

        public IDictionary<Guid, int> GetNotInstalledStudentCountPerApp(int staffId, int classId, int markingPeriodId)
        {
          
            var mp = ServiceLocator.MarkingPeriodService.GetMarkingPeriodById(markingPeriodId);
            var cps = ServiceLocator.ClassService.GetClassPersons(null, classId, true, markingPeriodId)
                .GroupBy(x => x.PersonRef, (key, group) => new {PersonId = key, Count = group.Count()});

            var personIds = cps.Select(x => x.PersonId);

            var appInstalls = ApplicationInstallStorage.GetAll().Where(
                    x => x.Active && x.OwnerRef == staffId
                    && x.SchoolYearRef == mp.SchoolYearRef &&
                    personIds.Contains(x.PersonRef));

            var classStudentsCount = personIds.Count();

            return appInstalls.GroupBy(x => x.ApplicationRef,
                (key, val) => new {ApplicationId = key, NotInstalledCount = classStudentsCount - val.Count()})
                .ToDictionary(x => x.ApplicationId, x => x.NotInstalledCount);
        }

        public bool AppInstallExists(Guid applicationRef, int personId)
        {
            return ApplicationInstallStorage.Exists(applicationRef, personId);
        }

        public IList<ApplicationInstall> GetAppInstalls(Guid appId, bool active)
        {
            return ApplicationInstallStorage.GetAll(appId, active);
        }
    }
}
