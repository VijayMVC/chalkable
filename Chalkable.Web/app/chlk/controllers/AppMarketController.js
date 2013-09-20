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
REQUIRE('chlk.models.apps.AppMarketDetailsViewData');
REQUIRE('chlk.models.apps.AppMarketViewData');
REQUIRE('chlk.models.apps.AppInstallGroup');
REQUIRE('chlk.models.apps.AppInstallPostData');
REQUIRE('chlk.models.apps.AppDeletePostData');

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

        [chlk.controllers.SidebarButton('apps')],
        [chlk.controllers.AccessForRoles([
            chlk.models.common.RoleEnum.TEACHER,
            chlk.models.common.RoleEnum.ADMINEDIT,
            chlk.models.common.RoleEnum.ADMINGRADE
        ])],
        [chlk.controllers.SidebarButton('apps')],
        function myAppsAction() {
            var result = this.appMarketService
                .getInstalledApps(this.getCurrentPerson().getId())
                .then(function(apps){
                    return new chlk.models.apps.MyAppsViewData(apps, false);
                })
                .attach(this.validateResponse_());
            return this.PushView(chlk.activities.apps.MyAppsPage, result);
        },



        [chlk.controllers.SidebarButton('apps')],
        [chlk.controllers.AccessForRoles([
            chlk.models.common.RoleEnum.TEACHER,
            chlk.models.common.RoleEnum.ADMINEDIT,
            chlk.models.common.RoleEnum.ADMINGRADE
        ])],
        [chlk.controllers.SidebarButton('apps')],
        [[Boolean]],
        function updateMyAppsAction(isEdit) {
            var result = this.appMarketService
                .getInstalledApps(this.getCurrentPerson().getId())
                .then(function(data){

                    var apps = data.getItems() || [];

                    apps = apps.map(function(app){
                        var appInstalls = app.getApplicationInstalls() || [];
                        app.setSelfInstalled(false);

                        var currentPersonId = this.getCurrentPerson().getId();
                        var uninstallAppIds = [];

                        appInstalls.forEach(function(appInstall){
                            if (appInstall.isOwner() && isEdit){
                                uninstallAppIds.push(appInstall.getAppInstallId());
                                if(appInstall.getPersonId() == appInstall.getInstallationOwnerId()){
                                    app.setSelfInstalled(true);
                                }
                            }
                            if (appInstall.getPersonId() == currentPersonId){
                                app.setPersonal(true);
                            }

                        });
                        app.setUninstallable(isEdit && uninstallAppIds.length > 0);
                        var ids = uninstallAppIds.map(function(item){
                            return item.valueOf()
                        }).join(',');
                        app.setApplicationInstallIds(ids);
                        return app;
                    }, this);


                    data.setItems(apps);

                    return new chlk.models.apps.MyAppsViewData(data, isEdit);
                }, this)
                .attach(this.validateResponse_());
            return this.UpdateView(chlk.activities.apps.MyAppsPage, result);
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

                    var installBtnTitle ='';

                    if (this.userInRole(chlk.models.common.RoleEnum.STUDENT) && app.isInstalledOnlyForCurrentUser()){
                        installBtnTitle = 'Installed';
                    }else{
                        installBtnTitle = app.getApplicationPrice().getPrice() > 0 ? "$" + app.getApplicationPrice().getPrice() : "Free";
                    }
                    return new chlk.models.apps.AppMarketDetailsViewData(app, installBtnTitle);
                }, this)
                .attach(this.validateResponse_());
            return this.PushView(chlk.activities.apps.AppMarketDetailsPage, result);
        },

        [chlk.controllers.SidebarButton('apps')],
        [[Number]],
        function pageAction(pageIndex_) {
            var result = this.appMarketService
                .getApps(pageIndex_ | 0)
                .attach(this.validateResponse_());
            return this.UpdateView(chlk.activities.apps.AppMarketPage, result);
        },



        [chlk.controllers.SidebarButton('apps')],
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

                    var installedCount = 0;

                    installedForGroups = installedForGroups.map(function(item){
                        if (item.getGroupType() == chlk.models.apps.AppInstallGroupTypeEnum.ALL
                            && this.userInRole(chlk.models.common.RoleEnum.TEACHER))
                            item.setDescription('Whole School');
                        if (item.isInstalled()) ++installedCount;
                        return item;
                    }, this);
                    app.setInstalledForGroups(installedForGroups);
                    return new chlk.models.apps.AppMarketInstallViewData(app, installedCount == installedForGroups.length);
                }, this)
                .attach(this.validateResponse_())

            return this.ShadeView(chlk.activities.apps.InstallAppDialog, appInfo);
        },

        [chlk.controllers.SidebarButton('apps')],
        [chlk.controllers.AccessForRoles([
            chlk.models.common.RoleEnum.TEACHER,
            chlk.models.common.RoleEnum.ADMINEDIT,
            chlk.models.common.RoleEnum.ADMINGRADE
        ])],
        [[chlk.models.apps.AppInstallPostData]],
        function installAction(appInstallData) {
            return this.appMarketService
                .installApp(
                    appInstallData.getAppId(),
                    this.getIdsList(appInstallData.getDepartments(), chlk.models.id.AppInstallGroupId),
                    this.getIdsList(appInstallData.getClasses(), chlk.models.id.AppInstallGroupId),
                    this.getIdsList(appInstallData.getRoles(), chlk.models.id.AppInstallGroupId),
                    this.getIdsList(appInstallData.getGradeLevels(), chlk.models.id.AppInstallGroupId),
                    appInstallData.getCurrentPerson()
                )
                .then(function(result){
                    var title = result ? "Installation successful" : "Error while installing app.";
                       return this.ShowMsgBox(title, '', [{
                           text: 'Ok',
                           controller: 'appmarket',
                           action: 'list',
                           params: [],
                           color: chlk.models.common.ButtonColor.GREEN.valueOf()
                       }], 'center');
                }, this)
                .attach(this.validateResponse_());
        },

        [chlk.controllers.SidebarButton('apps')],
        [chlk.controllers.AccessForRoles([
            chlk.models.common.RoleEnum.TEACHER,
            chlk.models.common.RoleEnum.ADMINEDIT,
            chlk.models.common.RoleEnum.ADMINGRADE
        ])],
        [[String]],
        function uninstallAction(ids) {
            return this.appMarketService
                .uninstallApps(ids)
                .then(function(result){
                    return this.Redirect('appmarket', 'updateMyApps', [false]);
                }, this)
                .attach(this.validateResponse_());
        },

        [chlk.controllers.SidebarButton('apps')],
        [chlk.controllers.AccessForRoles([
            chlk.models.common.RoleEnum.TEACHER,
            chlk.models.common.RoleEnum.ADMINEDIT,
            chlk.models.common.RoleEnum.ADMINGRADE
        ])],

        [[chlk.models.apps.AppDeletePostData]],
        function tryToUninstallAction(data){
            if(!data.isUninstallable()){
                this.ShowMsgBox(
                    'Sorry you can\'t uninstall app. You are not owner of this app.',
                    'Oops.', [{
                        text: 'Ok',
                        color: chlk.models.common.ButtonColor.GREEN.valueOf()
                    }]
                );
            }
            else{
                var additionalMsg = ''
                if(!data.isSelfInstalled()){
                    additionalMsg = 'NOTE: This app is not self installed.';
                }
                var msgTitle = 'Uninstall the ' + data.getAppName() + ' app?' + additionalMsg;

                return this.ShowMsgBox(msgTitle, 'just checking.', [{
                    text: 'CANCEL',
                    color: chlk.models.common.ButtonColor.GREEN.valueOf()
                }, {
                    text: 'UNINSTALL',
                    controller: 'appmarket',
                    action: 'uninstall',
                    params: [data.getApplicationInstallIds()],
                    color: chlk.models.common.ButtonColor.RED.valueOf()
                }], 'center');

            }
        }
    ])
});
