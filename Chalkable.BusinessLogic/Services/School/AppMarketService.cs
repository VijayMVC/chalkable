using System;
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
        //IDictionary<Guid, int> GetNotInstalledStudentCountPerApp(int staffId, int classId, int markingPeriodId);
        //IList<ApplicationInstall> ListInstalledByAppId(Guid applicationId);
        //ApplicationInstallAction Install(Guid applicationId, int? personId, IList<int> classIds, DateTime dateTime);
        //IList<ApplicationInstall> GetInstallations(Guid applicationId, int personId, bool owners = true);
        //ApplicationInstall GetInstallationForPerson(Guid applicationId, int personId);
        //ApplicationInstall GetInstallationById(int applicationInstallId);
        //void Uninstall(int applicationInstallationId);
        //void Uninstall(IList<int> applicationInstallIds);
        //bool CanInstall(Guid applicationId, int? schoolPersonId, IList<int> classIds);

        //IList<PersonsForApplicationInstallCount> GetPersonsForApplicationInstallCount(Guid applicationId, int? personId, IList<int> classIds);
        //IList<StudentCountToAppInstallByClass> GetStudentCountToAppInstallByClass(int schoolYearId, Guid applicationId);

        ApplicationTotalPriceInfo GetApplicationTotalPrice(Guid applicationId, int? schoolPerson, IList<int> classids);
        //IList<ApplicationInstallHistory> GetApplicationInstallationHistory(Guid applicationId);
    }

    public class AppMarketService : SchoolServiceBase, IAppMarketService
    {
        private const string APP_INSTALLED_FOR_FMT = "{0} was installed for : \n";
        private const string APP_CLASS_FMT = "Class {0} \n";

        public AppMarketService(IServiceLocatorSchool serviceLocator) : base(serviceLocator)
        {
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

        public ApplicationInstallAction Install(Guid applicationId, int? personId, IList<int> classIds, DateTime dateTime)
        {
            if (!Context.SCEnabled)
                throw new StudyCenterDisabledException();
            Trace.Assert(Context.SchoolYearId.HasValue);
            Trace.Assert(Context.PersonId.HasValue);
            var app = ServiceLocator.ServiceLocatorMaster.ApplicationService.GetApplicationById(applicationId);
            using (var uow = Update())
            {
                var da = new ApplicationInstallDataAccess(uow);
                var res = RegisterInstallAction(uow, app, personId, classIds,  dateTime);
                var persons = da.GetPersonsForApplicationInstall(app.Id, Context.PersonId.Value, personId, classIds, Context.Role.Id
                        , app.HasAdminMyApps, app.HasTeacherMyApps, app.HasStudentMyApps, app.HasAdminExternalAttach
                        , app.HasStudentExternalAttach, app.HasTeacherExternalAttach, app.CanAttach, Context.SchoolYearId.Value);

                var appInstalls = new List<ApplicationInstall>();
                var spIds = persons.Select(x => new Pair<int,int>(x.PersonId, x.SchoolYearId)).Distinct().ToList();

                foreach (var spId in spIds)
                {
                    appInstalls.Add(new ApplicationInstall
                    {
                        ApplicationRef = app.Id,
                        PersonRef = spId.First,
                        OwnerRef = Context.PersonId.Value,
                        Active = true,
                        SchoolYearRef = spId.Second,
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
        
        private ApplicationInstallAction RegisterInstallAction(UnitOfWork uow, Application app, int? personId, IList<int> classids, DateTime dateTime)
        {
            Trace.Assert(Context.PersonId.HasValue);
            Trace.Assert(Context.SchoolYearId.HasValue);
            var res = new ApplicationInstallAction
            {
                ApplicationRef = app.Id,
                PersonRef = personId,
                Description = string.Empty,
                OwnerRef = Context.PersonId.Value,
                Date = dateTime,
                OwnerRoleId = Context.RoleId,
                Installed = true
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

        public void Uninstall(int applicationInstallationId)
        {
            Uninstall(new List<int> {applicationInstallationId});
        }

        public void Uninstall(IList<int> applicationInstallIds)
        {
            Trace.Assert(Context.PersonId.HasValue);
            using (var uow = Update())
            {
                var da = new ApplicationInstallDataAccess(uow);
                var appInstallsDic = da.GetByIds(applicationInstallIds).GroupBy(x=>x.ApplicationRef).ToDictionary(x=>x.Key, x=>x.ToList());
                var appInstallActionDa = new ApplicationInstallActionDataAccess(uow);
                
                foreach (var applicationId in appInstallsDic.Keys)
                {
                    var appAction = new ApplicationInstallAction
                    {
                        Installed = false,
                        OwnerRef = Context.PersonId.Value,
                        OwnerRoleId = Context.RoleId,
                        ApplicationRef = applicationId,
                        Date = Context.NowSchoolYearTime,
                        Description = $"Uninstalled application {applicationId}"
                    };
                    appInstallActionDa.Insert(appAction);
                    appAction = appInstallActionDa.GetLastAppInstallAction(applicationId, Context.PersonId.Value);
                    foreach (var appInstall in appInstallsDic[applicationId])
                    {
                        if (!ApplicationSecurity.CanUninstall(Context, appInstall))
                            throw new ChalkableSecurityException();
                        appInstall.Active = false;
                        appInstall.AppUninstallActionRef = appAction.Id;
                    }
                    
                }
                da.Update(appInstallsDic.SelectMany(x=>x.Value).ToList());
                uow.Commit();
            }
        }


        public bool CanInstall(Guid applicationId, int? schoolPersonId, IList<int> classIds)
        {
            if (!Context.SchoolLocalId.HasValue)
                throw new UnassignedUserException();
            var priceData = GetApplicationTotalPrice(applicationId, schoolPersonId, classIds);
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
            if (app.HasStudentMyApps || app.CanAttach || app.HasStudentExternalAttach)
            {
                using (var uow = Read())
                {
                    var da = new ApplicationInstallDataAccess(uow);
                    res.AddRange(da.GetStudentCountToAppInstallByClass(applicationId, schoolYearId, Context.PersonId ?? 0, Context.Role.Id));
                }
            }
            return res;
        }

        public ApplicationTotalPriceInfo GetApplicationTotalPrice(Guid applicationId, int? schoolPerson, IList<int> classids)
        {
            //var r = GetPersonsForApplicationInstallCount(applicationId, schoolPerson, roleids, classids, departmentids, gradelevelids).ToList();

            var app = ServiceLocator.ServiceLocatorMaster.ApplicationService.GetApplicationById(applicationId);
            var persons = GetPersonsForApplicationInstall(app, schoolPerson, classids);
            var totalPrice = GetApplicationTotalPrice(app, persons);
            var totalCount = persons.GroupBy(x => x.PersonId).Select(x => x.Key).Count();
            return ApplicationTotalPriceInfo.Create(totalPrice, totalCount);
        }

        public IList<ApplicationInstallHistory> GetApplicationInstallationHistory(Guid applicationId)
        {
            BaseSecurity.EnsureDistrictAdmin(Context);
            return DoRead(u => new ApplicationInstallActionDataAccess(u).GetApplicationInstallationHistory(applicationId));
        }

        private decimal GetApplicationTotalPrice(Application app, IEnumerable<PersonsForApplicationInstall> applicationInstallCount)
        {

            decimal totalPrice = 0;
            if (app.Price != 0)
            {
                var totalCount = applicationInstallCount.GroupBy(x=>x.PersonId).Select(x=>x.Key).Count();
                if (BaseSecurity.IsDistrictAdmin(Context))
                {
                    totalPrice = app.Price * totalCount;
                    return totalPrice;
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

        private IList<PersonsForApplicationInstall> GetPersonsForApplicationInstall(Application app, int? personId, IList<int> classIds)
        {
            Trace.Assert(Context.SchoolLocalId.HasValue);
            Trace.Assert(Context.PersonId.HasValue);
            using (var uow = Read())
            {
                var da = new ApplicationInstallDataAccess(uow);
                var syId = Context.SchoolYearId.Value; //new SchoolYearDataAccess(uow).GetByDate(Context.NowSchoolYearTime.Date, Context.SchoolLocalId.Value);
                return da.GetPersonsForApplicationInstall(app.Id, Context.PersonId.Value, personId, classIds, Context.Role.Id
                                                   , app.HasAdminMyApps, app.HasTeacherMyApps, app.HasStudentMyApps, app.HasAdminExternalAttach
                                                   , app.HasStudentExternalAttach, app.HasTeacherExternalAttach, app.CanAttach, syId);
            }
        }

        public IList<PersonsForApplicationInstallCount> GetPersonsForApplicationInstallCount(Guid applicationId, int? personId, IList<int> classIds)
        {
            Trace.Assert(Context.SchoolLocalId.HasValue);
            Trace.Assert(Context.PersonId.HasValue);

            var app = ServiceLocator.ServiceLocatorMaster.ApplicationService.GetApplicationById(applicationId);
            using (var uow = Read())
            {
                var da = new ApplicationInstallDataAccess(uow);
                var syId = ServiceLocator.SchoolYearService.GetCurrentSchoolYear(); //new SchoolYearDataAccess(uow).GetByDate(Context.NowSchoolYearTime.Date, Context.SchoolLocalId.Value);
                return da.GetPersonsForApplicationInstallCount(applicationId, Context.PersonId.Value, personId, classIds, Context.Role.Id
                                                   , app.HasAdminMyApps, app.HasTeacherMyApps, app.HasStudentMyApps, app.HasAdminExternalAttach
                                                   , app.HasStudentExternalAttach, app.HasTeacherExternalAttach, app.CanAttach, syId.Id);
            }
        }


        public IDictionary<Guid, int> GetNotInstalledStudentCountPerApp(int staffId, int classId, int markingPeriodId)
        {
           return DoRead(u=> new ApplicationInstallDataAccess(u).GetNotInstalledStudentsCountPerApplication(staffId, classId, markingPeriodId));
        }
    }
}