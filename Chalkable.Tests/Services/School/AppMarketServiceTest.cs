using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chalkable.BusinessLogic.Model;
using Chalkable.BusinessLogic.Services.Master;
using Chalkable.Common;
using Chalkable.Data.Master.Model;
using Chalkable.Data.School.Model;
using Chalkable.Tests.Services.Master;
using Chalkable.Tests.Services.TestContext;
using NUnit.Framework;

namespace Chalkable.Tests.Services.School
{
    public class AppMarketServiceTest : BaseSchoolServiceTest
    {
        [Test]
        public void InstallTest()
        {
            var devContext = CreateDeveloperSchoolTestContext();
            var appInfo = ApplicationServiceTest.PrepareDefaultAppInfo(SysAdminMasterLocator, devContext.Developer.Id, "app1", "http://test.app1.com");
            appInfo.ApplicationPrices = ApplicationPricesInfo.Create(0, 0, 0);
            appInfo.ApplicationAccessInfo = ApplicationAccessInfo.Create(false, false, false, false, false, false);
            var draftApp = ApplicationServiceTest.CreateLiveApp(SysAdminMasterLocator, devContext, appInfo);
            var liveApp = SchoolTestContext.AdminGradeSl.ServiceLocatorMaster.ApplicationService.GetApplicationById(draftApp.OriginalRef.Value);

            var mp = MarkingPeriodServiceTest.CreateSchoolYearWithMp(SchoolTestContext, null);
            var c = ClassServiceTest.CreateClass(SchoolTestContext, SchoolTestContext.FirstTeacher, SchoolTestContext.FirstStudent, null);

            //security 
            AssertException<Exception>(() => SchoolTestContext.FirstTeacherSl.AppMarketService
                .Install(liveApp.Id, null, null, null, null, null, mp.Id, SchoolTestContext.NowTime));
            AssertException<Exception>(() => SchoolTestContext.SecondTeacherSl.AppMarketService
                .Install(liveApp.Id, null, null, null, null, null, mp.Id, SchoolTestContext.NowTime));
            AssertException<Exception>(() => SchoolTestContext.FirstStudentSl.AppMarketService
                            .Install(liveApp.Id, null, null, null, null, null, mp.Id, SchoolTestContext.NowTime));
            AssertException<Exception>(() => SchoolTestContext.FirstTeacherSl.AppMarketService
                .Install(liveApp.Id, null, null, null, null, null, mp.Id, SchoolTestContext.NowTime));

            appInfo.ApplicationAccessInfo = ApplicationAccessInfo.Create(true, true, false, false, true, false);
            draftApp = ApplicationServiceTest.UpdateApp(SysAdminMasterLocator, devContext, draftApp.Id, appInfo);
            liveApp = SchoolTestContext.AdminGradeSl.ServiceLocatorMaster.ApplicationService.GetApplicationById(draftApp.OriginalRef.Value);
            Assert.IsFalse(SchoolTestContext.FirstTeacherSl.AppMarketService.CanInstall(liveApp.Id, SchoolTestContext.SecondTeahcer.Id, null, null, null,null));
            AssertException<Exception>(() => SchoolTestContext.FirstTeacherSl.AppMarketService
                .Install(liveApp.Id, SchoolTestContext.SecondTeahcer.Id, null, null, null, null, mp.Id, SchoolTestContext.NowTime));
            Assert.IsFalse(SchoolTestContext.FirstTeacherSl.AppMarketService.CanInstall(liveApp.Id, SchoolTestContext.AdminGrade.Id, null, null, null, null));
            AssertException<Exception>(() => SchoolTestContext.FirstTeacherSl.AppMarketService
                .Install(liveApp.Id, SchoolTestContext.AdminGrade.Id, null, null, null, null, mp.Id, SchoolTestContext.NowTime));
            Assert.IsFalse(SchoolTestContext.FirstTeacherSl.AppMarketService.CanInstall(liveApp.Id, SchoolTestContext.SecondStudent.Id, null, null, null, null));
            AssertException<Exception>(() => SchoolTestContext.FirstTeacherSl.AppMarketService
                .Install(liveApp.Id, SchoolTestContext.SecondStudent.Id, null, null, null, null, mp.Id, SchoolTestContext.NowTime));


            appInfo.ApplicationPrices = ApplicationPricesInfo.Create(2, 10, 100);
            draftApp = ApplicationServiceTest.UpdateApp(SysAdminMasterLocator, devContext, draftApp.Id, appInfo);
            liveApp = SchoolTestContext.AdminGradeSl.ServiceLocatorMaster.ApplicationService.GetApplicationById(draftApp.OriginalRef.Value);
            Assert.IsFalse(SchoolTestContext.FirstTeacherSl.AppMarketService.CanInstall(liveApp.Id, null, null, null, null, null));
            AssertException<Exception>(() => SchoolTestContext.FirstTeacherSl.AppMarketService
                             .Install(liveApp.Id, null, null, null, null, null, mp.SchoolYearRef, SchoolTestContext.NowTime));

            var fr = SchoolTestContext.AdminGradeSl.ServiceLocatorMaster.FundService.RequestByPurchaseOrder(SchoolTestContext.School.Id, SchoolTestContext.FirstTeacher.Id, 300, 100, 100,
                                                                     100, 0, "test", new byte[] {1, 2, 3});
            SysAdminMasterLocator.FundService.ApproveReject(fr.Id, true);
            var marketS = SchoolTestContext.FirstTeacherSl.AppMarketService;
            Assert.IsTrue(marketS.CanInstall(liveApp.Id, null, null, null, null, null));
            Assert.IsTrue(marketS.CanInstall(liveApp.Id, null, null, new List<Guid> { c.Id }, null, null));
            var appTotalPrice = SchoolTestContext.FirstTeacherSl.AppMarketService.GetApplicationTotalPrice(liveApp.Id, null, null, new List<Guid> { c.Id }, null, null);
            Assert.AreEqual(appTotalPrice.TotalPrice, 4);
            Assert.AreEqual(appTotalPrice.ApplicationInstallCountInfo.First(x => x.Type == PersonsFroAppInstallTypeEnum.Total).Count, 2);
            Assert.AreEqual(marketS.GetApplicationTotalPrice(liveApp.Id, SchoolTestContext.FirstStudent.Id, null, null, null, null).TotalPrice, 2);

            var aia = SchoolTestContext.FirstTeacherSl.AppMarketService.Install(liveApp.Id, null, null, new List<Guid> {c.Id}, null, null, mp.SchoolYearRef, SchoolTestContext.NowTime);

            var appInstalls = marketS.GetInstallations(liveApp.Id, SchoolTestContext.FirstTeacher.Id);
            Assert.AreEqual(appInstalls.Count, 2);


            //var roles = new List<int> {CoreRoles.ADMIN_GRADE_ROLE.Id};
            //Assert.IsFalse(SchoolTestContext.FirstTeacherSl.AppMarketService.CanInstall(app.Id, null, roles, null, null, null));
            //AssertException<Exception>(() => SchoolTestContext.FirstTeacherSl.AppMarketService
            //    .Install(app.Id, null, roles, null, null, null, mp.Id, SchoolTestContext.NowTime));

            //Assert.IsFalse(SchoolTestContext.FirstTeacherSl.AppMarketService.CanInstall(app.Id, null, null, null, new List<Guid>{c.GradeLevelRef}, null));
            //AssertException<Exception>(() => SchoolTestContext.FirstTeacherSl.AppMarketService
            //    .Install(app.Id, null, null, null, null, new List<Guid> { c.GradeLevelRef }, mp.Id, SchoolTestContext.NowTime));


            //Assert.IsFalse(SchoolTestContext.SecondTeacherSl.AppMarketService.CanInstall(app.Id, null, null, new List<Guid> { c.Id }, null, null));
            //AssertException<Exception>(() => SchoolTestContext.SecondTeacherSl.AppMarketService
            //    .Install(app.Id, null, null, new List<Guid> { c.Id }, null, null, mp.Id, SchoolTestContext.NowTime));
            

            //TODO: implementation
        }



        [Test]
        public void GetApplicationTotalPrice()
        {
            var devContext = CreateDeveloperSchoolTestContext();
            var baseData = PrepareBaseDataForInstallTest(SysAdminMasterLocator, devContext, SchoolTestContext);

            Assert.AreEqual(0, SchoolTestContext.FirstTeacherSl.AppMarketService.GetApplicationTotalPrice(baseData.LiveApp.Id, devContext.FirstTeacher.Id, null, null, null, null).TotalPrice);
            Assert.AreEqual(0, SchoolTestContext.FirstTeacherSl.AppMarketService.GetApplicationTotalPrice(baseData.LiveApp.Id, null, null, new List<Guid> { baseData.MathClass.Id }, null, null).TotalPrice);
            Assert.AreEqual(0, SchoolTestContext.AdminGradeSl.AppMarketService.GetApplicationTotalPrice(baseData.LiveApp.Id, null, null, null, null, null).TotalPrice);
            Assert.AreEqual(0, SchoolTestContext.AdminGradeSl.AppMarketService.GetApplicationTotalPrice(baseData.LiveApp.Id, null, baseData.Roles, null, null, null).TotalPrice);

            Assert.AreEqual(0, SchoolTestContext.AdminGradeSl.AppMarketService.GetApplicationTotalPrice(baseData.LiveApp.Id, null, null, null, baseData.GradeLevels, null).TotalPrice);
            Assert.AreEqual(0, SchoolTestContext.AdminGradeSl.AppMarketService.GetApplicationTotalPrice(baseData.LiveApp.Id, null, baseData.Roles, null, baseData.GradeLevels, baseData.Departments).TotalPrice);

            baseData.AppInfo.ApplicationPrices = ApplicationPricesInfo.Create(10, null, null);
            baseData.AppInfo.ApplicationAccessInfo.HasAdminMyApps = true;
            var app = SysAdminMasterLocator.ApplicationService.GetApplications(0, 1, false).First();
            app = ApplicationServiceTest.UpdateApp(SysAdminMasterLocator, devContext, app.Id, baseData.AppInfo);
            baseData.LiveApp = SchoolTestContext.AdminGradeSl.ServiceLocatorMaster.ApplicationService.GetApplicationById(app.OriginalRef.Value);

            Assert.AreEqual(10, SchoolTestContext.FirstTeacherSl.AppMarketService.GetApplicationTotalPrice(baseData.LiveApp.Id, SchoolTestContext.FirstTeacher.Id, null, null, null, null).TotalPrice);
            Assert.AreEqual(30, SchoolTestContext.FirstTeacherSl.AppMarketService.GetApplicationTotalPrice(baseData.LiveApp.Id, null, null, new List<Guid> { baseData.MathClass.Id }, null, null).TotalPrice);
            Assert.AreEqual(70, SchoolTestContext.AdminGradeSl.AppMarketService.GetApplicationTotalPrice(baseData.LiveApp.Id, null, null, null, null, null).TotalPrice);
            Assert.AreEqual(70, SchoolTestContext.AdminGradeSl.AppMarketService.GetApplicationTotalPrice(baseData.LiveApp.Id, null, baseData.Roles, null, null, null).TotalPrice);
            Assert.AreEqual(30, SchoolTestContext.AdminGradeSl.AppMarketService.GetApplicationTotalPrice(baseData.LiveApp.Id, null, null, null, baseData.GradeLevels, null).TotalPrice);
            Assert.AreEqual(70, SchoolTestContext.AdminGradeSl.AppMarketService.GetApplicationTotalPrice(baseData.LiveApp.Id, null, baseData.Roles, null, baseData.GradeLevels, baseData.Departments).TotalPrice);

            baseData.AppInfo.ApplicationPrices = ApplicationPricesInfo.Create(10, 15, 35);
            app = ApplicationServiceTest.UpdateApp(SysAdminMasterLocator, devContext, app.Id, baseData.AppInfo);
            baseData.LiveApp = SchoolTestContext.AdminGradeSl.ServiceLocatorMaster.ApplicationService.GetApplicationById(app.OriginalRef.Value);
            Assert.AreEqual(10, SchoolTestContext.FirstTeacherSl.AppMarketService.GetApplicationTotalPrice(baseData.LiveApp.Id, SchoolTestContext.FirstTeacher.Id, null, null, null, null).TotalPrice);
            Assert.AreEqual(25, SchoolTestContext.FirstTeacherSl.AppMarketService.GetApplicationTotalPrice(baseData.LiveApp.Id, null, null, new List<Guid> { baseData.MathClass.Id }, null, null).TotalPrice);
            Assert.AreEqual(35, SchoolTestContext.AdminGradeSl.AppMarketService.GetApplicationTotalPrice(baseData.LiveApp.Id, null, null, null, null, null).TotalPrice);
        
        
        }

        [Test]
        public void CanInstallTest()
        {
            var devContext = CreateDeveloperSchoolTestContext();
            var baseData = PrepareBaseDataForInstallTest(SysAdminMasterLocator, devContext, SchoolTestContext);

            baseData.AppInfo.ApplicationPrices = ApplicationPricesInfo.Create(5, 10, 15);
            var liveApp = SchoolTestContext.AdminGradeSl.ServiceLocatorMaster.ApplicationService
                .GetApplicationById(ApplicationServiceTest.CreateLiveApp(SysAdminMasterLocator, devContext, baseData.AppInfo).OriginalRef.Value);

            Assert.IsFalse(SchoolTestContext.FirstTeacherSl.AppMarketService.CanInstall(liveApp.Id, null, null, null, null, null));
            Assert.IsFalse(SchoolTestContext.AdminGradeSl.AppMarketService.CanInstall(liveApp.Id, null, null, null, null, null));

            var fundRequest = SchoolTestContext.AdminGradeSl.ServiceLocatorMaster
                .FundService.RequestByPurchaseOrder(SchoolTestContext.School.Id, null, 1000, 250, 250, 250, 0, "xxx", null);
            SysAdminMasterLocator.FundService.ApproveReject(fundRequest.Id, true);
          
            var departments =SysAdminMasterLocator.ChalkableDepartmentService.GetChalkableDepartments().Select(x => x.Id).ToList();

            Assert.IsFalse(SchoolTestContext.FirstTeacherSl.AppMarketService.CanInstall(liveApp.Id, devContext.AdminGrade.Id, null, null, null, null));
            Assert.IsTrue(SchoolTestContext.FirstTeacherSl.AppMarketService.CanInstall(liveApp.Id, null, null, null, null, departments));
            Assert.IsTrue(SchoolTestContext.FirstTeacherSl.AppMarketService.CanInstall(liveApp.Id, null, null, null, baseData.GradeLevels, null));
            Assert.IsTrue(SchoolTestContext.FirstTeacherSl.AppMarketService.CanInstall(liveApp.Id, null, baseData.Roles, null, null, null));
            Assert.IsTrue(SchoolTestContext.AdminGradeSl.AppMarketService.CanInstall(liveApp.Id, null, null, new List<Guid> { baseData.MathClass.Id }, null, null));
            Assert.IsTrue(SchoolTestContext.FirstStudentSl.AppMarketService.CanInstall(liveApp.Id, null, null, null, null, null));

            Assert.IsTrue(SchoolTestContext.FirstTeacherSl.AppMarketService.CanInstall(liveApp.Id, null, null, new List<Guid> { baseData.MathClass.Id }, null, null));
            Assert.IsTrue(SchoolTestContext.FirstTeacherSl.AppMarketService.CanInstall(liveApp.Id, null, null, null, null, null));
            Assert.IsTrue(SchoolTestContext.FirstTeacherSl.AppMarketService.CanInstall(liveApp.Id, SchoolTestContext.FirstTeacher.Id, null, null, null, null));
            Assert.IsTrue(SchoolTestContext.FirstStudentSl.AppMarketService.CanInstall(liveApp.Id, SchoolTestContext.FirstStudent.Id, null, null, null, null));

            SchoolTestContext.FirstTeacherSl.AppMarketService.Install(liveApp.Id, null, null, new List<Guid> { baseData.MathClass.Id }
                , null, null, baseData.SchoolYear.Id, DateTime.UtcNow);

            Assert.IsFalse(SchoolTestContext.FirstTeacherSl.AppMarketService.CanInstall(liveApp.Id, null, null, new List<Guid> { baseData.MathClass.Id }, null, null));
            Assert.IsFalse(SchoolTestContext.FirstTeacherSl.AppMarketService.CanInstall(liveApp.Id, SchoolTestContext.FirstTeacher.Id, null, null, null, null));
            Assert.IsFalse(SchoolTestContext.FirstTeacherSl.AppMarketService.CanInstall(liveApp.Id, null, null, null, null, null));
            Assert.IsFalse(SchoolTestContext.FirstStudentSl.AppMarketService.CanInstall(liveApp.Id, SchoolTestContext.FirstStudent.Id, null, null, null, null));
            var appInstalls = SchoolTestContext.FirstTeacherSl.AppMarketService.ListInstalledAppInstalls(SchoolTestContext.FirstTeacher.Id);
            foreach (var appInstall in appInstalls)
            {
                SchoolTestContext.FirstTeacherSl.AppMarketService.Uninstall(appInstall.Id);
            }
            Assert.IsTrue(SchoolTestContext.FirstTeacherSl.AppMarketService.CanInstall(liveApp.Id, SchoolTestContext.FirstTeacher.Id, null, null, null, null));
            baseData.AppInfo.ApplicationAccessInfo.CanAttach = false;
            baseData.AppInfo.ApplicationAccessInfo.HasStudentMyApps = false;
            baseData.AppInfo.ApplicationAccessInfo.HasTeacherMyApps = false;

            baseData.AppInfo = ApplicationServiceTest.PrepareDefaultAppInfo(SysAdminMasterLocator, devContext.Developer.Id, "app2", "http://test.app2.com");
            baseData.AppInfo.ApplicationAccessInfo = ApplicationAccessInfo.Create(false, false, false, false, false, false);
            baseData.AppInfo.ApplicationPrices = ApplicationPricesInfo.Create(5, 10, 15);
            liveApp = SchoolTestContext.AdminGradeSl.ServiceLocatorMaster.ApplicationService
                .GetApplicationById(ApplicationServiceTest.CreateLiveApp(SysAdminMasterLocator, devContext, baseData.AppInfo).OriginalRef.Value);

            AssertException<Exception>(() => devContext.FirstTeacherSl.AppMarketService.CanInstall(liveApp.Id, null, null, null, null, null));
            AssertException<Exception>(() => devContext.FirstStudentSl.AppMarketService.CanInstall(liveApp.Id, devContext.FirstStudent.Id, null, null, null, null));

        }


        [Test]
        public void GetPersonForInstallTest()
        {
            var devContext = CreateDeveloperSchoolTestContext();
            var dataForTest = PrepareBaseDataForInstallTest(SysAdminMasterLocator, devContext, SchoolTestContext);
            var app = dataForTest.LiveApp;

            Assert.AreEqual(7, SchoolTestContext.AdminGradeSl.AppMarketService.GetPersonsForApplicationInstallCount(app.Id, null, null, null, null, null).First(x => x.Type == PersonsFroAppInstallTypeEnum.Total).Count);
            //Why 7? because second teacher in school doesn't have any classes so doesn't attach to any GL
            Assert.AreEqual(1, SchoolTestContext.AdminGradeSl.AppMarketService.GetPersonsForApplicationInstallCount(app.Id,
                                                                                                SchoolTestContext.FirstTeacher.Id, null, null, null, null).First(x => x.Type == PersonsFroAppInstallTypeEnum.Total).Count);
            Assert.AreEqual(1, SchoolTestContext.AdminGradeSl.AppMarketService.GetPersonsForApplicationInstallCount(app.Id,
                                                                                                SchoolTestContext.AdminGrade.Id, null, null, null, null).First(x => x.Type == PersonsFroAppInstallTypeEnum.Total).Count);

            Assert.AreEqual(3, SchoolTestContext.AdminGradeSl.AppMarketService.GetPersonsForApplicationInstallCount(app.Id, 
                                                  null, null, null, null, dataForTest.GradeLevels).First(x => x.Type == PersonsFroAppInstallTypeEnum.Total).Count);
            Assert.AreEqual(3, SchoolTestContext.AdminGradeSl.AppMarketService.GetPersonsForApplicationInstallCount(app.Id,
                                                                                                null, null, null, dataForTest.Departments, null).First(x => x.Type == PersonsFroAppInstallTypeEnum.Total).Count);

            Assert.AreEqual(7, SchoolTestContext.AdminGradeSl.AppMarketService.GetPersonsForApplicationInstallCount(app.Id,
                                                                                                null, dataForTest.Roles, null, dataForTest.Departments, dataForTest.GradeLevels).First(x => x.Type == PersonsFroAppInstallTypeEnum.Total).Count);
            Assert.AreEqual(3, SchoolTestContext.FirstTeacherSl.AppMarketService.GetPersonsForApplicationInstallCount(app.Id, null, null, null, null, null).First(x => x.Type == PersonsFroAppInstallTypeEnum.Total).Count);
            Assert.AreEqual(3, SchoolTestContext.FirstTeacherSl.AppMarketService.GetPersonsForApplicationInstallCount(app.Id, null, null,
                                                                                                 new List<Guid> { dataForTest.MathClass.Id }, null, null).First(x => x.Type == PersonsFroAppInstallTypeEnum.Total).Count);
            Assert.AreEqual(1, SchoolTestContext.FirstTeacherSl.AppMarketService.GetPersonsForApplicationInstallCount(app.Id,
                                                                          SchoolTestContext.FirstTeacher.Id, null, null, null, null).First(x => x.Type == PersonsFroAppInstallTypeEnum.Total).Count);
            Assert.AreEqual(0, SchoolTestContext.FirstTeacherSl.AppMarketService.GetPersonsForApplicationInstallCount(app.Id,
                                                                                                SchoolTestContext.AdminGrade.Id, null, null, null, null).First(x => x.Type == PersonsFroAppInstallTypeEnum.Total).Count);
            Assert.AreEqual(3, SchoolTestContext.FirstTeacherSl.AppMarketService.GetPersonsForApplicationInstallCount(app.Id,
                                                                                                 null, dataForTest.Roles, null, dataForTest.Departments, dataForTest.GradeLevels).First(x => x.Type == PersonsFroAppInstallTypeEnum.Total).Count);

            Assert.AreEqual(1, SchoolTestContext.FirstStudentSl.AppMarketService.GetPersonsForApplicationInstallCount(app.Id,
                                                                                                SchoolTestContext.FirstStudent.Id, null, null, null, null).First(x => x.Type == PersonsFroAppInstallTypeEnum.Total).Count);

            SchoolTestContext.FirstTeacherSl.AppMarketService.Install(app.Id, SchoolTestContext.FirstStudent.Id, null,
                                                                      null, null, null, dataForTest.SchoolYear.Id, SchoolTestContext.NowTime);

            Assert.AreEqual(2, SchoolTestContext.FirstTeacherSl.AppMarketService.GetPersonsForApplicationInstallCount(app.Id, null, null, null, null, null).First(x => x.Type == PersonsFroAppInstallTypeEnum.Total).Count);
            Assert.AreEqual(2, SchoolTestContext.FirstTeacherSl.AppMarketService.GetPersonsForApplicationInstallCount(app.Id, null, null,
                                                                                                 new List<Guid> { dataForTest.MathClass.Id }, null, null).First(x => x.Type == PersonsFroAppInstallTypeEnum.Total).Count);
            Assert.AreEqual(0, SchoolTestContext.FirstStudentSl.AppMarketService.GetPersonsForApplicationInstallCount(app.Id,
                                                                                                SchoolTestContext.FirstStudent.Id, null, null, null, null).First(x => x.Type == PersonsFroAppInstallTypeEnum.Total).Count);
            
            dataForTest.AppInfo.ApplicationAccessInfo = ApplicationAccessInfo.Create(false, false, false, false, false, false);
            dataForTest.AppInfo.ShortApplicationInfo.Name = "app2";
            dataForTest.AppInfo.ShortApplicationInfo.Url = "http://test.app2.com";
            app = ApplicationServiceTest.CreateLiveApp(SysAdminMasterLocator, devContext, dataForTest.AppInfo);
            app = SysAdminMasterLocator.ApplicationService.GetApplicationById(app.OriginalRef.Value);
            
            AssertException<Exception>(() => SchoolTestContext.FirstStudentSl.AppMarketService.GetPersonsForApplicationInstallCount(app.Id, null, null, null, null, null));
            AssertException<Exception>(() => SchoolTestContext.FirstTeacherSl.AppMarketService.GetPersonsForApplicationInstallCount(app.Id, null, null, null, null, null));
            Assert.AreEqual(0, SchoolTestContext.AdminGradeSl.AppMarketService.GetPersonsForApplicationInstallCount(app.Id, null, null, null, null, null).First(x => x.Type == PersonsFroAppInstallTypeEnum.Total).Count);

            
            //TODO:needs continue

        }

        private static AppMarketTestData PrepareBaseDataForInstallTest(IServiceLocatorMaster sysLocator, DeveloperSchoolTestContex devContext, SchoolTestContext schoolContext)
        {
            var res = new AppMarketTestData();
            res.MathClass = ClassServiceTest.CreateClass(schoolContext, schoolContext.FirstTeacher,
                                             schoolContext.FirstStudent, schoolContext.SecondStudent);

            res.SchoolYear = schoolContext.AdminGradeSl.SchoolYearService.GetSchoolYearById(res.MathClass.SchoolYearRef);
            var department = sysLocator.ChalkableDepartmentService.Add("testDepartment", new List<string> { "k", "l" }, null);
            schoolContext.AdminGradeSl.CourseService.Edit(res.MathClass.CourseRef, "test", "test", null, department.Id);
            res.Roles = new List<int>
                {
                    CoreRoles.ADMIN_GRADE_ROLE.Id,
                    CoreRoles.ADMIN_VIEW_ROLE.Id,
                    CoreRoles.ADMIN_EDIT_ROLE.Id,
                    CoreRoles.TEACHER_ROLE.Id,
                    CoreRoles.STUDENT_ROLE.Id
                };
            res.Departments = sysLocator.ChalkableDepartmentService.GetChalkableDepartments().Select(x => x.Id).ToList();
            res.GradeLevels = schoolContext.AdminGradeSl.GradeLevelService.GetGradeLevels().Select(x => x.Id).ToList();
            res.LiveApp = ApplicationServiceTest.CreateDefaultFreeApp(sysLocator, devContext);
            res.AppInfo = BaseApplicationInfo.Create(res.LiveApp);
            return res;
        }

        public class AppMarketTestData
        {
            public Application LiveApp { get; set; }
            public BaseApplicationInfo AppInfo { get; set; }
            public Class MathClass { get; set; }
            public SchoolYear SchoolYear { get; set; }
            public IList<int> Roles { get; set; }
            public IList<Guid> Departments { get; set; }
            public IList<Guid> GradeLevels { get; set; }
        }

    }
}
