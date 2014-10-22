REQUIRE('chlk.controllers.BaseController');

REQUIRE('chlk.services.ApplicationService');
REQUIRE('chlk.services.AppCategoryService');
REQUIRE('chlk.services.GradeLevelService');
REQUIRE('chlk.services.AppMarketService');
REQUIRE('chlk.services.PictureService');
REQUIRE('chlk.services.DeveloperService');

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
REQUIRE('chlk.models.developer.HomeAnalytics');
REQUIRE('chlk.models.apps.AppPersonReviewPostData');
REQUIRE('chlk.models.apps.GetAppsPostData');

REQUIRE('chlk.models.apps.AppsListViewData');

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

        [ria.mvc.Inject],
        chlk.services.DeveloperService, 'developerService',

        [chlk.controllers.SidebarButton('apps')],
        [[chlk.models.apps.GetAppsPostData]],
        function listAction(postData_) {
            if(postData_)
                return this.getApplications_(
                    postData_.getStart(),
                    postData_.getDeveloperId(),
                    postData_.getState(),
                    postData_.getFilter()
                );
            return this.getApplications_();
        },

        [[chlk.models.id.SchoolPersonId, Number, String, Number]],
        function pageAction(developerId_, state_, filter_, start_) {
//            var result = this.appsService
//                .getApps(pageIndex_ | 0)
//                .attach(this.validateResponse_());
//            return this.UpdateView(chlk.activities.apps.AppsListPage, result);
            return this.getApplications_(start_, developerId_, state_, filter_);
        },

        [[Number, chlk.models.id.SchoolPersonId, Number, String]],//[[chlk.models.apps.GetAppsPostData]],
        function getApplications_(startIndex_, developerId_, state_, filter_){
            var pageIndex = startIndex_ || 0;
            var result =
                ria.async.wait(
                    this.appsService.getApps(pageIndex, developerId_ || null, state_, filter_),
                    this.developerService.getDevelopers()
                )
                .attach(this.validateResponse_())
                .then(function(res){
                    var states = [
                        new chlk.models.apps.AppState(chlk.models.apps.AppStateEnum.DRAFT),
                        new chlk.models.apps.AppState(chlk.models.apps.AppStateEnum.SUBMITTED_FOR_APPROVAL),
                        new chlk.models.apps.AppState(chlk.models.apps.AppStateEnum.APPROVED),
                        new chlk.models.apps.AppState(chlk.models.apps.AppStateEnum.REJECTED),
                        new chlk.models.apps.AppState(chlk.models.apps.AppStateEnum.LIVE)
                    ];
                    return new chlk.models.apps.AppsListViewData(res[0], res[1], states, developerId_, state_);
                });
            if(startIndex_ || developerId_ || state_ || filter_)
                return this.UpdateView(chlk.activities.apps.AppsListPage, result);
            return this.PushView(chlk.activities.apps.AppsListPage, result);
        },

        [[chlk.models.apps.Application, Boolean, Boolean]],
        ria.async.Future, function prepareAppInfo(app_, readOnly, isDraft) {
            var result = this.categoryService
                .getCategories()
                .attach(this.validateResponse_())
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

                }, this);

            return result;
        },



        [chlk.controllers.SidebarButton('apps-info')],
        [chlk.controllers.AccessForRoles([
            chlk.models.common.RoleEnum.DEVELOPER
        ])],
        [[chlk.models.id.AppId, Boolean]],
        function detailsDeveloperAction(appId_, isSubmit_) {
            var isReadonly = false;
            var isDraft = !(!!isSubmit_);
            return this.appsService
                .getInfo(appId_)
                .attach(this.validateResponse_())
                .then(function(data){
                    if (!data.getId()){
                        return this.Redirect('apps', 'add', []);
                    }
                    else
                        return this.PushView(chlk.activities.apps.AppInfoPage, this.prepareAppInfo(data, isReadonly, isDraft));
                }, this);
        },

        [chlk.controllers.SidebarButton('apps-info')],
        [chlk.controllers.AccessForRoles([
            chlk.models.common.RoleEnum.DEVELOPER
        ])],
        [[Number, Number, String]],
        function invalidPictureFormatDeveloperAction(width, height, msg) {
            var result = new ria.async.DeferredData(new chlk.models.apps.AppPicture(new chlk.models.id.PictureId(''), '', width, height, msg, true));
            return this.UpdateView(chlk.activities.apps.AppInfoPage, result, msg.toLowerCase());
        },

        [chlk.controllers.AccessForRoles([
            chlk.models.common.RoleEnum.DEVELOPER
        ])],
        [[Number, Number, String, Object]],
        function uploadPictureDeveloperAction(width, height, msg, file) {
            if (!this.isValidFileExtension(file[0].name, ['jpg', 'jpeg', 'png', 'bmp']))
                return this.ShowMsgBox("Sorry, this picture format is invalid", 'Error', [{
                    text: 'Ok',
                    color: chlk.models.common.ButtonColor.GREEN.valueOf(),
                    controller: 'apps',
                    action: 'invalidPictureFormat',
                    params: [width, height, msg]
                }]);

            var result = this.appsService
                .uploadPicture(file, width, height)
                .attach(this.validateResponse_())
                .then(function(id){
                    var pictureUrl = this.pictureService.getPictureUrl(id, width, height);
                    return new chlk.models.apps.AppPicture(id, pictureUrl, width, height, msg, true);
                }, this);
            return this.UpdateView(chlk.activities.apps.AppInfoPage, result, msg.toLowerCase());
        },

        [chlk.controllers.SidebarButton('apps-info')],
        [chlk.controllers.AccessForRoles([
            chlk.models.common.RoleEnum.DEVELOPER
        ])],
        [[String, Number, Number, String]],
        function invalidScreenShotPictureFormatDeveloperAction(screenshots, width, height, msg) {
            var imgs = screenshots.split(",").map(function(el){
                    var pictureId = new chlk.models.id.PictureId(el);
                    var pictureUrl = this.pictureService.getPictureUrl(pictureId, width, height);
                    return new chlk.models.apps.AppPicture(pictureId, pictureUrl, width, height, msg, true);
                }, this);
            var result = new ria.async.DeferredData(new chlk.models.apps.AppScreenshots(imgs, false));
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

            if (!this.isValidFileExtension(file[0].name, ['jpg', 'jpeg', 'png', 'bmp']))
                return this.ShowMsgBox("Sorry, this picture format is invalid", 'Error', [{
                    text: 'Ok',
                    color: chlk.models.common.ButtonColor.GREEN.valueOf(),
                    controller: 'apps',
                    action: 'invalidScreenShotPictureFormat',
                    params: [(screenshots_ || ""), width, height, msg]
                }]);

            var result = this.appsService
                .uploadPicture(file, width, height)
                .attach(this.validateResponse_())
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
                }, this);
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
        [[chlk.models.id.SchoolPersonId]],
        function testApplicationSysAdminAction(devId) {
            this.appsService
                .testDevApps(devId);
            return null;
        },

        [chlk.controllers.AccessForRoles([
            chlk.models.common.RoleEnum.SYSADMIN
        ])],
        [[chlk.models.id.AppId]],
        function approveSysAdminAction(appId) {
            return this.appsService
                .approveApp(appId)
                .attach(this.validateResponse_())
                .then(function(data){
                    return this.BackgroundNavigate('apps', 'list', []);
                }, this);
        },

        [chlk.controllers.AccessForRoles([
            chlk.models.common.RoleEnum.SYSADMIN
        ])],
        [[chlk.models.id.AppId]],
        function declineSysAdminAction(appId) {
            return this.appsService
                .declineApp(appId)
                .attach(this.validateResponse_())
                .then(function(data){
                    return this.BackgroundNavigate('apps', 'list', []);
                }, this);
        },

        [chlk.controllers.AccessForRoles([
            chlk.models.common.RoleEnum.DEVELOPER
        ])],
        [[chlk.models.id.AppId]],
        function goLiveDeveloperAction(appId) {
            return this.appsService
                .goLive(appId)
                .attach(this.validateResponse_())
                .then(function(data){
                    return this.BackgroundNavigate('apps', 'general', []);
                }, this);
        },

        [chlk.controllers.AccessForRoles([
            chlk.models.common.RoleEnum.DEVELOPER
        ])],
        [[chlk.models.id.AppId, chlk.models.id.AppId]],
        function unlistDeveloperAction(liveAppId) {
            return this.appsService
                .unlist(liveAppId)
                .attach(this.validateResponse_())
                .then(function(data){
                    return this.BackgroundNavigate('apps', 'general', []);
                }, this);
        },

        [chlk.controllers.AccessForRoles([
            chlk.models.common.RoleEnum.TEACHER
        ])],
        [[chlk.models.id.AnnouncementId, chlk.models.id.AppId]],
        function tryToAttachTeacherAction(announcementId, appId) {

            var result = this.appsService
                .addToAnnouncement(appId, announcementId)
                .catchError(function(error_){
                    throw new chlk.lib.exception.AppErrorException(error_);
                }, this)
                .attach(this.validateResponse_())
                .then(function(app){
                    app.setCurrentModeUrl(app.getEditUrl());
                    return new chlk.models.apps.AppWrapperViewData(app, chlk.models.apps.AppModes.EDIT);
                }, this);
            return this.ShadeView(chlk.activities.apps.AppWrapperDialog, result);
        },

        [[String, String, chlk.models.apps.AppModes, chlk.models.id.AnnouncementApplicationId, Boolean, chlk.models.id.SchoolPersonId]],
        function viewAppAction(url, viewUrl, mode, announcementAppId_, isBanned, studentId_) {
            var result = this.appsService
                .getOauthCode(url)
                .catchError(function(error_){
                    throw new chlk.lib.exception.AppErrorException(error_);
                }, this)
                .attach(this.validateResponse_())
                .then(function(code){
                    if (isBanned){
                        return chlk.models.apps.AppWrapperViewData.$createAppBannedViewData(url);
                    }

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

                    if (studentId_){
                        viewUrl += "&studentId=" + studentId_.valueOf();
                    }

                    var app = new chlk.models.apps.AppAttachment.$create(
                        viewUrl,
                        code,
                        announcementAppId_,
                        appData
                    );
                    return new chlk.models.apps.AppWrapperViewData(app, mode);
                }, this);
            return this.ShadeView(chlk.activities.apps.AppWrapperDialog, result);
        },

        [chlk.controllers.AccessForRoles([
            chlk.models.common.RoleEnum.SYSADMIN
        ])],
        [[chlk.models.id.AppId]],
        function detailsSysAdminAction(appId) {
            var isReadonly = true;
            var result = this.appsService
                .getInfo(appId)
                .attach(this.validateResponse_())
                .then(function(data){
                    return this.prepareAppInfo(data, isReadonly, true);
                }, this);
            return this.PushView(chlk.activities.apps.AppInfoPage, result);
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
            this.appsService
                .createApp(devId, model.getName())
                .catchError(function(error_){
                    this.ShowMsgBox("App with this name already exists", "whoa.", [{
                        text: 'Ok',
                        controller: 'apps',
                        action: 'general',
                        params: [],
                        color: chlk.models.common.ButtonColor.GREEN.valueOf()
                    }], 'center');
                    return ria.async.BREAK;
                }, this)
                .attach(this.validateResponse_())
                .then(function(model){
                    return this.BackgroundNavigate('apps', 'details', []);
                }, this);
            return this.ShadeLoader();
        },

        [chlk.controllers.AccessForRoles([
            chlk.models.common.RoleEnum.DEVELOPER
        ])],
        [[chlk.models.id.AppId]],
        function deleteDeveloperAction(id) {
            return this.appsService
                .deleteApp(id)
                .attach(this.validateResponse_())
                .then(function(){
                    return this.Redirect('apps', 'details', []);
                }, this);
        },

        [chlk.controllers.AccessForRoles([
            chlk.models.common.RoleEnum.SYSADMIN
        ])],
        [[chlk.models.id.AppId]],
        function deleteSysAdminAction(id) {
            return this.appsService
                .sysAdminDeleteApp(id)
                .attach(this.validateResponse_())
                .then(function(){
                    return this.Redirect('apps', 'list', []);
                }, this);
        },

        [chlk.controllers.AccessForRoles([
            chlk.models.common.RoleEnum.DEVELOPER
        ])],
        [[chlk.models.apps.Application]],
        function updateApp(app) {
            return this.BackgroundNavigate('apps', 'details', [app.getId().valueOf(), true]);
        },

        [chlk.controllers.AccessForRoles([
            chlk.models.common.RoleEnum.DEVELOPER,
            chlk.models.common.RoleEnum.SYSADMIN
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



        [chlk.controllers.SidebarButton('apps-info')],
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
                 .attach(this.validateResponse_())
                 .then(function(newApp){
                     if(newApp.getMessage()){
                         this.getView().pop();
                         this.ShowMsgBox(newApp.getMessage(), null, [{
                            text: 'Ok'
                         }], 'center');
                         return this.BackgroundUpdateView(chlk.activities.apps.AppInfoPage, new ria.async.DeferredData(new chlk.models.Success()), chlk.activities.lib.DontShowLoader())
                     }
                     return this.updateApp(newApp);
                 }, this);
             return this.ShadeLoader();
        },


        [chlk.controllers.AccessForRoles([
            chlk.models.common.RoleEnum.DEVELOPER
        ])],

        [[chlk.models.apps.AppPersonReviewPostData]],
        function getAppReviewsAction(data){
            var scroll = data.isScroll();
            var result = this.appsService
                .getAppReviews(data.getAppId(), data.getStart())
                .attach(this.validateResponse_())
                .then(function(data){
                    return new chlk.models.apps.AppGeneralInfoViewData.$createFromReviews(data);
                });
            return this.UpdateView(chlk.activities.apps.AppGeneralInfoPage, result, 'loadReviews');
        },

        [chlk.controllers.AccessForRoles([
            chlk.models.common.RoleEnum.DEVELOPER
        ])],

        [[chlk.models.id.AppId]],
        function generalDeveloperAction(){
            var result = this.appsService
                .getInfo()
                .attach(this.validateResponse_())
                .then(function(data){
                    if (!data.getId()){
                        this.BackgroundNavigate('apps', 'add', []);
                        return ria.async.BREAK;
                    }
                    else{
                        var pictureUrl = this.pictureService.getPictureUrl(data.getSmallPictureId(), 74);

                        return ria.async.wait(
                                this.appsService.getAppAnalytics(data.getId()),
                                this.appsService.getAppReviews(data.getId())
                            )
                            .attach(this.validateResponse_())
                            .then(function(res){
                               var appReviews = res[1];
                               var analytics = res[0];

                               return new chlk.models.apps.AppGeneralInfoViewData(
                                    data.getName(),
                                    data.getId(),
                                    data.getLiveAppId(),
                                    data.getState(),
                                    pictureUrl,
                                    5, //todo: pass rating,
                                    new chlk.models.common.ChlkDate(),
                                    appReviews,
                                    analytics || new chlk.models.developer.HomeAnalytics()
                               );
                            });
                    }
                }, this);

            return this.PushView(chlk.activities.apps.AppGeneralInfoPage, result);
        },

        [chlk.controllers.AccessForRoles([
            chlk.models.common.RoleEnum.SYSADMIN
        ])],

        [[Object]],
        function changeAppTypeAction(data){

            var isInternal = Boolean(data.isInternal === 'true' || data.isInternal === 'on');
            var appId = new chlk.models.id.AppId(data.appId);

            return this.appsService
                .changeAppType(appId, isInternal)
                .attach(this.validateResponse_())
                .then(function(data){
                    return this.Redirect('apps', 'page', []);
                }, this);

        }
    ])
});
