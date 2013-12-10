REQUIRE('chlk.controllers.BaseController');

REQUIRE('chlk.services.ApplicationService');
REQUIRE('chlk.services.AppCategoryService');
REQUIRE('chlk.services.GradeLevelService');
REQUIRE('chlk.services.AppMarketService');
REQUIRE('chlk.services.PictureService');

REQUIRE('chlk.activities.apps.AppsListPage');
REQUIRE('chlk.activities.apps.AppInfoPage');
REQUIRE('chlk.activities.apps.AppGeneralInfoPage');
REQUIRE('chlk.activities.apps.AddAppDialog');
REQUIRE('chlk.activities.apps.AppWrapperDialog');

REQUIRE('chlk.models.apps.Application');
REQUIRE('chlk.models.apps.AppPostData');
REQUIRE('chlk.models.apps.ShortAppInfo');
REQUIRE('chlk.models.apps.AppAccess');
REQUIRE('chlk.models.apps.AppPicture');
REQUIRE('chlk.models.apps.AppState');
REQUIRE('chlk.models.apps.AppScreenshots');
REQUIRE('chlk.models.apps.AppGeneralInfoViewData');
REQUIRE('chlk.models.apps.AppWrapperViewData');

REQUIRE('chlk.models.id.AppId');
REQUIRE('chlk.models.id.AppPermissionId');


NAMESPACE('chlk.controllers', function (){

    /** @class chlk.controllers.AppsController */
    CLASS(
        'AppsController', EXTENDS(chlk.controllers.BaseController), [

        [ria.mvc.Inject],
        chlk.services.ApplicationService, 'appsService',

        [ria.mvc.Inject],
        chlk.services.AppMarketService, 'appMarketService',

        [ria.mvc.Inject],
        chlk.services.AppCategoryService, 'categoryService',

        [ria.mvc.Inject],
        chlk.services.GradeLevelService, 'gradeLevelService',

        [ria.mvc.Inject],
        chlk.services.PictureService, 'pictureService',

        [chlk.controllers.SidebarButton('apps')],
        [[Number]],
        function listAction(pageIndex_) {
            var result = this.appsService
                .getApps(pageIndex_ | 0)
                .attach(this.validateResponse_());
            return this.PushView(chlk.activities.apps.AppsListPage, result);
        },

        [[Number]],
        function pageAction(pageIndex_) {
            var result = this.appsService
                .getApps(pageIndex_ | 0)
                .attach(this.validateResponse_());
            return this.UpdateView(chlk.activities.apps.AppsListPage, result);
        },


        [[chlk.models.apps.Application, Boolean, Boolean]],
        ria.async.Future, function prepareAppInfo(app_, readOnly, isDraft) {
            var result = this.categoryService
                .getCategories()
                .then(function(data){
                    var cats = data.getItems();
                    var gradeLevels = this.gradeLevelService.getGradeLevels();
                    var permissions = this.appsService.getAppPermissions();
                    var platforms = this.appsService.getAppPlatforms();
                    var appGradeLevels = app_.getGradeLevels();
                    if (!appGradeLevels) app_.setGradeLevels([]);
                    var appCategories = app_.getCategories();
                    if (!appCategories) app_.setCategories([]);
                    var appPlatforms = app_.getPlatforms();
                    if (!appPlatforms) app_.setPlatforms([]);

                    if (!app_.getState()){
                        var appState = new chlk.models.apps.AppState();
                        appState.deserialize(chlk.models.apps.AppStateEnum.DRAFT);
                        app_.setState(appState);
                    }
                    if (!app_.getAppAccess())
                        app_.setAppAccess(new chlk.models.apps.AppAccess());

                    var appIconId = app_.getSmallPictureId() || new chlk.models.id.PictureId('');
                    var iconUrl = this.pictureService.getPictureUrl(appIconId, 74);
                    app_.setIconPicture(new chlk.models.apps.AppPicture(appIconId, iconUrl, 74, 74, 'Icon', !readOnly));


                    var appBannerId = app_.getBigPictureId() || new chlk.models.id.PictureId('');
                    var bannerUrl = this.pictureService.getPictureUrl(appBannerId, 170, 110);
                    app_.setBannerPicture(new chlk.models.apps.AppPicture(appBannerId, bannerUrl, 170, 110, 'Banner', !readOnly));


                    var screenshots = app_.getScreenshotIds() || [];
                    var screenshotPictures = screenshots.map(function(pictureId){
                        var pictureUrl = this.pictureService.getPictureUrl(pictureId, 640, 390);
                        return new chlk.models.apps.AppPicture(pictureId, pictureUrl, 640, 390, 'screenshot', !readOnly);
                    }, this);

                    app_.setScreenshotPictures(new chlk.models.apps.AppScreenshots(screenshotPictures, readOnly));
		    

                    return new chlk.models.apps.AppInfoViewData(app_, readOnly, cats, gradeLevels, permissions, platforms, isDraft);

                }, this)
                .attach(this.validateResponse_());

            return result;
        },

        [chlk.controllers.AccessForRoles([
            chlk.models.common.RoleEnum.DEVELOPER
        ])],
        [[chlk.models.id.AppId, Boolean]],
        function detailsDeveloperAction(appId_, isSubmit_) {
            var isReadonly = false;
            var isDraft = !(!!isSubmit_);
            var app = this.appsService
                .getInfo(appId_)
                .then(function(data){
                    if (!data.getId()){
                        return this.Forward('apps', 'add', []);
                    }
                    else
                        return this.PushView(chlk.activities.apps.AppInfoPage, this.prepareAppInfo(data, isReadonly, isDraft));
                }, this)
                .attach(this.validateResponse_());
        },

        [chlk.controllers.AccessForRoles([
            chlk.models.common.RoleEnum.DEVELOPER
        ])],
        [[Number, Number, String, Object]],
        function uploadPictureDeveloperAction(width, height, msg, file) {
            var result = this.appsService
                .uploadPicture(file, width, height)
                .then(function(id){
                    var pictureUrl = this.pictureService.getPictureUrl(id, width, height);
                    return new chlk.models.apps.AppPicture(id, pictureUrl, width, height, msg, true);
                }, this)
                .attach(this.validateResponse_());
            return this.UpdateView(chlk.activities.apps.AppInfoPage, result, msg.toLowerCase());
        },

        [chlk.controllers.AccessForRoles([
            chlk.models.common.RoleEnum.DEVELOPER
        ])],
        [[String, Object]],
        function uploadScreenshotDeveloperAction(screenshots_, file) {
            var width = 640;
            var height = 390;
            var msg = "Screenshots";
            var result = this.appsService
                .uploadPicture(file, width, height)
                .then(function(id){
                    var screenshots = (screenshots_ || "").split(",");
                    screenshots.push(id.valueOf());
                    screenshots = screenshots
                        .filter(function(el){
                            return el.length > 0;
                        })
                        .map(function(el){
                            var pictureId = new chlk.models.id.PictureId(el);
                            var pictureUrl = this.pictureService.getPictureUrl(pictureId, width, height);
                            return new chlk.models.apps.AppPicture(pictureId, pictureUrl, width, height, msg, true);
                        }, this);
                    return new chlk.models.apps.AppScreenshots(screenshots, false);
                }, this)
                .attach(this.validateResponse_());
            return this.UpdateView(chlk.activities.apps.AppInfoPage, result, msg.toLowerCase());
        },



        [chlk.controllers.AccessForRoles([
            chlk.models.common.RoleEnum.DEVELOPER
        ])],
        [[Object]],
        function toggleBannerAction(data){
            if (data.bannerEnabled){
                var mdl = new chlk.models.apps.AppPicture(new chlk.models.id.PictureId(''), '', 170, 110, 'Banner', true);
                return this.UpdateView(chlk.activities.apps.AppInfoPage, ria.async.DeferredData(mdl), 'banner');
            }
        },

        [chlk.controllers.AccessForRoles([
            chlk.models.common.RoleEnum.DEVELOPER
        ])],
        [[String, Number, Number]],
        function deletePictureDeveloperAction(title, width, height) {
            var result = new ria.async.DeferredData(new chlk.models.apps.AppPicture(
                new chlk.models.id.PictureId(''), '#', width, height, title, true));
            return this.UpdateView(chlk.activities.apps.AppInfoPage, result, title.toLowerCase());
        },

        [chlk.controllers.AccessForRoles([
            chlk.models.common.RoleEnum.DEVELOPER
        ])],
        [[String, chlk.models.id.PictureId]],
        function deleteScreenshotDeveloperAction(screenshots, id)
        {
            var width = 640;
            var height = 390;
            var msg = "Screenshots";
            var screenshots = screenshots.split(',');
            screenshots = screenshots
                .filter(function(item){
                    return  item != id.valueOf()
                })
                .map(function(el){
                    var pictureId = new chlk.models.id.PictureId(el);
                    var pictureUrl = this.pictureService.getPictureUrl(pictureId, width, height);
                    return new chlk.models.apps.AppPicture(pictureId, pictureUrl, width, height, msg, true);
                }, this);
            var result = new ria.async.DeferredData(new chlk.models.apps.AppScreenshots(screenshots, false));
            return this.UpdateView(chlk.activities.apps.AppInfoPage, result, msg.toLowerCase());
        },



        [chlk.controllers.AccessForRoles([
            chlk.models.common.RoleEnum.SYSADMIN
        ])],
        [[chlk.models.id.AppId]],
        function approveSysAdminAction(appId) {
            return this.appsService
                .approveApp(appId)
                .then(function(data){
                    return this.Redirect('apps', 'list', []);
                }, this)
                .attach(this.validateResponse_());
        },

        [chlk.controllers.AccessForRoles([
            chlk.models.common.RoleEnum.SYSADMIN
        ])],
        [[chlk.models.id.AppId]],
        function declineSysAdminAction(appId) {
            return this.appsService
                .declineApp(appId)
                .then(function(data){
                    return this.Redirect('apps', 'list', []);
                }, this)
                .attach(this.validateResponse_());
        },

        [chlk.controllers.AccessForRoles([
            chlk.models.common.RoleEnum.DEVELOPER
        ])],
        [[chlk.models.id.AppId]],
        function goLiveDeveloperAction(appId) {
            return this.appsService
                .goLive(appId)
                .then(function(data){
                    return this.Redirect('apps', 'general', []);
                }, this)
                .attach(this.validateResponse_());
        },

        [chlk.controllers.AccessForRoles([
            chlk.models.common.RoleEnum.DEVELOPER
        ])],
        [[chlk.models.id.AppId, chlk.models.id.AppId]],
        function unlistDeveloperAction(liveAppId) {
            return this.appsService
                .unlist(liveAppId)
                .then(function(data){
                    return this.Redirect('apps', 'general', []);
                }, this)
                .attach(this.validateResponse_());
        },

        [chlk.controllers.AccessForRoles([
            chlk.models.common.RoleEnum.TEACHER
        ])],
        [[chlk.models.id.AnnouncementId, chlk.models.id.AppId]],
        function tryToAttachTeacherAction(announcementId, appId) {

            var result = this.appsService
                .addToAnnouncement(appId, announcementId)
                .then(function(app){
                    var apps = this.getContext().getSession().get('AnnoucementApplications', []);
                    apps.push(app);
                    this.getContext().getSession().set('AnnoucementApplications', apps);
                    app.setCurrentModeUrl(app.getEditUrl());
                    return new chlk.models.apps.AppWrapperViewData(app, chlk.models.apps.AppModes.EDIT);
                }, this)
                .attach(this.validateResponse_());
            return this.ShadeView(chlk.activities.apps.AppWrapperDialog, result);
        },

        [[String, String, chlk.models.apps.AppModes, chlk.models.id.AnnouncementApplicationId]],
        function viewAppAction(url, viewUrl, mode, announcementAppId_) {
            var result = this.appsService
                .getOauthCode(url)
                .then(function(code){

                    var appData = null;
                    if (mode == chlk.models.apps.AppModes.MYAPPSVIEW){
                        appData =  this.appMarketService.getMyAppByUrl(url);

                        var appAccess = appData.getAppAccess();

                        var hasMyAppsView = appAccess.isStudentMyAppsEnabled() && this.userInRole(chlk.models.common.RoleEnum.STUDENT) ||
                            appAccess.isTeacherMyAppsEnabled() && this.userInRole(chlk.models.common.RoleEnum.TEACHER) ||
                            appAccess.isAdminMyAppsEnabled() && this.userIsAdmin() ||
                            appAccess.isParentMyAppsEnabled() && this.userInRole(chlk.models.common.RoleEnum.PARENT);
                        appAccess.setMyAppsForCurrentRoleEnabled(hasMyAppsView);
                        appData.setAppAccess(appAccess);

                    }
                    var app = new chlk.models.apps.AppAttachment.$create(
                        viewUrl,
                        code,
                        announcementAppId_,
                        appData
                    );
                    return new chlk.models.apps.AppWrapperViewData(app, mode);
                }, this)
                .attach(this.validateResponse_());
            return this.ShadeView(chlk.activities.apps.AppWrapperDialog, result);
        },

        [chlk.controllers.AccessForRoles([
            chlk.models.common.RoleEnum.SYSADMIN
        ])],
        [[chlk.models.id.AppId]],
        function detailsSysAdminAction(appId) {
            var isReadonly = true;
            var app = this.appsService
                .getInfo(appId)
                .then(function(data){
                        return this.PushView(chlk.activities.apps.AppInfoPage, this.prepareAppInfo(data, isReadonly, true));
                }, this)
                .attach(this.validateResponse_());
        },

        [chlk.controllers.AccessForRoles([
            chlk.models.common.RoleEnum.DEVELOPER
        ])],
        function addDeveloperAction(){
           var app = new chlk.models.apps.Application();
           return this.PushView(chlk.activities.apps.AddAppDialog, new ria.async.DeferredData(app));
        },

        [chlk.controllers.AccessForRoles([
            chlk.models.common.RoleEnum.DEVELOPER
        ])],

        [[chlk.models.apps.Application]],
        function createDeveloperAction(model){
            var devId = this.getCurrentPerson().getId();
            return this.appsService
                .createApp(devId, model.getName())
                .then(function(){
                    return this.Forward('apps', 'details', []);
                }, this)
                .attach(this.validateResponse_());
        },

        [chlk.controllers.AccessForRoles([
            chlk.models.common.RoleEnum.DEVELOPER
        ])],
        [[chlk.models.id.AppId]],
        function deleteDeveloperAction(id) {
            return this.appsService
                .deleteApp(id)
                .then(function(){
                    return this.Forward('apps', 'details', []);
                }, this)
                .attach(this.validateResponse_());
        },

        [chlk.controllers.AccessForRoles([
            chlk.models.common.RoleEnum.DEVELOPER
        ])],
        [[chlk.models.apps.Application]],
        function updateApp(app) {
            return this.Forward('apps', 'details', [app.getId().valueOf(), true]);
        },

        [chlk.controllers.AccessForRoles([
            chlk.models.common.RoleEnum.DEVELOPER
        ])],
        [[chlk.models.id.AppId, String]],
        function tryDeleteApplicationAction(id, appName) {
            var msgText = "You are about to delete " + appName + " application.\n\n This can not be undone.";

            return this.ShowMsgBox(msgText, "whoa.", [{
                text: "Cancel",
                color: chlk.models.common.ButtonColor.GREEN.valueOf()
            }, {
                text: 'Delete',
                controller: 'apps',
                action: 'delete',
                params: [id.valueOf()],
                color: chlk.models.common.ButtonColor.RED.valueOf()
            }], 'center');
        },

        [chlk.controllers.AccessForRoles([
            chlk.models.common.RoleEnum.DEVELOPER
        ])],
        [[chlk.models.apps.AppPostData]],
        function updateDeveloperAction(model){




            var appAccess = new chlk.models.apps.AppAccess(
                model.isStudentMyAppsEnabled(),
                model.isTeacherMyAppsEnabled(),
                model.isAdminMyAppsEnabled(),
                model.isParentMyAppsEnabled(),
                model.isAttachEnabled(),
                model.isShowInGradingViewEnabled()
            );

             var shortAppData = new chlk.models.apps.ShortAppInfo(
                model.getName(),
                model.getUrl(),
                model.getVideoDemoUrl(),
                model.getShortDescription(),
                model.getLongDescription(),
                model.getAppIconId(),
                model.getAppBannerId()
             );

             var isFreeApp = model.isFree();

             var isSchoolFlatRateEnabled = model.isSchoolFlatRateEnabled();
             var isClassFlatRateEnabled = model.isSchoolFlatRateEnabled();

             var appPriceInfo = isFreeApp ? new chlk.models.apps.AppPrice()
                                          :
                                            new chlk.models.apps.AppPrice(
                                                model.getCostPerUser(),
                                                isSchoolFlatRateEnabled ? model.getCostPerClass() : null,
                                                isClassFlatRateEnabled  ? model.getCostPerSchool(): null
                                            );
            var cats = this.getIdsList(model.getCategories(), chlk.models.id.AppCategoryId);
            var gradeLevels = this.getIdsList(model.getGradeLevels(), chlk.models.id.GradeLevelId);
            var appPermissions = this.getIdsList(model.getPermissions(), chlk.models.apps.AppPermissionTypeEnum);
            var appScreenShots = this.getIdsList(model.getAppScreenshots(), chlk.models.id.PictureId);
            var appPlatforms = this.getIdsList(model.getPlatforms(), chlk.models.apps.AppPlatformTypeEnum);

            if (!model.isDraft()){
                var appIconId = null;

                if (model.getAppIconId()){
                    appIconId = model.getAppIconId().valueOf();
                    if (appIconId.length == 0) appIconId = null;
                }

                var appBannerId = null;
                if (model.getAppBannerId()){
                    appBannerId = model.getAppBannerId().valueOf();

                    if (appBannerId.length == 0) appBannerId = null;
                }

                if (appAccess.isAttachEnabled()){
                    if (appIconId == null || appBannerId == null){
                        return this.ShowMsgBox('You need to upload icon and banner picture for you app', 'Error', [{
                            text: 'Ok'
                        }], 'center');
                    }
                }
                else
                {
                    if (appIconId == null){
                        return this.ShowMsgBox('You need to upload icon picture for you app', 'Error', [{
                            text: 'Ok'
                        }], 'center');
                    }
                }

                var developerWebsite = model.getDeveloperWebSite();
                var developerName = model.getDeveloperName();

                if ((developerWebsite == null || developerWebsite == "") ||
                    (developerName == null || developerName == "")){
                    return this.ShowMsgBox('We just need you to enter your Developer Name and Website. You can do that in settings, on the left.', null, [{
                        text: 'Ok'
                    }], 'center');
                }
            }

             var result = this.appsService
                 .updateApp(
                     model.getId(),
                     shortAppData,
                     appPermissions,
                     appPriceInfo,
                     this.getCurrentPerson().getId(),
                     appAccess,
                     cats,
                     appScreenShots,
                     gradeLevels,
                     appPlatforms,
                     !model.isDraft()
                 )
                 .then(function(newApp){
                     return this.updateApp(newApp);
                 }, this)
                 .attach(this.validateResponse_())
             return result;
        },

        [chlk.controllers.AccessForRoles([
            chlk.models.common.RoleEnum.DEVELOPER
        ])],

        [[chlk.models.id.AppId]],
        function generalDeveloperAction(){
            return this.appsService
                .getInfo()
                .then(function(data){
                    if (!data.getId()){
                        return this.Forward('apps', 'add', []);
                    }
                    else{
                        var pictureUrl = this.pictureService.getPictureUrl(data.getSmallPictureId(), 74);
                        var isAppLive = data.getLiveAppId() != null && data.getLiveAppId().valueOf() != "";
                        var isApproved = data.getState().getStateId() == chlk.models.apps.AppStateEnum.APPROVED;

                        var appInfo = new chlk.models.apps.AppGeneralInfoViewData(
                            data.getName(),
                            data.getId(),
                            data.getLiveAppId(),
                            data.getState().toString(isAppLive),
                            isApproved,
                            pictureUrl,
                            new chlk.models.apps.AppRating()
                        );
                        return this.PushView(chlk.activities.apps.AppGeneralInfoPage, new ria.async.DeferredData(appInfo));
                    }
                }, this)
                .attach(this.validateResponse_());
        }
    ])
});
