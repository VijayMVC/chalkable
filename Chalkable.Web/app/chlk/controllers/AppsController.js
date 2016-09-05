REQUIRE('chlk.controllers.BaseController');

REQUIRE('chlk.services.ApplicationService');
REQUIRE('chlk.services.AppCategoryService');
REQUIRE('chlk.services.GradeLevelService');
REQUIRE('chlk.services.PictureService');
REQUIRE('chlk.services.DeveloperService');
REQUIRE('chlk.services.StandardService');

REQUIRE('chlk.activities.apps.AppsListPage');
REQUIRE('chlk.activities.apps.AppInfoPage');
REQUIRE('chlk.activities.apps.AppGeneralInfoPage');
REQUIRE('chlk.activities.apps.AddAppDialog');
REQUIRE('chlk.activities.apps.AppWrapperDialog');
REQUIRE('chlk.activities.apps.ExternalAttachAppDialog');
REQUIRE('chlk.activities.apps.AppWrapperPage');
REQUIRE('chlk.activities.apps.MyAppsPage');
REQUIRE('chlk.activities.apps.DisableAppDialog');


REQUIRE('chlk.models.apps.Application');
REQUIRE('chlk.models.apps.AppPostData');
REQUIRE('chlk.models.apps.ShortAppInfo');
REQUIRE('chlk.models.apps.AppAccess');
REQUIRE('chlk.models.apps.AppPicture');
REQUIRE('chlk.models.apps.AppState');
REQUIRE('chlk.models.apps.AppScreenShots');
REQUIRE('chlk.models.apps.AppGeneralInfoViewData');
REQUIRE('chlk.models.apps.AppWrapperViewData');
REQUIRE('chlk.models.developer.HomeAnalytics');
REQUIRE('chlk.models.apps.GetAppsPostData');

REQUIRE('chlk.models.apps.MyAppsViewData');


REQUIRE('chlk.models.apps.AppsListViewData');

REQUIRE('chlk.models.id.AppId');
REQUIRE('chlk.models.id.AppPermissionId');

REQUIRE('chlk.models.standard.GetStandardTreePostData');
REQUIRE('chlk.models.id.AnnouncementAttributeId');

NAMESPACE('chlk.controllers', function (){

    /** @class chlk.controllers.AppsController */
    CLASS(
        'AppsController', EXTENDS(chlk.controllers.BaseController), [

        [ria.mvc.Inject],
        chlk.services.ApplicationService, 'appsService',

        [ria.mvc.Inject],
        chlk.services.AppCategoryService, 'categoryService',

        [ria.mvc.Inject],
        chlk.services.GradeLevelService, 'gradeLevelService',

        [ria.mvc.Inject],
        chlk.services.PictureService, 'pictureService',

        [ria.mvc.Inject],
        chlk.services.DeveloperService, 'developerService',

        [ria.mvc.Inject],
        chlk.services.StandardService, 'standardService',

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
                    return new chlk.models.apps.AppsListViewData(
                        res[0],
                        res[1],
                        states,
                        developerId_,
                        state_,
                        this.userInRole(chlk.models.common.RoleEnum.APPTESTER)
                    );
                }, this);

            return this.PushOrUpdateView(chlk.activities.apps.AppsListPage, result);
        },

        [[chlk.models.apps.Application, Boolean, Boolean]],
        ria.async.Future, function prepareAppInfo(app_, readOnly, isDraft) {
            var result = this.categoryService.getCategories()
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
                    var standards = app_.getStandards();
                    if(!standards) app_.setStandards([]);

                    this.getContext().getSession().set(ChlkSessionConstants.CC_STANDARDS, app_.getStandards() || []);

                    if (!app_.getState()){
                        var appState = new chlk.models.apps.AppState();
                        appState.deserialize(chlk.models.apps.AppStateEnum.DRAFT);
                        app_.setState(appState);
                    }
                    if (!app_.getAppAccess())
                        app_.setAppAccess(new chlk.models.apps.AppAccess());

                    var iconDims = chlk.models.apps.AppPicture.ICON_DIMS();
                    var appIconId = app_.getSmallPictureId() || new chlk.models.id.PictureId('');
                    var iconUrl = this.pictureService.getPictureUrl(appIconId, iconDims.width, iconDims.height);
                    app_.setIconPicture(new chlk.models.apps.AppPicture(appIconId, iconUrl, iconDims.width, iconDims.height, 'Icon', !readOnly));


                    var bannerDims = chlk.models.apps.AppPicture.BANNER_DIMS();
                    var appBannerId = app_.getBigPictureId() || new chlk.models.id.PictureId('');
                    var bannerUrl = this.pictureService.getPictureUrl(appBannerId, bannerDims.width, bannerDims.height);
                    app_.setBannerPicture(new chlk.models.apps.AppPicture(appBannerId, bannerUrl,
                        bannerDims.width, bannerDims.height, 'Banner', !readOnly));

                    var attachIconDims = chlk.models.apps.AppPicture.EXTERNAL_ATTACH_ICON_DIMS();
                    var attachIconId = app_.getExternalAttachPictureId() || new chlk.models.id.PictureId('');
                    var attachIconUrl = this.pictureService.getPictureUrl(attachIconId, attachIconDims.width, attachIconDims.height);
                    app_.setExternalAttachPicture(new chlk.models.apps.AppPicture(attachIconId, attachIconUrl,
                        attachIconDims.width, attachIconDims.height, 'ExternalAttachPicture', !readOnly));


                    var screenshots = app_.getScreenshotIds() || [];
                    var screenshotDims = chlk.models.apps.AppPicture.SCREENSHOT_DIMS();

                    var screenshotPictures = screenshots.map(function(pictureId){
                        var pictureUrl = this.pictureService.getPictureUrl(pictureId, screenshotDims.width, screenshotDims.height);
                        return new chlk.models.apps.AppPicture(
                            pictureId,
                            pictureUrl,
                            screenshotDims.width,
                            screenshotDims.height,
                            'screenshot',
                            !readOnly
                        );
                    }, this);

                    app_.setScreenshotPictures(new chlk.models.apps.AppScreenShots(screenshotPictures, readOnly));

                    return new chlk.models.apps.AppInfoViewData(app_, readOnly, cats, gradeLevels, permissions, platforms, isDraft);

                }, this);

            return result;
        },

        //move cc_standards logic to StandardController
        [[chlk.models.id.AppId, Boolean]],
        function showStandardsAction(applicationId, onlyOne_){
            var standards = this.getContext().getSession().get(ChlkSessionConstants.CC_STANDARDS, []);
            var standardIds = standards.map(function (_) { return _.getStandardId().valueOf() });

            var res = this.WidgetStart('standard', 'showABStandards', [standardIds, onlyOne_])
                .then(function (data) {
                    var standards = data;
                    this.getContext().getSession().set(ChlkSessionConstants.CC_STANDARDS, data);
                    return this.addStandards_(standards, applicationId);
                }, this);

            return this.UpdateView(chlk.activities.apps.AppInfoPage, res);
        },

        //move cc_standards logic to StandardController

        [[ArrayOf(chlk.models.academicBenchmark.Standard), chlk.models.id.AppId]],
        chlk.models.standard.ApplicationStandardsViewData, function addStandards_(standards, appId){
            this.getContext().getSession().set(ChlkSessionConstants.CC_STANDARDS, standards);
            return new chlk.models.standard.ApplicationStandardsViewData(appId, standards);
        },

        [[chlk.models.id.AppId, chlk.models.id.ABStandardId]],
        function removeStandardAction(appId, standardId){
            var standards = this.getContext().getSession().get(ChlkSessionConstants.CC_STANDARDS, []);
            if(standards)
                standards = standards.filter(function(s){return s.getId() != standardId});
            this.getContext().getSession().set(ChlkSessionConstants.CC_STANDARDS, standards);
            var res = new chlk.models.standard.ApplicationStandardsViewData(appId, standards);
            return this.UpdateView(chlk.activities.apps.AppInfoPage, new ria.async.DeferredData(res));
        },

        [[chlk.models.id.AppId]],
        function removeAllStandardsAction(appId){
            this.getContext().getSession().set(ChlkSessionConstants.CC_STANDARDS, []);
            var res = new chlk.models.standard.ApplicationStandardsViewData(appId, []);
            return this.UpdateView(chlk.activities.apps.AppInfoPage, new ria.async.DeferredData(res))
        },

        [chlk.controllers.SidebarButton('new-item')],
        [chlk.controllers.AccessForRoles([
            chlk.models.common.RoleEnum.DEVELOPER
        ])],
        [[chlk.models.id.AppId, Boolean]],
        function detailsDeveloperAction(appId_, isSubmit_) {
            var isReadonly = false;
            var isDraft = !(!!isSubmit_);
            var result = this.appsService
                .getInfo(appId_)
                .attach(this.validateResponse_())
                .then(function(data){
                    if (!data.getId()){
                        return this.ShowAlertBox('This application was not found')
                            .thenBreak();
                    }

                    return this.prepareAppInfo(data, isReadonly, isDraft);
                }, this);

            return this.PushOrUpdateView(chlk.activities.apps.AppInfoPage, result);
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
        function uploadPictureDeveloperAction(width, height, msg, fileList) {
            if (!this.isValidFileExtension(fileList[0].name, ['jpg', 'jpeg', 'png', 'bmp']))
                return this.ShowMsgBox("Sorry, this picture format is invalid", 'Error', [{
                    text: 'Ok',
                    color: chlk.models.common.ButtonColor.GREEN.valueOf(),
                    controller: 'apps',
                    action: 'invalidPictureFormat',
                    params: [width, height, msg]
                }]), null;

            var result = this.appsService
                .uploadPicture(fileList[0], width, height)
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
            var result = new ria.async.DeferredData(new chlk.models.apps.AppScreenShots(imgs, false));
            return this.UpdateView(chlk.activities.apps.AppInfoPage, result, msg.toLowerCase());
        },

        [chlk.controllers.AccessForRoles([
            chlk.models.common.RoleEnum.DEVELOPER
        ])],
        [[String, Object]],
        function uploadScreenshotDeveloperAction(screenshots_, fileList) {

            var screenshotDims = chlk.models.apps.AppPicture.SCREENSHOT_DIMS();
            var width = screenshotDims.width;
            var height = screenshotDims.height;
            var msg = "Screenshots";

            if (!this.isValidFileExtension(fileList[0].name, ['jpg', 'jpeg', 'png', 'bmp']))
                return this.ShowMsgBox("Sorry, this picture format is invalid", 'Error', [{
                    text: 'Ok',
                    color: chlk.models.common.ButtonColor.GREEN.valueOf(),
                    controller: 'apps',
                    action: 'invalidScreenShotPictureFormat',
                    params: [(screenshots_ || ""), width, height, msg]
                }]), null;

            var result = this.appsService
                .uploadPicture(fileList[0], width, height)
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
                    return new chlk.models.apps.AppScreenShots(screenshots, false);
                }, this);
            return this.UpdateView(chlk.activities.apps.AppInfoPage, result, msg.toLowerCase());
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
            var screenshotDims = chlk.models.apps.AppPicture.SCREENSHOT_DIMS();
            var width = screenshotDims.width;
            var height = screenshotDims.height;
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
            var result = new ria.async.DeferredData(new chlk.models.apps.AppScreenShots(screenshots, false));
            return this.UpdateView(chlk.activities.apps.AppInfoPage, result, msg.toLowerCase());
        },

        [chlk.controllers.AccessForRoles([
            chlk.models.common.RoleEnum.SYSADMIN,
            chlk.models.common.RoleEnum.APPTESTER,
        ])],
        [[chlk.models.id.SchoolPersonId]],
        function testApplicationAction(devId) {
            this.appsService
                .testDevApps(devId);
            return null;
        },

        [chlk.controllers.AccessForRoles([
            chlk.models.common.RoleEnum.SYSADMIN,
            chlk.models.common.RoleEnum.APPTESTER
        ])],
        [[chlk.models.id.AppId]],
        function approveAction(appId) {
            this.appsService
                .approveApp(appId)
                .attach(this.validateResponse_())
                .thenCall(this.BackgroundNavigate, ['apps', 'list', []]);
            return null;
        },

        [chlk.controllers.AccessForRoles([
            chlk.models.common.RoleEnum.SYSADMIN,
            chlk.models.common.RoleEnum.APPTESTER
        ])],
        [[chlk.models.id.AppId]],
        function declineAction(appId) {
            this.appsService
                .declineApp(appId)
                .attach(this.validateResponse_())
                .thenCall(this.BackgroundNavigate, ['apps', 'list', []]);
            return null;
        },

        [chlk.controllers.AccessForRoles([
            chlk.models.common.RoleEnum.DEVELOPER
        ])],
        [[chlk.models.id.AppId]],
        function goLiveDeveloperAction(appId) {
            this.appsService
                .goLive(appId)
                .attach(this.validateResponse_())
                .thenCall(this.BackgroundNavigate, ['apps', 'general', []]);
            return null;
        },

        [chlk.controllers.AccessForRoles([
            chlk.models.common.RoleEnum.DEVELOPER
        ])],
        [[chlk.models.id.AppId, chlk.models.id.AppId]],
        function unlistDeveloperAction(liveAppId) {
            this.appsService
                .unlist(liveAppId)
                .attach(this.validateResponse_())
                .thenCall(this.BackgroundNavigate, ['apps', 'general', []]);
            return null;
        },

        [chlk.controllers.StudyCenterEnabled()],
        [chlk.controllers.AccessForRoles([
            chlk.models.common.RoleEnum.TEACHER,
            chlk.models.common.RoleEnum.DISTRICTADMIN
        ])],
        [[chlk.models.id.AppId, chlk.models.id.AnnouncementId, chlk.models.announcement.AnnouncementTypeEnum, String, String]],
        function openRecommendedContentsAction(appId, annId, announcementType, contentId, standardsUrlComponents_){
            var appUrlAppend_ = '';
            if(contentId)
                appUrlAppend_ += 'contentId=' + contentId;
            if(standardsUrlComponents_)
                appUrlAppend_ += '&' + standardsUrlComponents_;
            return this.tryToAttachAction(annId, appId, announcementType, appUrlAppend_);
        },

        [chlk.controllers.StudyCenterEnabled()],
        [chlk.controllers.AccessForRoles([
            chlk.models.common.RoleEnum.TEACHER,
            chlk.models.common.RoleEnum.DISTRICTADMIN
        ])],
        [[chlk.models.id.AppId, chlk.models.id.AnnouncementId, chlk.models.announcement.AnnouncementTypeEnum, String]],
        function openSuggestedAppTeacherAction(appId, annId, announcementType, appUrlAppend_){
            return this.tryToAttachAction(annId, appId, announcementType, appUrlAppend_);
        },


        [chlk.controllers.StudyCenterEnabled()],
        [[chlk.models.id.AppId, String, String, Boolean, String]],
        function openSuggestedAppFromExplorerAction(appId, appUrl, viewUrl, isBanned, appUrlSuffix_){
            if(viewUrl){
                this.userTrackingService.openedAppFrom(appUrl, "explorer");
                return this.viewAppAction(appUrl, viewUrl, chlk.models.apps.AppModes.VIEW, new chlk.models.id.AnnouncementApplicationId(appId.valueOf()), isBanned, null, appUrlSuffix_);
            }
            return null;
        },

        [chlk.controllers.StudyCenterEnabled()],
        [chlk.controllers.AccessForRoles([
            chlk.models.common.RoleEnum.TEACHER,
            chlk.models.common.RoleEnum.DISTRICTADMIN
        ])],
        [[chlk.models.id.AnnouncementId, chlk.models.id.AppId, chlk.models.announcement.AnnouncementTypeEnum, String]],
        function tryToAttachAction(announcementId, appId, announcementType, appUrlAppend_) {
            var result = this.appsService
                .addToAnnouncement(this.getCurrentPerson().getId(), appId, announcementId, announcementType)
                .catchError(function(error_){
                    throw new chlk.lib.exception.AppErrorException(error_);
                }, this)
                .attach(this.validateResponse_())
                .then(function(app){
                    app.setCurrentModeUrl(app.getEditUrl() + (appUrlAppend_ ? '&' + appUrlAppend_ : ''));
                    return new chlk.models.apps.AppWrapperViewData(app, chlk.models.apps.AppModes.EDIT, announcementType);
                }, this);
            return this.ShadeView(chlk.activities.apps.AppWrapperDialog, result);
        },

        [chlk.controllers.StudyCenterEnabled()],
        [[chlk.models.id.AnnouncementId, chlk.models.id.AppId, chlk.models.announcement.AnnouncementTypeEnum, String, chlk.models.id.AnnouncementAttributeId]],
        function viewExternalAttachAppAction(announcementId, appId, announcementType, appUrlAppend_, attributeId_) {
            if(!this.isStudyCenterEnabled())
                return this.ShowMsgBox('Current school doesn\'t support applications, study center, profile explorer', 'whoa.'), null;

            var mode = chlk.models.apps.AppModes.ATTACH;

            var result = this.appsService
                .getAccessToken(this.getCurrentPerson().getId(), null, appId)
                .catchError(function(error_){
                    throw new chlk.lib.exception.AppErrorException(error_);
                })
                .attach(this.validateResponse_())
                .then(function(data){
                    var appData = data.getApplication();

                    var viewUrl = appData.getUrl() + '?mode=' + mode.valueOf()
                        + '&apiRoot=' + encodeURIComponent(_GLOBAL.location.origin)
                        + '&token=' + encodeURIComponent(data.getToken())
                        + '&announcementId=' + encodeURIComponent(announcementId.valueOf())
                        + '&announcementType=' + encodeURIComponent(announcementType.valueOf())
                        + (attributeId_ ? '&attributeId=' + encodeURIComponent(attributeId_.valueOf()) : '')
                        + (appUrlAppend_ ? '&' + appUrlAppend_ : '');

                    var options = this.getContext().getSession().get(ChlkSessionConstants.ATTACH_OPTIONS);

                    return new chlk.models.apps.ExternalAttachAppViewData(options, appData, viewUrl, 'Attach ' + appData.getName() + ' File');
                }, this);

            return this.ShadeView(chlk.activities.apps.ExternalAttachAppDialog, result);
        },

        [chlk.controllers.AccessForRoles([
            chlk.models.common.RoleEnum.TEACHER,
            chlk.models.common.RoleEnum.DISTRICTADMIN
        ])],
        [[chlk.models.id.AnnouncementId, chlk.models.id.AppId, chlk.models.announcement.AnnouncementTypeEnum, String]],
        function attachAssessmentAppAction(announcementId, appId, announcementType, appUrlAppend_) {
            //if(!this.isStudyCenterEnabled())
            //    return this.ShowMsgBox('Current school doesn\'t support applications, study center, profile explorer', 'whoa.'), null;

            var result = this.appsService
                .addToAnnouncement(this.getCurrentPerson().getId(), appId, announcementId, announcementType)
                .catchError(function(error_){
                    throw new chlk.lib.exception.AppErrorException(error_);
                }, this)
                .attach(this.validateResponse_())
                .then(function(app){
                    var viewUrl = app.getEditUrl()
                        + '&apiRoot=' + encodeURIComponent(_GLOBAL.location.origin)
                        + '&token=' + encodeURIComponent(app.getToken())
                        + (appUrlAppend_ ? '&' + appUrlAppend_ : '');

                    var options = this.getContext().getSession().get(ChlkSessionConstants.ATTACH_OPTIONS);

                    return new chlk.models.apps.ExternalAttachAppViewData(options, app
                        , viewUrl, '', app.getAnnouncementApplicationId());
                }, this);

            return this.ShadeView(chlk.activities.apps.ExternalAttachAppDialog, result);
        },

        [chlk.controllers.SidebarButton('add-new')],
        [chlk.controllers.StudyCenterEnabled()],
        [chlk.controllers.AccessForRoles([
            chlk.models.common.RoleEnum.TEACHER,
            chlk.models.common.RoleEnum.DISTRICTADMIN
        ])],
        [[chlk.models.id.AnnouncementId, chlk.models.id.AppId, chlk.models.announcement.AnnouncementTypeEnum, String]],
        function tryToAttachFromAnnouncementAction(announcementId, appId, announcementType, appUrlAppend_) {
            return this.tryToAttachAction(announcementId, appId, announcementType, appUrlAppend_);
        },

        [[String, String, chlk.models.apps.AppModes, chlk.models.id.AnnouncementApplicationId, Boolean, chlk.models.id.SchoolPersonId, String, Boolean]],
        function viewAppAction(url, viewUrl, mode, announcementAppId_, isBanned, studentId_, appUrlSuffix_, isAssessment_) {

            if(!isAssessment_ && !this.isStudyCenterEnabled())
                return this.ShowMsgBox('Current school doesn\'t support applications, study center, profile explorer', 'whoa.'), null;

            if(isAssessment_ && !this.isAssessmentEnabled() && !this.isStudyCenterEnabled())
                return this.ShowMsgBox('Current school doesn\'t support assessments'), null;


            var result = this.appsService
                .getAccessToken(this.getCurrentPerson().getId(), url)
                .catchError(function(error_){
                    throw new chlk.lib.exception.AppErrorException(error_);
                }, this)
                .attach(this.validateResponse_())
                .then(function(data){
                    if (isBanned){
                        return chlk.models.apps.AppWrapperViewData.$createAppBannedViewData(url);
                    }

                    var appData = data.getApplication();
                    if (mode == chlk.models.apps.AppModes.MYAPPSVIEW){
                        var appAccess = appData.getAppAccess();
                        var hasMyAppsView = appAccess.isStudentMyAppsEnabled() && this.userInRole(chlk.models.common.RoleEnum.STUDENT) ||
                            appAccess.isTeacherMyAppsEnabled() && this.userInRole(chlk.models.common.RoleEnum.TEACHER) ||
                            appAccess.isAdminMyAppsEnabled() && this.userIsAdmin() ||
                            appAccess.isParentMyAppsEnabled() && this.userInRole(chlk.models.common.RoleEnum.PARENT);
                        appAccess.setMyAppsForCurrentRoleEnabled(hasMyAppsView);
                    }

                    //check if it's assessment app

                    if (studentId_){
                        viewUrl += "&studentId=" + studentId_.valueOf();
                    }

                    if (appUrlSuffix_) {
                        viewUrl += "&" + appUrlSuffix_;
                    }

                    var app = new chlk.models.apps.AppAttachment.$create(
                        viewUrl,
                        data.getToken(),
                        announcementAppId_,
                        appData
                    );
                    return new chlk.models.apps.AppWrapperViewData(app, mode);
                }, this);


            var startLocation = "";
            if (mode == chlk.models.apps.AppModes.MYAPPSVIEW)
                startLocation = "My Apps";
            if (mode == chlk.models.apps.AppModes.EDIT)
                startLocation = "New Item";

            if (isAssessment_)
                this.userTrackingService.tookAssessment();
            else
                this.userTrackingService.openedAppFrom(url, startLocation);
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
            var result = this.createApp_(true);
            return this.ShadeView(chlk.activities.lib.PendingActionDialog, result);
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
            chlk.models.common.RoleEnum.SYSADMIN
        ])],
        [[chlk.models.id.AppId, String]],
        function tryDeleteApplicationSysAdminAction(id, appName) {
            return this.tryDeleteApplication_(id, appName)
                .then(function() {
                    return this.Redirect('apps', 'list', [])
                }, this);
        },


        [chlk.controllers.AccessForRoles([
            chlk.models.common.RoleEnum.DEVELOPER,
        ])],
        [[chlk.models.id.AppId, String]],
        function tryDeleteApplicationDeveloperAction(id, appName) {
            var result = this.tryDeleteApplication_(id, appName)
                .then(function(){
                    return this.BackgroundNavigate('apps', 'general', []); }, this)
                .thenBreak();
            return this.UpdateView(chlk.activities.apps.AppGeneralInfoPage, result);
        },

        ria.async.Future, function tryDeleteApplication_(id, appName){
            var msgText = "You are about to delete application. This can not be undone!!!\n\nPlease type application name to confirm.",
                buttons = [{text: 'DELETE', clazz: 'negative-button', value: 'ok'}, {text: 'Cancel'}];
            return this.ShowMsgBox(msgText, "whoa.", buttons, null, false, 'text', "")
                .then(function (mrResult) {
                    if (appName != mrResult)
                        return this.ShowAlertBox("Incorrect application name provided", "Invalid application name")
                            .thenBreak();

                    return mrResult;
                }, this)
                .thenCall(this.appsService.deleteApp, [id])
                .attach(this.validateResponse_());
        },


        [chlk.controllers.SidebarButton('apps-info')],
        [chlk.controllers.AccessForRoles([
            chlk.models.common.RoleEnum.DEVELOPER
        ])],
        [[chlk.models.apps.AppPostData]],
        function updateDeveloperAction(model){

            var appAccess = model.isAdvancedApp() ? new chlk.models.apps.AppAccess(
                model.isStudentMyAppsEnabled(),
                model.isTeacherMyAppsEnabled(),
                model.isAdminMyAppsEnabled(),
                model.isParentMyAppsEnabled(),
                model.isAttachEnabled(),
                model.isShowInGradingViewEnabled(),
                null,
                model.isStudentExternalAttachEnabled(),
                model.isTeacherExternalAttachEnabled(),
                model.isAdminExternalAttachEnabled(),
                model.isSysAdminSettingsEnabled(),
                model.isDistrictAdminSettingsEnabled(),
                model.isStudentProfileEnabled(),
                model.isProvidesRecommendedContent()

            ) : new chlk.models.apps.AppAccess(
                true, true, true, true, true, false, false, false, false
            );

             var shortAppData = new chlk.models.apps.ShortAppInfo(
                model.getName(),
                model.getUrl(),
                model.getVideoDemoUrl(),
                model.getShortDescription(),
                model.getLongDescription(),
                model.isAdvancedApp(),
                model.getAppIconId(),
                model.getAppBannerId(),
                model.getAppExternalAttachPictureId()
             );

             var isFreeApp = model.isFree();

             var isSchoolFlatRateEnabled = model.isSchoolFlatRateEnabled();
             var isClassFlatRateEnabled = model.isClassFlatRateEnabled();
            var cats = this.getIdsList(model.getCategories(), chlk.models.id.AppCategoryId);
            var gradeLevels = this.getIdsList(model.getGradeLevels(), chlk.models.id.GradeLevelId);
            var appPermissions = this.getIdsList(model.getPermissions(), chlk.models.apps.AppPermissionTypeEnum);
            var appScreenShots = this.getIdsList(model.getAppScreenshots(), chlk.models.id.PictureId);
            var appPlatforms = this.getIdsList(model.getPlatforms(), chlk.models.apps.AppPlatformTypeEnum);
            var standards = this.getIdsList(model.getStandards(), chlk.models.id.ABStandardId);

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

                var externalAttachPictureId = null;
                if(model.getAppExternalAttachPictureId()){
                    externalAttachPictureId = model.getAppExternalAttachPictureId().valueOf();
                    if(externalAttachPictureId.length == 0) externalAttachPictureId = null;
                }

                if (appIconId == null || appBannerId == null){
                    return this.ShowAlertBox('You need to upload icon and banner picture for you app', 'Error'), null;
                }

                //todo : check for externalAttach picture

                var developerWebsite = model.getDeveloperWebSite();
                var developerName = model.getDeveloperName();

                if ((developerWebsite == null || developerWebsite == "") ||
                    (developerName == null || developerName == "")){
                    return this.ShowAlertBox('We just need you to enter your Developer Name and Website. You can do that in settings, on the left.'), null;
                }
            }

             var result = this.appsService
                 .updateApp(
                     model.getId(),
                     shortAppData,
                     appPermissions,
                     this.getCurrentPerson().getId(),
                     appAccess,
                     cats,
                     appScreenShots,
                     gradeLevels,
                     appPlatforms,
                     !model.isDraft(),
                     standards
                 )
                 .attach(this.validateResponse_())
                 .then(function(newApp){
                     if(newApp.getMessage()){
                         return this
                             .ShowAlertBox(newApp.getMessage())
                             .thenBreak()
                     }

                     this.BackgroundNavigate('apps', 'details', [newApp.getId().valueOf(), true]);
                     return ria.async.BREAK;
                 }, this);
             return this.UpdateView(chlk.activities.apps.AppInfoPage, result);
        },



        ria.async.Future, function createApp_(navigate_) {
            var devId = this.getCurrentPerson().getId();
            return this.ShowPromptBox('Please enter application name', '', null, {"class":"validate[required]"})
                .then(function(appName){
                    return this.appsService.createApp(devId, appName);
                }, this)
                .catchError(function(error_){
                    return this.ShowMsgBox("App with this name already exists", "whoa.", [{
                            text: 'Ok',
                            controller: 'apps',
                            action: 'general',
                            params: [],
                            color: chlk.models.common.ButtonColor.GREEN.valueOf()
                        }], 'center')
                        .thenBreak();
                }, this)
                .attach(this.validateResponse_())
                .then(function(model){
                    return navigate_ ? this.BackgroundNavigate('apps', 'details', []) : null;
                }, this)
                .thenBreak();
        },

        [chlk.controllers.AccessForRoles([
            chlk.models.common.RoleEnum.DEVELOPER
        ])],
        [chlk.controllers.SidebarButton('apps')],
        [[chlk.models.id.AppId]],
        function generalDeveloperAction(appId_){
            var result = this.appsService
                .getInfo(appId_, !!appId_)
                .attach(this.validateResponse_())
                .then(function (data) {
                    return data.getId() ? data : this.createApp_(true);
                }, this)
                .then(function(data){
                    var pictureUrl = data.getBannerPictureUrl();
                    return ria.async.wait(
                            this.appsService.getAppAnalytics(data.getId()),
                            this.appsService.getDevApps()
                        )
                        .attach(this.validateResponse_())
                        .then(function(res){
                           var analytics = res[0];
                           var devApps = res[1];

                           return new chlk.models.apps.AppGeneralInfoViewData(
                                data.getName(),
                                data.getId(),
                                data.getLiveAppId(),
                                data.getState(),
                                pictureUrl,
                                5, //todo: pass rating,
                                new chlk.models.common.ChlkDate(),
                                analytics || new chlk.models.developer.HomeAnalytics(),
                                devApps
                           );
                        });
                }, this);

            return this.PushView(chlk.activities.apps.AppGeneralInfoPage, result);
        },

        [chlk.controllers.AccessForRoles([
            chlk.models.common.RoleEnum.SYSADMIN
        ])],
        [chlk.controllers.SidebarButton('apps')],
        [[Object]],
        function changeAppTypeSysAdminAction(data){

            var isInternal = Boolean(data.isInternal === 'true' || data.isInternal === 'on');
            var appId = new chlk.models.id.AppId(data.appId);

            this.appsService
                .changeAppType(appId, isInternal)
                .attach(this.validateResponse_())
                .thenCall(this.BackgroundNavigate, ['apps', 'page', []]);
            return null;
        },
        [[chlk.models.apps.Application]],
        function setInternalDataSysAdminAction(app){
            this.appsService.setInternalData(app.getId(), app.getInternalScore(), app.getInternalDescription())
                .attach(this.validateResponse_())
                .thenCall(this.BackgroundNavigate, ['apps', 'details', [app.getId()]]);
            return null;
        },

        [[String, String]],
        ria.async.Future, function prepareExternalAttachAppViewData_(mode, appUrlAppend_){

            var appId = chlk.models.id.AppId(_GLOBAL.assessmentApplicationId);

            return this.appsService
                .getAccessToken(this.getCurrentPerson().getId(), null, appId)
                .catchError(function(error_){
                    throw new chlk.lib.exception.AppErrorException(error_);
                })
                .attach(this.validateResponse_())
                .then(function(data){
                    var appData = data.getApplication();

                    var viewUrl = appData.getUrl() + '?mode=' + mode
                        + '&apiRoot=' + encodeURIComponent(_GLOBAL.location.origin)
                        + '&token=' + encodeURIComponent(data.getToken())
                        + (appUrlAppend_ ? '&' + appUrlAppend_ : '');

                    return new chlk.models.apps.ExternalAttachAppViewData(null, appData, viewUrl, '');
                }, this);
        },

        [chlk.controllers.AssessmentEnabled()],
        [chlk.controllers.AccessForRoles([
            chlk.models.common.RoleEnum.DISTRICTADMIN,
            chlk.models.common.RoleEnum.TEACHER,
            chlk.models.common.RoleEnum.STUDENT
        ])],
        [chlk.controllers.SidebarButton('assessment')],
        [[String]],
        function assessmentAction(appUrlAppend_) {
            var result = this.prepareExternalAttachAppViewData_("myview", appUrlAppend_);
            return this.PushView(chlk.activities.apps.AppWrapperPage, result);
        },

        [chlk.controllers.SidebarButton('assessment')],
        [chlk.controllers.AccessForRoles([
            chlk.models.common.RoleEnum.SYSADMIN,
            chlk.models.common.RoleEnum.APPTESTER
        ])],
        [[String]],
        function assessmentSettingsAction(appUrlAppend_) {
            var result = this.prepareExternalAttachAppViewData_("sysadminview", appUrlAppend_);
            return this.PushOrUpdateView(chlk.activities.apps.AppWrapperPage, result);
        },

        [chlk.controllers.SidebarButton('apps')],
        function myAppsAction(){
            var result = this.appsService
                .getMyApps()
                .attach(this.validateResponse_())
                .then(function(apps){
                    return new chlk.models.apps.MyAppsViewData(apps);
                });
            return this.PushOrUpdateView(chlk.activities.apps.MyAppsPage, result);
        },

        [[chlk.models.id.AppId]],
        function disableAppAction(appId){
            //var schoolsIds = schoolIdsStr ? schoolIdsStr.split(',').map(function(_){
            //    return new chlk.models.id.SchoolId
            //}) : [];
            //schoolsIds.push(new chlk.models.id.SchoolId('736FFA83-7219-4F54-A4C2-837453863209'));
            var res = this.WidgetStart('apps', 'disableApp', [appId, false])
                .then(function(data){
                    return this.BackgroundNavigate('apps', 'myApps', []);
                }, this);
            return null;
        },

        [[String, chlk.models.id.AppId, Boolean]],
        function disableAppWidgetAction(requestId, appId, appBanned){
            var res = this.appsService.getApplicationBannedSchools(appId)
                .attach(this.validateResponse_())
                .then(function(data){
                    return new chlk.models.apps.ApplicationBanViewData(requestId, appId, data);
                });
            return this.ShadeView(chlk.activities.apps.DisableAppDialog, res);
        },


        [[chlk.models.apps.SubmitApplicationBan]],
        function submitDisableAppAction(model){
            var res = this.appsService
                .submitApplicationBan(
                    model.getApplicationId(),
                    model.getSchoolIds()
                )
                .attach(this.validateResponse_())
                .then(function(data){
                    this.WidgetComplete(model.getRequestId(), data);
                }, this);
            return this.CloseView(chlk.activities.apps.DisableAppDialog);
        },

        [[chlk.models.id.ClassId, String, String]],
        function getSuggestedAppsAction(classId, academicBenchmarkIds, standardUrlComponents_) {
            var result = this.appsService
                .getSuggestedApps(academicBenchmarkIds, null, null, true)
                .attach(this.validateResponse_())
                .then(function(apps){
                    return new chlk.models.apps.SuggestedAppsList(classId, null, apps, null, standardUrlComponents_)
                });
            return this.UpdateView(this.getView().getCurrent().getClass(), result, 'apps');
        },

    ])
});
