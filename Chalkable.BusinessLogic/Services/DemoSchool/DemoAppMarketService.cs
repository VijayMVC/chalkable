using System;
using System.Collections.Generic;
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


        private DemoApplicationInstallStorage ApplicationInstallStorage { get; set; }
        private DemoApplicationInstallActionStorage ApplicationInstallActionStorage { get; set; }
        private DemoApplicationInstallActionClassesStorage ApplicationInstallActionClassesStorage { get; set; }
        
        
        

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
                            GroupId = classId,
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
                            GroupId = classId,
                            PersonId = x.PrimaryTeacherRef.Value,
                            Type = PersonsForAppInstallTypeEnum.Class
                        }));
            }
        }

        public IList<PersonsForApplicationInstallCount> GetPersonsForApplicationInstallCount(Guid applicationId, int userId, int? personId, IList<int> classIds, int id, bool hasAdminMyApps, bool hasTeacherMyApps, bool hasStudentMyApps, bool canAttach, int schoolYearId)
        {
            var personsForAppInstall = GetPersonsForApplicationInstall(applicationId, userId, personId, classIds, id, hasAdminMyApps, hasTeacherMyApps, hasStudentMyApps, canAttach, schoolYearId);

            var res =
                (personsForAppInstall.GroupBy(p => new {p.Type, p.GroupId}).Select(gr => new
                {
                    gr.ToList().Count,
                    gr.Key.GroupId,
                    gr.Key.Type
                })).Union(new[] {new
                {
                    personsForAppInstall.Select(x => x.PersonId).Distinct().ToList().Count,
                    GroupId = (int?)null,
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

        private static void PreparePersonInstalls(int? personId, int callerRoleId, int? callerId, List<PersonsForApplicationInstall> result,
            List<KeyValuePair<int, int>> personsForInstall)
        {
            if (callerRoleId == CoreRoles.TEACHER_ROLE.Id && !personId.HasValue)
            {
                personId = callerId;
            }

            if (personId.HasValue)
            {
                result.AddRange(personsForInstall.Where(x => x.Key == personId).Select(x => new PersonsForApplicationInstall
                {
                    GroupId = x.Key,
                    PersonId = x.Key,
                    Type = PersonsForAppInstallTypeEnum.Person
                }));
            }
        }

        public IList<PersonsForApplicationInstall> GetPersonsForApplicationInstall(Guid applicationId, int value, int? personId, IList<int> classIds, int id, bool hasAdminMyApps, bool hasTeacherMyApps, bool hasStudentMyApps, bool canAttach, int schoolYearId)
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
            PrepareClassInstalls(classIds, personsForInstall, result, callerRoleId);
            PreparePersonInstalls(personId, callerRoleId, callerId, result, personsForInstall);
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

        public ApplicationInstallAction Install(Guid applicationId, int? personId, IList<int> classIds, int schoolYearId, DateTime dateTime)
        {
            if (!Context.PersonId.HasValue)
                throw new UnassignedUserException();

            if (!CanInstall(applicationId, personId, classIds))
                throw new ChalkableException(ChlkResources.ERR_APP_NOT_ENOUGH_MONEY);
            ChargeMoneyForAppInstall(applicationId, personId, classIds);

            var app = ServiceLocator.ServiceLocatorMaster.ApplicationService.GetApplicationById(applicationId);
            var persons = GetPersonsForApplicationInstall(applicationId, Context.PersonId.Value, personId, classIds, Context.Role.Id
                                               , app.HasAdminMyApps, app.HasTeacherMyApps, app.HasStudentMyApps, app.CanAttach, schoolYearId);
            var spIds = persons.Select(x => x.PersonId).Distinct().ToList();
            var schoolYear = ServiceLocator.SchoolYearService.GetSchoolYearById(schoolYearId);
            var res = RegisterApplicationInstallAction(app, personId, classIds);
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

        private ApplicationInstallAction RegisterApplicationInstallAction(Application app, int? personId, IList<int> classids)
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
                var teacherClasses = ServiceLocator.ClassService.GetTeacherClasses(Context.SchoolYearId.Value, personId.Value);
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


        public bool CanInstall(Guid applicationId, int? schoolPersonId, IList<int> classIds)
        {
            if (!Context.PersonId.HasValue)
                throw new UnassignedUserException();
            var priceData = GetApplicationTotalPrice(applicationId, schoolPersonId, classIds);
            var cnt = priceData.TotalCount;//ApplicationInstallCountInfo.First(x => x.Type == PersonsForAppInstallTypeEnum.Total).Count.Value;
            var budgetBalance = ServiceLocator.ServiceLocatorMaster.FundService.GetUserBalance(Context.PersonId.Value);
            return (budgetBalance - priceData.TotalPrice >= 0 || priceData.TotalPrice == 0) && cnt > 0;
        }

        private void ChargeMoneyForAppInstall(Guid applicationId, int? schoolPersonId, IList<int> classIds)
        {
            var priceData = GetApplicationTotalPrice(applicationId, schoolPersonId, classIds);

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

        public ApplicationTotalPriceInfo GetApplicationTotalPrice(Guid applicationId, int? schoolPerson, IList<int> classids)
        {
            var app = ServiceLocator.ServiceLocatorMaster.ApplicationService.GetApplicationById(applicationId);
            var persons = GetPersonsForApplicationInstall(app, schoolPerson, classids);
            var totalPrice = GetApplicationTotalPrice(app, persons);
            var totalCount = persons.GroupBy(x => x.PersonId).Select(x => x.Key).Count();
            return ApplicationTotalPriceInfo.Create(totalPrice, totalCount);
        }
        
        private decimal GetApplicationTotalPrice(Application app, IEnumerable<PersonsForApplicationInstall> applicationInstallCount)
        {

            decimal totalPrice = 0;
            if (app.Price != 0)
            {
                var totalCount = applicationInstallCount.GroupBy(x => x.PersonId).Select(x => x.Key).Count();
                if (BaseSecurity.IsDistrictAdmin(Context))
                {
                    totalPrice = app.Price * totalCount;
                    return totalPrice;
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

        private IList<PersonsForApplicationInstall> GetPersonsForApplicationInstall(Application app, int? personId, IList<int> classIds)
        {
            var sy = ((DemoSchoolYearService)ServiceLocator.SchoolYearService).GetByDate(Context.NowSchoolTime.Date);
            return GetPersonsForApplicationInstall(app.Id, Context.PersonId ?? 0
                                                   , personId, classIds, Context.Role.Id
                                                   , app.HasAdminMyApps, app.HasTeacherMyApps, app.HasStudentMyApps, app.CanAttach, sy.Id);
        }


        public IList<PersonsForApplicationInstallCount> GetPersonsForApplicationInstallCount(Guid applicationId, int? personId, IList<int> classIds)
        {
            var app = ServiceLocator.ServiceLocatorMaster.ApplicationService.GetApplicationById(applicationId);
            var sy = ((DemoSchoolYearService)ServiceLocator.SchoolYearService).GetByDate(Context.NowSchoolTime.Date);
            return GetPersonsForApplicationInstallCount(applicationId, Context.PersonId ?? 0, personId, classIds, Context.Role.Id, app.HasAdminMyApps, app.HasTeacherMyApps, app.HasStudentMyApps, app.CanAttach, sy.Id);
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
        
        public IList<ApplicationInstallHistory> GetApplicationInstallationHistory(Guid applicationId)
        {
            throw new NotImplementedException();
        }
    }
}
