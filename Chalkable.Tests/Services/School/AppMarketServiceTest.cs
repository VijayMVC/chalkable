//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using Chalkable.BusinessLogic.Model;
//using Chalkable.BusinessLogic.Services.Master;
//using Chalkable.Common;
//using Chalkable.Data.Master.Model;
//using Chalkable.Data.School.Model;
//using Chalkable.Tests.Services.Master;
//using Chalkable.Tests.Services.TestContext;
//using NUnit.Framework;

//namespace Chalkable.Tests.Services.School
//{
//    public class AppMarketServiceTest : BaseSchoolServiceTest
//    {
//        [Test]
//        public void InstallTest()
//        {
//            var devContext = CreateDeveloperSchoolTestContext();
//            var appInfo = ApplicationServiceTest.PrepareDefaultAppInfo(SysAdminMasterLocator, devContext.Developer.Id, "app1", "http://test.app1.com");
//            appInfo.ApplicationPrices = ApplicationPricesInfo.Create(0, 0, 0);
//            appInfo.ApplicationAccessInfo = ApplicationAccessInfo.Create(false, false, false, false, false, false);
//            var draftApp = ApplicationServiceTest.CreateLiveApp(SysAdminMasterLocator, devContext, appInfo);
//            var liveApp = FirstSchoolContext.AdminGradeSl.ServiceLocatorMaster.ApplicationService.GetApplicationById(draftApp.OriginalRef.Value);

//            var mp = MarkingPeriodServiceTest.CreateSchoolYearWithMp(FirstSchoolContext, null);
//            var c = ClassServiceTest.CreateClass(FirstSchoolContext, FirstSchoolContext.FirstTeacher, FirstSchoolContext.FirstStudent, null);

//            //security 
//            AssertException<Exception>(() => FirstSchoolContext.FirstTeacherSl.AppMarketService
//                .Install(liveApp.Id, null, null, null, null, null, mp.Id, FirstSchoolContext.NowTime));
//            AssertException<Exception>(() => FirstSchoolContext.SecondTeacherSl.AppMarketService
//                .Install(liveApp.Id, null, null, null, null, null, mp.Id, FirstSchoolContext.NowTime));
//            AssertException<Exception>(() => FirstSchoolContext.FirstStudentSl.AppMarketService
//                            .Install(liveApp.Id, null, null, null, null, null, mp.Id, FirstSchoolContext.NowTime));
//            AssertException<Exception>(() => FirstSchoolContext.FirstTeacherSl.AppMarketService
//                .Install(liveApp.Id, null, null, null, null, null, mp.Id, FirstSchoolContext.NowTime));

//            appInfo.ApplicationAccessInfo = ApplicationAccessInfo.Create(true, true, false, false, true, false);
//            draftApp = ApplicationServiceTest.UpdateApp(SysAdminMasterLocator, devContext, draftApp.Id, appInfo);
//            liveApp = FirstSchoolContext.AdminGradeSl.ServiceLocatorMaster.ApplicationService.GetApplicationById(draftApp.OriginalRef.Value);
//            Assert.IsFalse(FirstSchoolContext.FirstTeacherSl.AppMarketService.CanInstall(liveApp.Id, FirstSchoolContext.SecondTeahcer.Id, null, null, null,null));
//            AssertException<Exception>(() => FirstSchoolContext.FirstTeacherSl.AppMarketService
//                .Install(liveApp.Id, FirstSchoolContext.SecondTeahcer.Id, null, null, null, null, mp.Id, FirstSchoolContext.NowTime));
//            Assert.IsFalse(FirstSchoolContext.FirstTeacherSl.AppMarketService.CanInstall(liveApp.Id, FirstSchoolContext.AdminGrade.Id, null, null, null, null));
//            AssertException<Exception>(() => FirstSchoolContext.FirstTeacherSl.AppMarketService
//                .Install(liveApp.Id, FirstSchoolContext.AdminGrade.Id, null, null, null, null, mp.Id, FirstSchoolContext.NowTime));
//            Assert.IsFalse(FirstSchoolContext.FirstTeacherSl.AppMarketService.CanInstall(liveApp.Id, FirstSchoolContext.SecondStudent.Id, null, null, null, null));
//            AssertException<Exception>(() => FirstSchoolContext.FirstTeacherSl.AppMarketService
//                .Install(liveApp.Id, FirstSchoolContext.SecondStudent.Id, null, null, null, null, mp.Id, FirstSchoolContext.NowTime));


//            appInfo.ApplicationPrices = ApplicationPricesInfo.Create(2, 10, 100);
//            draftApp = ApplicationServiceTest.UpdateApp(SysAdminMasterLocator, devContext, draftApp.Id, appInfo);
//            liveApp = FirstSchoolContext.AdminGradeSl.ServiceLocatorMaster.ApplicationService.GetApplicationById(draftApp.OriginalRef.Value);
//            Assert.IsFalse(FirstSchoolContext.FirstTeacherSl.AppMarketService.CanInstall(liveApp.Id, null, null, null, null, null));
//            AssertException<Exception>(() => FirstSchoolContext.FirstTeacherSl.AppMarketService
//                             .Install(liveApp.Id, null, null, null, null, null, mp.SchoolYearRef, FirstSchoolContext.NowTime));

//            var fr = FirstSchoolContext.AdminGradeSl.ServiceLocatorMaster.FundService.RequestByPurchaseOrder(FirstSchoolContext.School.Id, FirstSchoolContext.FirstTeacher.Id, 300, 100, 100,
//                                                                     100, 0, "test", new byte[] {1, 2, 3});
//            SysAdminMasterLocator.FundService.ApproveReject(fr.Id, true);
//            var marketS = FirstSchoolContext.FirstTeacherSl.AppMarketService;
//            Assert.IsTrue(marketS.CanInstall(liveApp.Id, null, null, null, null, null));
//            Assert.IsTrue(marketS.CanInstall(liveApp.Id, null, null, new List<Guid> { c.Id }, null, null));
//            var appTotalPrice = FirstSchoolContext.FirstTeacherSl.AppMarketService.GetApplicationTotalPrice(liveApp.Id, null, null, new List<Guid> { c.Id }, null, null);
//            Assert.AreEqual(appTotalPrice.TotalPrice, 4);
//            Assert.AreEqual(appTotalPrice.ApplicationInstallCountInfo.First(x => x.Type == PersonsFroAppInstallTypeEnum.Total).Count, 2);
//            Assert.AreEqual(marketS.GetApplicationTotalPrice(liveApp.Id, FirstSchoolContext.FirstStudent.Id, null, null, null, null).TotalPrice, 2);

//            var aia = FirstSchoolContext.FirstTeacherSl.AppMarketService.Install(liveApp.Id, null, null, new List<Guid> {c.Id}, null, null, mp.SchoolYearRef, FirstSchoolContext.NowTime);

//            var appInstalls = marketS.GetInstallations(liveApp.Id, FirstSchoolContext.FirstTeacher.Id);
//            Assert.AreEqual(appInstalls.Count, 2);


//            //var roles = new List<int> {CoreRoles.ADMIN_GRADE_ROLE.Id};
//            //Assert.IsFalse(FirstSchoolContext.FirstTeacherSl.AppMarketService.CanInstall(app.Id, null, roles, null, null, null));
//            //AssertException<Exception>(() => FirstSchoolContext.FirstTeacherSl.AppMarketService
//            //    .Install(app.Id, null, roles, null, null, null, mp.Id, FirstSchoolContext.NowTime));

//            //Assert.IsFalse(FirstSchoolContext.FirstTeacherSl.AppMarketService.CanInstall(app.Id, null, null, null, new List<Guid>{c.GradeLevelRef}, null));
//            //AssertException<Exception>(() => FirstSchoolContext.FirstTeacherSl.AppMarketService
//            //    .Install(app.Id, null, null, null, null, new List<Guid> { c.GradeLevelRef }, mp.Id, FirstSchoolContext.NowTime));


//            //Assert.IsFalse(FirstSchoolContext.SecondTeacherSl.AppMarketService.CanInstall(app.Id, null, null, new List<Guid> { c.Id }, null, null));
//            //AssertException<Exception>(() => FirstSchoolContext.SecondTeacherSl.AppMarketService
//            //    .Install(app.Id, null, null, new List<Guid> { c.Id }, null, null, mp.Id, FirstSchoolContext.NowTime));
            

//            //TODO: implementation
//        }



//        [Test]
//        public void GetApplicationTotalPrice()
//        {
//            var devContext = CreateDeveloperSchoolTestContext();
//            var baseData = PrepareBaseDataForInstallTest(SysAdminMasterLocator, devContext, FirstSchoolContext);

//            Assert.AreEqual(0, FirstSchoolContext.FirstTeacherSl.AppMarketService.GetApplicationTotalPrice(baseData.LiveApp.Id, devContext.FirstTeacher.Id, null, null, null, null).TotalPrice);
//            Assert.AreEqual(0, FirstSchoolContext.FirstTeacherSl.AppMarketService.GetApplicationTotalPrice(baseData.LiveApp.Id, null, null, new List<Guid> { baseData.MathClass.Id }, null, null).TotalPrice);
//            Assert.AreEqual(0, FirstSchoolContext.AdminGradeSl.AppMarketService.GetApplicationTotalPrice(baseData.LiveApp.Id, null, null, null, null, null).TotalPrice);
//            Assert.AreEqual(0, FirstSchoolContext.AdminGradeSl.AppMarketService.GetApplicationTotalPrice(baseData.LiveApp.Id, null, baseData.Roles, null, null, null).TotalPrice);

//            Assert.AreEqual(0, FirstSchoolContext.AdminGradeSl.AppMarketService.GetApplicationTotalPrice(baseData.LiveApp.Id, null, null, null, baseData.GradeLevels, null).TotalPrice);
//            Assert.AreEqual(0, FirstSchoolContext.AdminGradeSl.AppMarketService.GetApplicationTotalPrice(baseData.LiveApp.Id, null, baseData.Roles, null, baseData.GradeLevels, baseData.Departments).TotalPrice);

//            baseData.AppInfo.ApplicationPrices = ApplicationPricesInfo.Create(10, null, null);
//            baseData.AppInfo.ApplicationAccessInfo.HasAdminMyApps = true;
//            var app = SysAdminMasterLocator.ApplicationService.GetApplications(0, 1, false).First();
//            app = ApplicationServiceTest.UpdateApp(SysAdminMasterLocator, devContext, app.Id, baseData.AppInfo);
//            baseData.LiveApp = FirstSchoolContext.AdminGradeSl.ServiceLocatorMaster.ApplicationService.GetApplicationById(app.OriginalRef.Value);

//            Assert.AreEqual(10, FirstSchoolContext.FirstTeacherSl.AppMarketService.GetApplicationTotalPrice(baseData.LiveApp.Id, FirstSchoolContext.FirstTeacher.Id, null, null, null, null).TotalPrice);
//            Assert.AreEqual(30, FirstSchoolContext.FirstTeacherSl.AppMarketService.GetApplicationTotalPrice(baseData.LiveApp.Id, null, null, new List<Guid> { baseData.MathClass.Id }, null, null).TotalPrice);
//            Assert.AreEqual(70, FirstSchoolContext.AdminGradeSl.AppMarketService.GetApplicationTotalPrice(baseData.LiveApp.Id, null, null, null, null, null).TotalPrice);
//            Assert.AreEqual(70, FirstSchoolContext.AdminGradeSl.AppMarketService.GetApplicationTotalPrice(baseData.LiveApp.Id, null, baseData.Roles, null, null, null).TotalPrice);
//            Assert.AreEqual(30, FirstSchoolContext.AdminGradeSl.AppMarketService.GetApplicationTotalPrice(baseData.LiveApp.Id, null, null, null, baseData.GradeLevels, null).TotalPrice);
//            Assert.AreEqual(70, FirstSchoolContext.AdminGradeSl.AppMarketService.GetApplicationTotalPrice(baseData.LiveApp.Id, null, baseData.Roles, null, baseData.GradeLevels, baseData.Departments).TotalPrice);

//            baseData.AppInfo.ApplicationPrices = ApplicationPricesInfo.Create(10, 15, 35);
//            app = ApplicationServiceTest.UpdateApp(SysAdminMasterLocator, devContext, app.Id, baseData.AppInfo);
//            baseData.LiveApp = FirstSchoolContext.AdminGradeSl.ServiceLocatorMaster.ApplicationService.GetApplicationById(app.OriginalRef.Value);
//            Assert.AreEqual(10, FirstSchoolContext.FirstTeacherSl.AppMarketService.GetApplicationTotalPrice(baseData.LiveApp.Id, FirstSchoolContext.FirstTeacher.Id, null, null, null, null).TotalPrice);
//            Assert.AreEqual(25, FirstSchoolContext.FirstTeacherSl.AppMarketService.GetApplicationTotalPrice(baseData.LiveApp.Id, null, null, new List<Guid> { baseData.MathClass.Id }, null, null).TotalPrice);
//            Assert.AreEqual(35, FirstSchoolContext.AdminGradeSl.AppMarketService.GetApplicationTotalPrice(baseData.LiveApp.Id, null, null, null, null, null).TotalPrice);
        
        
//        }

//        [Test]
//        public void CanInstallTest()
//        {
//            var devContext = CreateDeveloperSchoolTestContext();
//            var baseData = PrepareBaseDataForInstallTest(SysAdminMasterLocator, devContext, FirstSchoolContext);

//            baseData.AppInfo.ApplicationPrices = ApplicationPricesInfo.Create(5, 10, 15);
//            var liveApp = FirstSchoolContext.AdminGradeSl.ServiceLocatorMaster.ApplicationService
//                .GetApplicationById(ApplicationServiceTest.CreateLiveApp(SysAdminMasterLocator, devContext, baseData.AppInfo).OriginalRef.Value);

//            Assert.IsFalse(FirstSchoolContext.FirstTeacherSl.AppMarketService.CanInstall(liveApp.Id, null, null, null, null, null));
//            Assert.IsFalse(FirstSchoolContext.AdminGradeSl.AppMarketService.CanInstall(liveApp.Id, null, null, null, null, null));

//            var fundRequest = FirstSchoolContext.AdminGradeSl.ServiceLocatorMaster
//                .FundService.RequestByPurchaseOrder(FirstSchoolContext.School.Id, null, 1000, 250, 250, 250, 0, "xxx", null);
//            SysAdminMasterLocator.FundService.ApproveReject(fundRequest.Id, true);
          
//            var departments =SysAdminMasterLocator.ChalkableDepartmentService.GetChalkableDepartments().Select(x => x.Id).ToList();

//            Assert.IsFalse(FirstSchoolContext.FirstTeacherSl.AppMarketService.CanInstall(liveApp.Id, devContext.AdminGrade.Id, null, null, null, null));
//            Assert.IsTrue(FirstSchoolContext.FirstTeacherSl.AppMarketService.CanInstall(liveApp.Id, null, null, null, null, departments));
//            Assert.IsTrue(FirstSchoolContext.FirstTeacherSl.AppMarketService.CanInstall(liveApp.Id, null, null, null, baseData.GradeLevels, null));
//            Assert.IsTrue(FirstSchoolContext.FirstTeacherSl.AppMarketService.CanInstall(liveApp.Id, null, baseData.Roles, null, null, null));
//            Assert.IsTrue(FirstSchoolContext.AdminGradeSl.AppMarketService.CanInstall(liveApp.Id, null, null, new List<Guid> { baseData.MathClass.Id }, null, null));
//            Assert.IsTrue(FirstSchoolContext.FirstStudentSl.AppMarketService.CanInstall(liveApp.Id, null, null, null, null, null));

//            Assert.IsTrue(FirstSchoolContext.FirstTeacherSl.AppMarketService.CanInstall(liveApp.Id, null, null, new List<Guid> { baseData.MathClass.Id }, null, null));
//            Assert.IsTrue(FirstSchoolContext.FirstTeacherSl.AppMarketService.CanInstall(liveApp.Id, null, null, null, null, null));
//            Assert.IsTrue(FirstSchoolContext.FirstTeacherSl.AppMarketService.CanInstall(liveApp.Id, FirstSchoolContext.FirstTeacher.Id, null, null, null, null));
//            Assert.IsTrue(FirstSchoolContext.FirstStudentSl.AppMarketService.CanInstall(liveApp.Id, FirstSchoolContext.FirstStudent.Id, null, null, null, null));

//            FirstSchoolContext.FirstTeacherSl.AppMarketService.Install(liveApp.Id, null, null, new List<Guid> { baseData.MathClass.Id }
//                , null, null, baseData.SchoolYear.Id, DateTime.UtcNow);

//            Assert.IsFalse(FirstSchoolContext.FirstTeacherSl.AppMarketService.CanInstall(liveApp.Id, null, null, new List<Guid> { baseData.MathClass.Id }, null, null));
//            Assert.IsFalse(FirstSchoolContext.FirstTeacherSl.AppMarketService.CanInstall(liveApp.Id, FirstSchoolContext.FirstTeacher.Id, null, null, null, null));
//            Assert.IsFalse(FirstSchoolContext.FirstTeacherSl.AppMarketService.CanInstall(liveApp.Id, null, null, null, null, null));
//            Assert.IsFalse(FirstSchoolContext.FirstStudentSl.AppMarketService.CanInstall(liveApp.Id, FirstSchoolContext.FirstStudent.Id, null, null, null, null));
//            var appInstalls = FirstSchoolContext.FirstTeacherSl.AppMarketService.ListInstalledAppInstalls(FirstSchoolContext.FirstTeacher.Id);
//            foreach (var appInstall in appInstalls)
//            {
//                FirstSchoolContext.FirstTeacherSl.AppMarketService.Uninstall(appInstall.Id);
//            }
//            Assert.IsTrue(FirstSchoolContext.FirstTeacherSl.AppMarketService.CanInstall(liveApp.Id, FirstSchoolContext.FirstTeacher.Id, null, null, null, null));
//            baseData.AppInfo.ApplicationAccessInfo.CanAttach = false;
//            baseData.AppInfo.ApplicationAccessInfo.HasStudentMyApps = false;
//            baseData.AppInfo.ApplicationAccessInfo.HasTeacherMyApps = false;

//            baseData.AppInfo = ApplicationServiceTest.PrepareDefaultAppInfo(SysAdminMasterLocator, devContext.Developer.Id, "app2", "http://test.app2.com");
//            baseData.AppInfo.ApplicationAccessInfo = ApplicationAccessInfo.Create(false, false, false, false, false, false);
//            baseData.AppInfo.ApplicationPrices = ApplicationPricesInfo.Create(5, 10, 15);
//            liveApp = FirstSchoolContext.AdminGradeSl.ServiceLocatorMaster.ApplicationService
//                .GetApplicationById(ApplicationServiceTest.CreateLiveApp(SysAdminMasterLocator, devContext, baseData.AppInfo).OriginalRef.Value);

//            AssertException<Exception>(() => devContext.FirstTeacherSl.AppMarketService.CanInstall(liveApp.Id, null, null, null, null, null));
//            AssertException<Exception>(() => devContext.FirstStudentSl.AppMarketService.CanInstall(liveApp.Id, devContext.FirstStudent.Id, null, null, null, null));

//        }


//        [Test]
//        public void GetPersonForInstallTest()
//        {
//            var devContext = CreateDeveloperSchoolTestContext();
//            var dataForTest = PrepareBaseDataForInstallTest(SysAdminMasterLocator, devContext, FirstSchoolContext);
//            var app = dataForTest.LiveApp;

//            Assert.AreEqual(7, FirstSchoolContext.AdminGradeSl.AppMarketService.GetPersonsForApplicationInstallCount(app.Id, null, null, null, null, null).First(x => x.Type == PersonsFroAppInstallTypeEnum.Total).Count);
//            //Why 7? because second teacher in school doesn't have any classes so doesn't attach to any GL
//            Assert.AreEqual(1, FirstSchoolContext.AdminGradeSl.AppMarketService.GetPersonsForApplicationInstallCount(app.Id,
//                                                                                                FirstSchoolContext.FirstTeacher.Id, null, null, null, null).First(x => x.Type == PersonsFroAppInstallTypeEnum.Total).Count);
//            Assert.AreEqual(1, FirstSchoolContext.AdminGradeSl.AppMarketService.GetPersonsForApplicationInstallCount(app.Id,
//                                                                                                FirstSchoolContext.AdminGrade.Id, null, null, null, null).First(x => x.Type == PersonsFroAppInstallTypeEnum.Total).Count);

//            Assert.AreEqual(3, FirstSchoolContext.AdminGradeSl.AppMarketService.GetPersonsForApplicationInstallCount(app.Id, 
//                                                  null, null, null, null, dataForTest.GradeLevels).First(x => x.Type == PersonsFroAppInstallTypeEnum.Total).Count);
//            Assert.AreEqual(3, FirstSchoolContext.AdminGradeSl.AppMarketService.GetPersonsForApplicationInstallCount(app.Id,
//                                                                                                null, null, null, dataForTest.Departments, null).First(x => x.Type == PersonsFroAppInstallTypeEnum.Total).Count);

//            Assert.AreEqual(7, FirstSchoolContext.AdminGradeSl.AppMarketService.GetPersonsForApplicationInstallCount(app.Id,
//                                                                                                null, dataForTest.Roles, null, dataForTest.Departments, dataForTest.GradeLevels).First(x => x.Type == PersonsFroAppInstallTypeEnum.Total).Count);
//            Assert.AreEqual(3, FirstSchoolContext.FirstTeacherSl.AppMarketService.GetPersonsForApplicationInstallCount(app.Id, null, null, null, null, null).First(x => x.Type == PersonsFroAppInstallTypeEnum.Total).Count);
//            Assert.AreEqual(3, FirstSchoolContext.FirstTeacherSl.AppMarketService.GetPersonsForApplicationInstallCount(app.Id, null, null,
//                                                                                                 new List<Guid> { dataForTest.MathClass.Id }, null, null).First(x => x.Type == PersonsFroAppInstallTypeEnum.Total).Count);
//            Assert.AreEqual(1, FirstSchoolContext.FirstTeacherSl.AppMarketService.GetPersonsForApplicationInstallCount(app.Id,
//                                                                          FirstSchoolContext.FirstTeacher.Id, null, null, null, null).First(x => x.Type == PersonsFroAppInstallTypeEnum.Total).Count);
//            Assert.AreEqual(0, FirstSchoolContext.FirstTeacherSl.AppMarketService.GetPersonsForApplicationInstallCount(app.Id,
//                                                                                                FirstSchoolContext.AdminGrade.Id, null, null, null, null).First(x => x.Type == PersonsFroAppInstallTypeEnum.Total).Count);
//            Assert.AreEqual(3, FirstSchoolContext.FirstTeacherSl.AppMarketService.GetPersonsForApplicationInstallCount(app.Id,
//                                                                                                 null, dataForTest.Roles, null, dataForTest.Departments, dataForTest.GradeLevels).First(x => x.Type == PersonsFroAppInstallTypeEnum.Total).Count);

//            Assert.AreEqual(1, FirstSchoolContext.FirstStudentSl.AppMarketService.GetPersonsForApplicationInstallCount(app.Id,
//                                                                                                FirstSchoolContext.FirstStudent.Id, null, null, null, null).First(x => x.Type == PersonsFroAppInstallTypeEnum.Total).Count);

//            FirstSchoolContext.FirstTeacherSl.AppMarketService.Install(app.Id, FirstSchoolContext.FirstStudent.Id, null,
//                                                                      null, null, null, dataForTest.SchoolYear.Id, FirstSchoolContext.NowTime);

//            Assert.AreEqual(2, FirstSchoolContext.FirstTeacherSl.AppMarketService.GetPersonsForApplicationInstallCount(app.Id, null, null, null, null, null).First(x => x.Type == PersonsFroAppInstallTypeEnum.Total).Count);
//            Assert.AreEqual(2, FirstSchoolContext.FirstTeacherSl.AppMarketService.GetPersonsForApplicationInstallCount(app.Id, null, null,
//                                                                                                 new List<Guid> { dataForTest.MathClass.Id }, null, null).First(x => x.Type == PersonsFroAppInstallTypeEnum.Total).Count);
//            Assert.AreEqual(0, FirstSchoolContext.FirstStudentSl.AppMarketService.GetPersonsForApplicationInstallCount(app.Id,
//                                                                                                FirstSchoolContext.FirstStudent.Id, null, null, null, null).First(x => x.Type == PersonsFroAppInstallTypeEnum.Total).Count);
            
//            dataForTest.AppInfo.ApplicationAccessInfo = ApplicationAccessInfo.Create(false, false, false, false, false, false);
//            dataForTest.AppInfo.ShortApplicationInfo.Name = "app2";
//            dataForTest.AppInfo.ShortApplicationInfo.Url = "http://test.app2.com";
//            app = ApplicationServiceTest.CreateLiveApp(SysAdminMasterLocator, devContext, dataForTest.AppInfo);
//            app = SysAdminMasterLocator.ApplicationService.GetApplicationById(app.OriginalRef.Value);
            
//            AssertException<Exception>(() => FirstSchoolContext.FirstStudentSl.AppMarketService.GetPersonsForApplicationInstallCount(app.Id, null, null, null, null, null));
//            AssertException<Exception>(() => FirstSchoolContext.FirstTeacherSl.AppMarketService.GetPersonsForApplicationInstallCount(app.Id, null, null, null, null, null));
//            Assert.AreEqual(0, FirstSchoolContext.AdminGradeSl.AppMarketService.GetPersonsForApplicationInstallCount(app.Id, null, null, null, null, null).First(x => x.Type == PersonsFroAppInstallTypeEnum.Total).Count);

            
//            //TODO:needs continue

//        }

//        private static AppMarketTestData PrepareBaseDataForInstallTest(IServiceLocatorMaster sysLocator, DeveloperSchoolTestContex devContext, FirstSchoolContext schoolContext)
//        {
//            var res = new AppMarketTestData();
//            res.MathClass = ClassServiceTest.CreateClass(schoolContext, schoolContext.FirstTeacher,
//                                             schoolContext.FirstStudent, schoolContext.SecondStudent);

//            res.SchoolYear = schoolContext.AdminGradeSl.SchoolYearService.GetSchoolYearById(res.MathClass.SchoolYearRef);
//            var department = sysLocator.ChalkableDepartmentService.Add("testDepartment", new List<string> { "k", "l" }, null);
//            schoolContext.AdminGradeSl.CourseService.Edit(res.MathClass.CourseRef, "test", "test", null, department.Id);
//            res.Roles = new List<int>
//                {
//                    CoreRoles.ADMIN_GRADE_ROLE.Id,
//                    CoreRoles.ADMIN_VIEW_ROLE.Id,
//                    CoreRoles.ADMIN_EDIT_ROLE.Id,
//                    CoreRoles.TEACHER_ROLE.Id,
//                    CoreRoles.STUDENT_ROLE.Id
//                };
//            res.Departments = sysLocator.ChalkableDepartmentService.GetChalkableDepartments().Select(x => x.Id).ToList();
//            res.GradeLevels = schoolContext.AdminGradeSl.GradeLevelService.GetGradeLevels().Select(x => x.Id).ToList();
//            res.LiveApp = ApplicationServiceTest.CreateDefaultFreeApp(sysLocator, devContext);
//            res.AppInfo = BaseApplicationInfo.Create(res.LiveApp);
//            return res;
//        }

//        public class AppMarketTestData
//        {
//            public Application LiveApp { get; set; }
//            public BaseApplicationInfo AppInfo { get; set; }
//            public Class MathClass { get; set; }
//            public SchoolYear SchoolYear { get; set; }
//            public IList<int> Roles { get; set; }
//            public IList<Guid> Departments { get; set; }
//            public IList<Guid> GradeLevels { get; set; }
//        }

//    }
//}
