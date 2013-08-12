using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chalkable.BusinessLogic.Model;
using Chalkable.Common;
using Chalkable.Data.Master.Model;
using Chalkable.Data.School.Model;
using Chalkable.Tests.Services.Master;
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
            var c = ClassServiceTest.CreateClass(SchoolTestContext, SchoolTestContext.FirstTeacher,
                                                         SchoolTestContext.FirstStudent, SchoolTestContext.SecondStudent);
            var roles = new List<int>
                {
                    CoreRoles.ADMIN_GRADE_ROLE.Id,
                    CoreRoles.ADMIN_VIEW_ROLE.Id,
                    CoreRoles.ADMIN_EDIT_ROLE.Id,
                    CoreRoles.TEACHER_ROLE.Id,
                    CoreRoles.STUDENT_ROLE.Id
                };
            var departments = SysAdminMasterLocator.ChalkableDepartmentService.GetChalkableDepartments().Select(x => x.Id).ToList();
            var gradeLevels = SchoolTestContext.AdminGradeSl.GradeLevelService.GetGradeLevels();
            var gradeLevelsIds = gradeLevels.Select(x => x.Id).ToList();
            
            var appInfo = ApplicationServiceTest.PrepareDefaultAppInfo(SysAdminMasterLocator, devContext.Developer.Id, "app1", "http://test.app1.com");
            appInfo.ApplicationPrices = ApplicationPricesInfo.Create(0, 0, 0);
            appInfo.GradeLevels = gradeLevels.Select(x => x.Number).ToList();
            var app = ApplicationServiceTest.CreateLiveApp(SysAdminMasterLocator,devContext, appInfo);
            var liveApp = SchoolTestContext.AdminGradeSl.ServiceLocatorMaster.ApplicationService.GetApplicationById(app.OriginalRef.Value);
            
            Assert.AreEqual(0, SchoolTestContext.FirstTeacherSl.AppMarketService.GetApplicationTotalPrice(liveApp.Id, devContext.FirstTeacher.Id, null, null, null, null).TotalPrice);
            Assert.AreEqual(0, SchoolTestContext.FirstTeacherSl.AppMarketService.GetApplicationTotalPrice(liveApp.Id, null, null, new List<Guid> { c.Id }, null, null).TotalPrice);
            Assert.AreEqual(0, SchoolTestContext.AdminGradeSl.AppMarketService.GetApplicationTotalPrice(liveApp.Id, null, null, null, null, null).TotalPrice);
            Assert.AreEqual(0, SchoolTestContext.AdminGradeSl.AppMarketService.GetApplicationTotalPrice(liveApp.Id, null, roles, null, null, null).TotalPrice);

            Assert.AreEqual(0, SchoolTestContext.AdminGradeSl.AppMarketService.GetApplicationTotalPrice(liveApp.Id, null, null, null, gradeLevelsIds, null).TotalPrice);
            Assert.AreEqual(0, SchoolTestContext.AdminGradeSl.AppMarketService.GetApplicationTotalPrice(liveApp.Id, null, roles, null, gradeLevelsIds, departments).TotalPrice);

            appInfo.ApplicationPrices = ApplicationPricesInfo.Create(10, null, null);
            appInfo.ApplicationAccessInfo.HasAdminMyApps = true;
            app = ApplicationServiceTest.UpdateApp(SysAdminMasterLocator, devContext, app.Id, appInfo);
            liveApp = SchoolTestContext.AdminGradeSl.ServiceLocatorMaster.ApplicationService.GetApplicationById(app.OriginalRef.Value);

            Assert.AreEqual(10, SchoolTestContext.FirstTeacherSl.AppMarketService.GetApplicationTotalPrice(liveApp.Id, SchoolTestContext.FirstTeacher.Id, null, null, null, null).TotalPrice);
            Assert.AreEqual(30, SchoolTestContext.FirstTeacherSl.AppMarketService.GetApplicationTotalPrice(liveApp.Id, null, null, new List<Guid> { c.Id }, null, null).TotalPrice);
            Assert.AreEqual(70, SchoolTestContext.AdminGradeSl.AppMarketService.GetApplicationTotalPrice(liveApp.Id, null, null, null, null, null).TotalPrice);
            Assert.AreEqual(70, SchoolTestContext.AdminGradeSl.AppMarketService.GetApplicationTotalPrice(liveApp.Id, null, roles, null, null, null).TotalPrice);
            Assert.AreEqual(30, SchoolTestContext.AdminGradeSl.AppMarketService.GetApplicationTotalPrice(liveApp.Id, null, null, null, gradeLevelsIds, null).TotalPrice);
            Assert.AreEqual(70, SchoolTestContext.AdminGradeSl.AppMarketService.GetApplicationTotalPrice(liveApp.Id, null, roles, null, gradeLevelsIds, departments).TotalPrice);

            appInfo.ApplicationPrices = ApplicationPricesInfo.Create(10, 15, 35);
            app = ApplicationServiceTest.UpdateApp(SysAdminMasterLocator, devContext, app.Id, appInfo);
            liveApp = SchoolTestContext.AdminGradeSl.ServiceLocatorMaster.ApplicationService.GetApplicationById(app.OriginalRef.Value);
            Assert.AreEqual(10, SchoolTestContext.FirstTeacherSl.AppMarketService.GetApplicationTotalPrice(liveApp.Id, SchoolTestContext.FirstTeacher.Id, null, null, null, null).TotalPrice);
            Assert.AreEqual(25, SchoolTestContext.FirstTeacherSl.AppMarketService.GetApplicationTotalPrice(liveApp.Id, null, null, new List<Guid> { c.Id }, null, null).TotalPrice);
            Assert.AreEqual(35, SchoolTestContext.AdminGradeSl.AppMarketService.GetApplicationTotalPrice(liveApp.Id, null, null, null, null, null).TotalPrice);
        
        
        }

        //[Test]
        //public void CanInstallTest()
        //{
        //    var devContext = CreateDeveloperSchoolTestContext();
        //    var c = ClassServiceTest.CreateClass(SchoolTestContext, SchoolTestContext.FirstTeacher,
        //                                                 SchoolTestContext.FirstStudent, SchoolTestContext.SecondStudent);
          
        //    var schoolyear = SchoolTestContext.AdminGradeSl.SchoolYearService.GetCurrentSchoolYear(c.SchoolYearRef);
        //    var department = SysAdminServiceLocator.ChalkableDepartmentService.Add("testDepartment", new List<string> { "k", "l" }, null);
        //    devContext.School1AdminSL.RecreateEntities();
        //    devContext.School1AdminSL.CourseInfoService.Edit(mathClass.CourseInfoId, "test", "test", null, department.Id);


        //    var roles = new List<int>();
        //    roles.Add(SysAdminServiceLocator.UserService.GetRoleByName(CoreRoles.ADMIN_GRADE_ROLE.Name).Id);
        //    roles.Add(SysAdminServiceLocator.UserService.GetRoleByName(CoreRoles.ADMIN_VIEW_ROLE.Name).Id);
        //    roles.Add(SysAdminServiceLocator.UserService.GetRoleByName(CoreRoles.ADMIN_EDIT_ROLE.Name).Id);
        //    roles.Add(SysAdminServiceLocator.UserService.GetRoleByName(CoreRoles.TEACHER_ROLE.Name).Id);
        //    roles.Add(SysAdminServiceLocator.UserService.GetRoleByName(CoreRoles.STUDENT_ROLE.Name).Id);

        //    var applicationPriceInfo = ApplicationPricesInfo.Create(5, 10, 15);
        //    BaseApplicationInfo appInfo;
        //    var application = devContext.School1AdminSL.ApplicationService.GetApplicationById(ApplicationServiceTest
        //          .CreateDefualt(devContext, out appInfo, ApplicationStateEnum.Live, null, "app1", "http://app1.com", applicationPriceInfo).OriginalRef.Value);

        //    Assert.IsFalse(devContext.School1TeacherSL.AppMarketService.CanInstall(application.Id, null, null, null, null, null));
        //    Assert.IsFalse(devContext.School1AdminSL.AppMarketService.CanInstall(application.Id, null, null, null, null, null));

        //    var fundRequest = devContext.School1AdminSL.FundService.RequestByPurchaseOrder(schoolyear.SchoolInfoRef, null, 1000, 250, 250, 250, 0, "xxx", null);
        //    SysAdminServiceLocator.FundService.ApproveReject(fundRequest.Id, true);
        //    devContext.School1TeacherSL.RecreateEntities();
        //    devContext.School1AdminSL.RecreateEntities();

        //    var departments = devContext.School1AdminSL.ChalkableDepartmentService.List().Select(x => x.Id).ToList();

        //    Assert.IsFalse(devContext.School1TeacherSL.AppMarketService.CanInstall(application.Id, devContext.School1Admin.Id, null, null, null, null));
        //    Assert.IsTrue(devContext.School1TeacherSL.AppMarketService.CanInstall(application.Id, null, null, null, null, departments));
        //    Assert.IsTrue(devContext.School1TeacherSL.AppMarketService.CanInstall(application.Id, null, null, null, appInfo.GradeLevels, null));
        //    Assert.IsTrue(devContext.School1TeacherSL.AppMarketService.CanInstall(application.Id, null, roles, null, null, null));
        //    Assert.IsTrue(devContext.School1AdminSL.AppMarketService.CanInstall(application.Id, null, null, new List<int> { mathClass.ClassId }, null, null));
        //    Assert.IsTrue(devContext.School1StudentSL.AppMarketService.CanInstall(application.Id, null, null, null, null, null));

        //    Assert.IsTrue(devContext.School1TeacherSL.AppMarketService.CanInstall(application.Id, null, null, new List<int> { mathClass.ClassId }, null, null));
        //    Assert.IsTrue(devContext.School1TeacherSL.AppMarketService.CanInstall(application.Id, null, null, null, null, null));
        //    Assert.IsTrue(devContext.School1TeacherSL.AppMarketService.CanInstall(application.Id, devContext.School1Teacher.Id, null, null, null, null));
        //    Assert.IsTrue(devContext.School1StudentSL.AppMarketService.CanInstall(application.Id, devContext.School1Student.Id, null, null, null, null));

        //    devContext.School1TeacherSL.AppMarketService.Install(application.Id, devContext.School1Id, null, null,
        //                                                          new List<int> { mathClass.ClassId }, null, null, schoolyear.Id,
        //                                                          DateTime.UtcNow);
        //    Assert.IsFalse(devContext.School1TeacherSL.AppMarketService.CanInstall(application.Id, null, null, new List<int> { mathClass.ClassId }, null, null));
        //    Assert.IsFalse(devContext.School1TeacherSL.AppMarketService.CanInstall(application.Id, devContext.School1Teacher.Id, null, null, null, null));
        //    Assert.IsFalse(devContext.School1TeacherSL.AppMarketService.CanInstall(application.Id, null, null, null, null, null));
        //    Assert.IsFalse(devContext.School1StudentSL.AppMarketService.CanInstall(application.Id, devContext.School1Student.Id, null, null, null, null));
        //    var appInstalls = devContext.School1TeacherSL.AppMarketService.ListInstalledAppInstalls(devContext.School1Teacher.Id);
        //    foreach (var appInstall in appInstalls)
        //    {
        //        devContext.School1TeacherSL.AppMarketService.Uninstall(appInstall.Id);
        //    }
        //    Assert.IsTrue(devContext.School1TeacherSL.AppMarketService.CanInstall(application.Id, devContext.School1Teacher.Id, null, null, null, null));
        //    appInfo.ApplicationAccessInfo.CanAttach = false;
        //    appInfo.ApplicationAccessInfo.HasStudentMyApps = false;
        //    appInfo.ApplicationAccessInfo.HasTeacherMyApps = false;

        //    application = devContext.School1AdminSL.ApplicationService.GetApplicationById(ApplicationServiceTest
        //        .CreateDefualt(devContext, out appInfo, ApplicationStateEnum.Live, ApplicationAccessInfo.Create(false, false, false, false, false, false),
        //         "app2", "http://app2.com", applicationPriceInfo).OriginalRef.Value);

        //    AssertException<Exception>(() => devContext.School1TeacherSL.AppMarketService.CanInstall(application.Id, null, null, null, null, null));
        //    AssertException<Exception>(() => devContext.School1StudentSL.AppMarketService.CanInstall(application.Id, devContext.School1Student.Id, null, null, null, null));

        //}


    }
}
