using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chalkable.BusinessLogic.Model;
using Chalkable.BusinessLogic.Services.Master;
using Chalkable.Data.Common.Enums;
using Chalkable.Data.Master.Model;
using NUnit.Framework;

namespace Chalkable.Tests.Services.Master
{
    public class ApplicationServiceTest : MasterServiceTestBase
    {
        [Test]
        public void CreateTest()
        {
            var context = CreateDeveloperSchoolTestContext();
            //TODO: pictures upload test

            var appInfo = PrepareDefaultAppInfo(SysAdminMasterLocator, context.Developer.Id, "app1", "http://test.app1.com");
            //security check 
            AssertException<Exception>(() => context.AdminGradeSl.ServiceLocatorMaster.ApplicationUploadService.Create(appInfo));
            AssertException<Exception>(() => context.AdminEditSl.ServiceLocatorMaster.ApplicationUploadService.Create(appInfo));
            AssertException<Exception>(() => context.AdminViewSl.ServiceLocatorMaster.ApplicationUploadService.Create(appInfo));
            AssertException<Exception>(() => context.FirstTeacherSl.ServiceLocatorMaster.ApplicationUploadService.Create(appInfo));
            AssertException<Exception>(() => context.FirstStudentSl.ServiceLocatorMaster.ApplicationUploadService.Create(appInfo));

            var application = context.DeveloperMl.ApplicationUploadService.Create(appInfo);
            Assert.AreEqual(application.State, ApplicationStateEnum.Draft);
            CheckApplication(appInfo, application);
            AssertAreEqual(application, context.DeveloperMl.ApplicationService.GetApplicationById(application.Id));
            Assert.AreEqual(context.DeveloperMl.ApplicationService.GetApplications().Count, 1);

            appInfo.ShortApplicationInfo.Name = null;
            AssertException<Exception>(() => context.DeveloperMl.ApplicationUploadService.Create(appInfo));
            //check on dublication names 
            appInfo.ShortApplicationInfo.Name = "app1";
            appInfo.ShortApplicationInfo.Url = "http://test.app2.com";
            AssertException<Exception>(() => context.DeveloperMl.ApplicationUploadService.Create(appInfo));
            
            //check valid price param
            appInfo.ShortApplicationInfo.Name = "app2";
            appInfo.ShortApplicationInfo.Url = "http://test.app2.com";
            appInfo.ApplicationPrices = ApplicationPricesInfo.Create(-1, -2, -3);
            AssertException<Exception>(() => context.DeveloperMl.ApplicationUploadService.Create(appInfo));
                   
        }
        
        [Test]
        public void UpdateDraftTest()
        {
            var context = CreateDeveloperSchoolTestContext();
            var appInfo = PrepareDefaultAppInfo(SysAdminMasterLocator, context.Developer.Id, "app1", "http://test.app1.com");
            var app = context.DeveloperMl.ApplicationUploadService.Create(appInfo);
            appInfo.ShortApplicationInfo.Name = "app2";
            appInfo.ShortApplicationInfo.Url = "http://test.app2.com";
            appInfo.ShortApplicationInfo.Description = "testDec2";
            appInfo.ShortApplicationInfo.ShortDescription = "testShortDec2";
            appInfo.GradeLevels.RemoveAt(0);
            appInfo.PermissionIds.RemoveAt(0);
            appInfo.Categories.RemoveAt(0);
            appInfo.ApplicationPrices = ApplicationPricesInfo.Create(2, 40, 2000);
            appInfo.ApplicationAccessInfo = ApplicationAccessInfo.Create(false, false, false, false, true, true);

            //security test 
            AssertException<Exception>(() => context.AdminGradeSl.ServiceLocatorMaster.ApplicationUploadService.UpdateDraft(app.Id, appInfo));
            AssertException<Exception>(() => context.AdminEditSl.ServiceLocatorMaster.ApplicationUploadService.UpdateDraft(app.Id, appInfo));
            AssertException<Exception>(() => context.AdminViewSl.ServiceLocatorMaster.ApplicationUploadService.UpdateDraft(app.Id, appInfo));
            AssertException<Exception>(() => context.FirstTeacherSl.ServiceLocatorMaster.ApplicationUploadService.UpdateDraft(app.Id, appInfo));
            AssertException<Exception>(() => context.FirstStudentSl.ServiceLocatorMaster.ApplicationUploadService.UpdateDraft(app.Id, appInfo));

            app = context.DeveloperMl.ApplicationUploadService.UpdateDraft(app.Id, appInfo);
            Assert.AreEqual(app.State, ApplicationStateEnum.Draft);
            CheckApplication(appInfo, app);
            AssertAreEqual(app, context.DeveloperMl.ApplicationService.GetApplicationById(app.Id));

            //check valid name, url
            appInfo.ShortApplicationInfo.Name = null;
            AssertException<Exception>(() => context.DeveloperMl.ApplicationUploadService.UpdateDraft(app.Id, appInfo));
            appInfo.ShortApplicationInfo.Name = "app2";
            appInfo.ShortApplicationInfo.Url = null;
            AssertException<Exception>(() => context.DeveloperMl.ApplicationUploadService.UpdateDraft(app.Id, appInfo));

            // check name , url dublicating
            appInfo.ShortApplicationInfo.Name = "app1";
            var app2 = context.DeveloperMl.ApplicationUploadService.Create(appInfo);
            appInfo.ShortApplicationInfo.Url = "http://test.app2.com";
            Assert.IsTrue(context.DeveloperMl.ApplicationUploadService.Exists(app2.Id, 
                appInfo.ShortApplicationInfo.Name, appInfo.ShortApplicationInfo.Url));
            AssertException<Exception>(() => context.DeveloperMl.ApplicationUploadService.UpdateDraft(app2.Id, appInfo));
            appInfo.ShortApplicationInfo.Name = "app2";
            appInfo.ShortApplicationInfo.Url = "http://test.app1.com";
            Assert.IsTrue(context.DeveloperMl.ApplicationUploadService.Exists(app2.Id,
                appInfo.ShortApplicationInfo.Name, appInfo.ShortApplicationInfo.Url));
            AssertException<Exception>(() => context.DeveloperMl.ApplicationUploadService.UpdateDraft(app2.Id, appInfo));
         
            //TODO: test access other developers to current app ... (fix developerSchoolContext)
        }

        [Test]
        public void SubmitTest()
        {
            
        }

        [Test]
        public void ApproveRejectTest()
        {
            var context = CreateDeveloperSchoolTestContext();
            var appInfo = PrepareDefaultAppInfo(SysAdminMasterLocator, context.Developer.Id, "app1", "http://test.app1.com");
            var app = context.DeveloperMl.ApplicationUploadService.Create(appInfo);

            //security test
            AssertException<Exception>(() => context.AdminGradeSl.ServiceLocatorMaster.ApplicationUploadService.ApproveReject(app.Id, true));
            AssertException<Exception>(() => context.AdminEditSl.ServiceLocatorMaster.ApplicationUploadService.ApproveReject(app.Id, true));
            AssertException<Exception>(() => context.AdminViewSl.ServiceLocatorMaster.ApplicationUploadService.ApproveReject(app.Id, true));
            AssertException<Exception>(() => context.FirstTeacherSl.ServiceLocatorMaster.ApplicationUploadService.ApproveReject(app.Id, true));
            AssertException<Exception>(() => context.FirstStudentSl.ServiceLocatorMaster.ApplicationUploadService.ApproveReject(app.Id, true));
            AssertException<Exception>(() => context.DeveloperMl.ApplicationUploadService.ApproveReject(app.Id, true));

            Assert.IsFalse(SysAdminMasterLocator.ApplicationUploadService.ApproveReject(app.Id, true));
            app = context.DeveloperMl.ApplicationUploadService.Submit(app.Id, appInfo);
            Assert.IsTrue(SysAdminMasterLocator.ApplicationUploadService.ApproveReject(app.Id, true));
            app = context.DeveloperMl.ApplicationService.GetApplicationById(app.Id);
            Assert.AreEqual(app.State, ApplicationStateEnum.Approved);
            Assert.IsFalse(SysAdminMasterLocator.ApplicationUploadService.ApproveReject(app.Id, false));
          
            app = context.DeveloperMl.ApplicationUploadService.Submit(app.Id, appInfo);
            Assert.IsTrue(SysAdminMasterLocator.ApplicationUploadService.ApproveReject(app.Id, false));
            app = context.DeveloperMl.ApplicationService.GetApplicationById(app.Id);
            Assert.AreEqual(app.State, ApplicationStateEnum.Rejected);


        }

        [Test]
        public void GoLiveUnListTest()
        {
                        
        }

        public static Application CreateDefaultDraftApp(IServiceLocatorMaster sysLocator, DeveloperSchoolTestContex developerContext, string name, string url)
        {
            var appInfo = PrepareDefaultAppInfo(sysLocator, developerContext.Developer.Id, name, url);
            return developerContext.DeveloperMl.ApplicationUploadService.Create(appInfo);
        }
        public static BaseApplicationInfo PrepareDefaultAppInfo(IServiceLocatorMaster sysLocator, Guid developerId, string name, string url)
        {
            var shortAppInfo = ShortApplicationInfo.Create(name, url, "test_short_desc",
                               "test_desc", "http://test.app.video.com", Guid.NewGuid(), Guid.NewGuid());

            var appPrice = ApplicationPricesInfo.Create(1, 30, 1000);
            var appAccess = ApplicationAccessInfo.Create(true, true, false, false, true, true);
            var appPermissionTypes = new List<AppPermissionType>
                {
                    AppPermissionType.Grade,
                    AppPermissionType.User,
                    AppPermissionType.Announcement
                };
            var categoriesIds = CategoryServiceTest.CreateDefaultCategories(sysLocator).Select(x => x.Id).ToList();
            var gradeLevels = new List<int> { 1, 2, 3, 4, 5 };
            return BaseApplicationInfo.Create(shortAppInfo, developerId, appPermissionTypes, null, appPrice,
                categoriesIds, appAccess, gradeLevels);
        }
        
        private static void CheckApplication(BaseApplicationInfo appInfo, Application app)
        {
            Assert.AreEqual(appInfo.ShortApplicationInfo.Name, app.Name);
            Assert.AreEqual(appInfo.ShortApplicationInfo.Description, app.Description);
            Assert.AreEqual(appInfo.ShortApplicationInfo.ShortDescription, app.ShortDescription);
            Assert.AreEqual(appInfo.ShortApplicationInfo.Url, app.Url);
            Assert.AreEqual(appInfo.ShortApplicationInfo.SmallPictureId, app.SmallPictureRef);
            Assert.AreEqual(appInfo.ShortApplicationInfo.BigPictureId, app.BigPictureRef);
            Assert.AreEqual(appInfo.ShortApplicationInfo.VideoDemoUrl, app.VideoUrl);
            Assert.AreEqual(appInfo.ApplicationPrices.Price, app.Price);
            Assert.AreEqual(appInfo.ApplicationPrices.PricePerClass, app.PricePerClass);
            Assert.AreEqual(appInfo.ApplicationPrices.PricePerSchool, app.PricePerSchool);
            Assert.AreEqual(appInfo.DeveloperId, app.DeveloperRef);
            Assert.AreEqual(appInfo.ApplicationAccessInfo.HasAdminMyApps, app.HasAdminMyApps);
            Assert.AreEqual(appInfo.ApplicationAccessInfo.HasTeacherMyApps, app.HasTeacherMyApps);
            Assert.AreEqual(appInfo.ApplicationAccessInfo.HasStudentMyApps, app.HasStudentMyApps);
            Assert.AreEqual(appInfo.ApplicationAccessInfo.HasParentMyApps, app.HasParentMyApps);
            Assert.AreEqual(appInfo.ApplicationAccessInfo.CanAttach, app.CanAttach);
            Assert.AreEqual(appInfo.ApplicationAccessInfo.ShowInGradeView, app.ShowInGradeView);

            if (appInfo.Categories == null)
                Assert.IsTrue(app.Categories == null || app.Categories.Count == 0);
            else
            {
                IList<Guid> c1 = appInfo.Categories.OrderBy(x => x).ToList();
                IList<Guid> c2 = app.Categories.OrderBy(x => x.CategoryRef).Select(x => x.CategoryRef).ToList();
                AssertAreEqual(c1, c2);
            }

            if (appInfo.PicturesId == null)
                Assert.IsTrue(app.Pictures == null || app.Pictures.Count == 0);
            else
            {
                IList<Guid> p1 = appInfo.PicturesId.OrderBy(x => x).ToList();
                IList<Guid> p2 = app.Pictures.OrderBy(x => x.Id).Select(x => x.Id).ToList();
                AssertAreEqual(p1, p2);
            }

            if (appInfo.PermissionIds == null)
                Assert.IsTrue(app.Permissions == null || app.Permissions.Count == 0);
            else
            {
                IList<int> perm1 = appInfo.PermissionIds.OrderBy(x => x).Select(x=>(int)x).ToList();
                IList<int> perm2 = app.Permissions.OrderBy(x => x.Permission).Select(x => (int)x.Permission).ToList();
                AssertAreEqual(perm1, perm2);
            }

            if (appInfo.GradeLevels == null)
                Assert.IsTrue(app.GradeLevels == null || app.GradeLevels.Count == 0);
            else
            {
                IList<int> g1 = appInfo.GradeLevels.OrderBy(x => x).ToList();
                IList<int> g2 = app.GradeLevels.OrderBy(x => x.GradeLevel).Select(x => x.GradeLevel).ToList();
                AssertAreEqual(g1, g2);
            }
        }
    }
}
