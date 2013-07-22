using System;
using System.Collections.Generic;
using Chalkable.Data.Master.Model;
using Chalkable.Data.School.Model.ApplicationInstall;

namespace Chalkable.BusinessLogic.Services.School
{
    public interface IAppMarketService
    {
        IList<Application> List(string keyword);
        IList<Application> ListInstalled(int schoolPersonId, bool owner);
        IList<ApplicationInstall> ListInstalledAppInstalls(int schoolPersonId);
        IList<ApplicationInstall> ListInstalledForClass(int classId);
        IList<Application> ListInstalledAppsForClass(int classId);
        IList<ApplicationInstall> ListInstalledByAppId(int applicationId, int schoolInfoId);
        ApplicationInstallAction Install(int applicationId, int schoolInfoId, int? schoolPersonId, IList<int> roleIds, IList<int> classIds, IList<int> departmentIds, IList<int> gradeLevelIds, int schoolYearId, DateTime dateTime);
        IList<ApplicationInstall> GetInstallations(int applicationId, int schoolPersonId, bool owners = true);
        ApplicationInstall GetInstallationForPerson(int applicationId, int schoolPersonId);
        ApplicationInstall GetInstallationById(int applicationInstallId);
        bool IsPersonForInstall(int applicationId);
        void Uninstall(int applicationInstallationId);
        bool CanInstall(int applicationId, int? schoolPersonId, IList<int> roleIds, IList<int> classIds, IList<int> gradelevelIds, IList<int> departmentIds);

        /*IList<PersonsForApplicationInstallComplex> GetPersonsForApplicationInstall(int applicationId, int? schoolPersonId, IList<int> roleIds, IList<int> classIds, IList<int> departmentIds, IList<int> gradeLevelIds);
        IList<PersonsForApplicationInstallCountComplex> GetPersonsForApplicationInstallCount(int applicationId, int? schoolPersonId, IList<int> roleIds, IList<int> classIds, IList<int> departmentIds, IList<int> gradeLevelIds);
        IList<StudentCountToAppInstallByClassComplex> GetStudentCountToAppInstallByClass(int schoolYearId, int applicationId);
        ApplicationTotalPriceInfo GetApplicationTotalPrice(int applicationId, int? schoolPerson, IList<int> roleIds, IList<int> classIds, IList<int> gradelevelIds, IList<int> departmentIds);*/

    }

    public class AppMarketService : SchoolServiceBase, IAppMarketService
    {
        public AppMarketService(IServiceLocatorSchool serviceLocator) : base(serviceLocator)
        {
        }

        public IList<Application> List(string keyword)
        {
            throw new NotImplementedException();
        }

        public IList<Application> ListInstalled(int schoolPersonId, bool owner)
        {
            throw new NotImplementedException();
        }

        public IList<ApplicationInstall> ListInstalledAppInstalls(int schoolPersonId)
        {
            throw new NotImplementedException();
        }

        public IList<ApplicationInstall> ListInstalledForClass(int classId)
        {
            throw new NotImplementedException();
        }

        public IList<Application> ListInstalledAppsForClass(int classId)
        {
            throw new NotImplementedException();
        }

        public IList<ApplicationInstall> ListInstalledByAppId(int applicationId, int schoolInfoId)
        {
            throw new NotImplementedException();
        }

        public ApplicationInstallAction Install(int applicationId, int schoolInfoId, int? schoolPersonId, IList<int> roleIds, IList<int> classIds,
                                                IList<int> departmentIds, IList<int> gradeLevelIds, int schoolYearId, DateTime dateTime)
        {
            throw new NotImplementedException();
        }

        public IList<ApplicationInstall> GetInstallations(int applicationId, int schoolPersonId, bool owners = true)
        {
            throw new NotImplementedException();
        }

        public ApplicationInstall GetInstallationForPerson(int applicationId, int schoolPersonId)
        {
            throw new NotImplementedException();
        }

        public ApplicationInstall GetInstallationById(int applicationInstallId)
        {
            throw new NotImplementedException();
        }

        public bool IsPersonForInstall(int applicationId)
        {
            throw new NotImplementedException();
        }

        public void Uninstall(int applicationInstallationId)
        {
            throw new NotImplementedException();
        }

        public bool CanInstall(int applicationId, int? schoolPersonId, IList<int> roleIds, IList<int> classIds, IList<int> gradelevelIds,
                               IList<int> departmentIds)
        {
            throw new NotImplementedException();
        }
    }
}