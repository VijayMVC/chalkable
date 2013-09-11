REQUIRE('chlk.controllers.BaseController');

REQUIRE('chlk.services.AppMarketService');
REQUIRE('chlk.services.AppCategoryService');
REQUIRE('chlk.services.GradeLevelService');
REQUIRE('chlk.services.PictureService');

REQUIRE('chlk.activities.apps.AppMarketPage');
REQUIRE('chlk.activities.apps.AppMarketDetailsPage');
REQUIRE('chlk.activities.apps.MyAppsPage');
REQUIRE('chlk.activities.apps.AttachAppDialog');
REQUIRE('chlk.activities.apps.InstallAppDialog');


REQUIRE('chlk.models.apps.AppMarketInstallViewData');
REQUIRE('chlk.models.apps.AppMarketViewData');
REQUIRE('chlk.models.apps.AppInstallGroup');

REQUIRE('chlk.models.id.AppId');

NAMESPACE('chlk.controllers', function (){

    /** @class chlk.controllers.AppMarketController */
    CLASS(
        'AppMarketController', EXTENDS(chlk.controllers.BaseController), [

        [ria.mvc.Inject],
        chlk.services.AppMarketService, 'appMarketService',

        [ria.mvc.Inject],
        chlk.services.AppCategoryService, 'appCategoryService',

        [ria.mvc.Inject],
        chlk.services.GradeLevelService, 'gradeLevelService',

        [ria.mvc.Inject],
        chlk.services.PictureService, 'pictureService',

        [chlk.controllers.SidebarButton('apps')],

        [[Number]],
        function listAction(pageIndex_) {
            var result = this.appCategoryService
                .getCategories()
                .then(function(data){
                    return data.getItems();
                })
                .then(function(categories){
                    return this.appMarketService
                        .getApps(
                            categories,
                            this.gradeLevelService.getGradeLevels(),
                            ""
                        );
                }, this)
                .then(function(apps){
                    var items = apps.getItems();
                    items = items.map(function(app){
                        var screenshots = this.pictureService.getAppPicturesByIds(app.getScreenshotIds(), 640, 390);
                        app.setScreenshotPictures(new chlk.models.apps.AppScreenshots(screenshots, false));
                        return app;
                    }, this);
                    apps.setItems(items);
                    return new chlk.models.apps.AppMarketViewData(apps);
                }, this)
                .attach(this.validateResponse_());
            return this.PushView(chlk.activities.apps.AppMarketPage, result);
        },

        function myAppsAction() {
            var result = this.appCategoryService
                .getCategories()
                .then(function(data){
                    return data.getItems();
                })
                .then(function(categories){
                    return this.appMarketService
                        .getApps(
                            categories,
                            this.gradeLevelService.getGradeLevels(),
                            ""
                        );
                }, this)
                .attach(this.validateResponse_());
            return this.PushView(chlk.activities.apps.MyAppsPage, result);
        },

        [chlk.controllers.SidebarButton('apps')],
        [[chlk.models.id.AppId]],
        function detailsAction(id) {
            var result = this.appMarketService
                .getDetails(id)
                .then(function(app){
                    var screenshots = this.pictureService.getAppPicturesByIds(app.getScreenshotIds(), 640, 390);
                    app.setScreenshotPictures(new chlk.models.apps.AppScreenshots(screenshots, false));

                    var appPermissions = app.getPermissions() || [];

                    appPermissions = appPermissions.map(function(permission){
                         switch(permission.getId().valueOf()){
                             case chlk.models.apps.AppPermissionTypeEnum.MESSAGE.valueOf() : permission.setName("Messaging");
                                 break;
                             case chlk.models.apps.AppPermissionTypeEnum.GRADE.valueOf(): permission.setName("Grading");
                                 break;
                         }
                         return permission;
                    });

                    var filteredPermissions = appPermissions.filter(function(permission){
                        return permission.getId() != chlk.models.apps.AppPermissionTypeEnum.USER
                            && permission.getId() != chlk.models.apps.AppPermissionTypeEnum.ANNOUNCEMENT
                            && permission.getId() != chlk.models.apps.AppPermissionTypeEnum.CLAZZ;
                    });

                     if (appPermissions.length > filteredPermissions.length){
                         filteredPermissions.unshift(new chlk.models.apps.AppPermission(
                             chlk.models.apps.AppPermissionTypeEnum.UNKNOWN, 'Basic info'));
                     }

                    app.setPermissions(filteredPermissions);
                    return app;
                }, this)
                .attach(this.validateResponse_());
            return this.PushView(chlk.activities.apps.AppMarketDetailsPage, result);
        },


        [[Number]],
        function pageAction(pageIndex_) {
            var result = this.appMarketService
                .getApps(pageIndex_ | 0)
                .attach(this.validateResponse_());
            return this.UpdateView(chlk.activities.apps.AppMarketPage, result);
        },




        [chlk.controllers.AccessForRoles([
            chlk.models.common.RoleEnum.TEACHER,
            chlk.models.common.RoleEnum.ADMINEDIT,
            chlk.models.common.RoleEnum.ADMINGRADE
        ])],
        [[chlk.models.id.AppId]],
        function tryToInstallAction(appId) {
            var appInfo = this.appMarketService
                .getDetails(appId)
                .then(function(app){
                    var installedForGroups = app.getInstalledForGroups() || [];
                    if (!this.userInRole(chlk.models.common.RoleEnum.STUDENT)){
                        installedForGroups.unshift(new chlk.models.apps.AppInstallGroup(
                            new chlk.models.id.AppInstallGroupId(this.getCurrentPerson().getId().valueOf()),
                            chlk.models.apps.AppInstallGroupTypeEnum.CURRENT_USER,
                            app.isInstalledOnlyForCurrentUser(),
                            "Just me"
                        ));
                    }
                    app.setInstalledForGroups(installedForGroups);
                    return new chlk.models.apps.AppMarketInstallViewData(app);
                }, this)
                .attach(this.validateResponse_())

            return this.ShadeView(chlk.activities.apps.InstallAppDialog, appInfo);
        },

        [chlk.controllers.AccessForRoles([
            chlk.models.common.RoleEnum.TEACHER,
            chlk.models.common.RoleEnum.ADMINEDIT,
            chlk.models.common.RoleEnum.ADMINGRADE
        ])],
        [[chlk.models.id.AppId]],
        function installAction(appId) {

        }
    ])
});
