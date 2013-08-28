REQUIRE('chlk.controllers.BaseController');

REQUIRE('chlk.services.ApplicationService');
REQUIRE('chlk.services.AppCategoryService');
REQUIRE('chlk.services.GradeLevelService');
REQUIRE('chlk.services.PictureService');

REQUIRE('chlk.activities.apps.AppsListPage');
REQUIRE('chlk.activities.apps.AppInfoPage');
REQUIRE('chlk.activities.apps.AppGeneralInfoPage');
REQUIRE('chlk.activities.apps.AddAppDialog');

REQUIRE('chlk.models.apps.Application');
REQUIRE('chlk.models.apps.AppPostData');
REQUIRE('chlk.models.apps.ShortAppInfo');
REQUIRE('chlk.models.apps.AppAccess');
REQUIRE('chlk.models.apps.AppPicture');
REQUIRE('chlk.models.apps.AppState');

REQUIRE('chlk.models.id.AppId');
REQUIRE('chlk.models.id.AppPermissionId');
REQUIRE('chlk.models.apps.AppGeneralInfoViewData');

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
                    var appGradeLevels = app_.getGradeLevels();
                    if (!appGradeLevels) app_.setGradeLevels([]);
                    var appCategories = app_.getCategories();
                    if (!appCategories) app_.setCategories([]);

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

                    return new chlk.models.apps.AppInfoViewData(app_, readOnly, cats, gradeLevels, permissions, isDraft);

                }, this);

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
                            return this.forward_('apps', 'add', []);
                        }
                        else
                            return this.PushView(chlk.activities.apps.AppInfoPage, this.prepareAppInfo(data, isReadonly, isDraft));
                    }, this)
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
                }.bind(this));
            return this.UpdateView(chlk.activities.apps.AppInfoPage, result, msg.toLowerCase());
        },

        [chlk.controllers.AccessForRoles([
            chlk.models.common.RoleEnum.DEVELOPER
        ])],
        [[String, Number, Number]],
        function deletePictureDeveloperAction(title, width, height) {
            var result= new ria.async.DeferredData(new chlk.models.apps.AppPicture(new chlk.models.id.PictureId(''), '#', width, height, title, true));
            return this.UpdateView(chlk.activities.apps.AppInfoPage, result, title.toLowerCase());
        },



        [chlk.controllers.AccessForRoles([
            chlk.models.common.RoleEnum.SYSADMIN
        ])],
        [[chlk.models.id.AppId]],
        function approveSysAdminAction(appId) {
            return this.appsService
                .approveApp(appId)
                .then(function(data){
                    return this.redirect_('apps', 'list', []);
                }, this);
        },

        [chlk.controllers.AccessForRoles([
            chlk.models.common.RoleEnum.SYSADMIN
        ])],
        [[chlk.models.id.AppId]],
        function declineSysAdminAction(appId) {
            return this.appsService
                .declineApp(appId)
                .then(function(data){
                    return this.redirect_('apps', 'list', []);
                }, this);
        },

        [chlk.controllers.AccessForRoles([
            chlk.models.common.RoleEnum.DEVELOPER
        ])],
        [[chlk.models.id.AppId]],
        function goLiveDeveloperAction(appId) {
            return this.appsService
                .goLive(appId)
                .then(function(data){
                    return this.redirect_('apps', 'general', []);
                }, this);
        },

        [chlk.controllers.AccessForRoles([
            chlk.models.common.RoleEnum.DEVELOPER
        ])],
        [[chlk.models.id.AppId]],
        function unlistDeveloperAction(appId) {
            return this.appsService
                .unlist(appId)
                .then(function(data){
                    return this.redirect_('apps', 'general', []);
                }, this);
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
                }, this);
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
            var result = this.appsService
                .createApp(devId, model.getName())
                .then(function(){
                    return this.forward_('apps', 'details', []);
                }, this);
            return result;
        },

        [chlk.controllers.AccessForRoles([
            chlk.models.common.RoleEnum.DEVELOPER
        ])],
        [[chlk.models.id.AppId]],
        function deleteDeveloperAction(id) {
            return this.appsService
                .deleteApp(id)
                .then(function(){
                    return this.forward_('apps', 'details', []);
                }, this);
            return result;
        },


        [chlk.controllers.AccessForRoles([
            chlk.models.common.RoleEnum.DEVELOPER
        ])],
        [[chlk.models.apps.Application]],
        function updateApp(app) {
            this.getContext().getSession().set('currentApp', app);
            return this.forward_('apps', 'details', [app.getId().valueOf(), true]);
        },

        [chlk.controllers.AccessForRoles([
            chlk.models.common.RoleEnum.DEVELOPER
        ])],
        [[chlk.models.id.AppId]],
        function tryDeleteApplicationAction(id) {
            return this.ShowMsgBox('Are you sure you want to delete?', null, [{
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





        [[String, Function]],
        function getIdsList(ids, idClass){
            var result = ids ? ids.split(',').map(function(item){
                return new idClass(item)
            }) : [];
            return result;
        },


        [chlk.controllers.AccessForRoles([
            chlk.models.common.RoleEnum.DEVELOPER
        ])],
        [[chlk.models.apps.AppPostData]],
        function updateDeveloperAction(model){


             var shortAppData = new chlk.models.apps.ShortAppInfo(
                model.getName(),
                model.getUrl(),
                model.getVideoDemoUrl(),
                model.getShortDescription(),
                model.getLongDescription(),
                model.getAppIconId(),
                model.getAppBannerId()
             );

                //small pic id,
                //big pic id

             var appAccess = new chlk.models.apps.AppAccess(
                 model.isHasStudentMyApps(),
                 model.isHasTeacherMyApps(),
                 model.isHasAdminMyApps(),
                 model.isHasParentMyApps(),
                 model.isCanAttach(),
                 model.isShowInGradingView()
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
            var appPermissions = this.getIdsList(model.getPermissions(), chlk.models.id.AppPermissionId);
            var appScreenShots = this.getIdsList(model.getAppScreenShotIds(), chlk.models.id.PictureId);

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
                     !model.isDraft()
                 )
                 .then(function(newApp){
                     return this.updateApp(newApp);
                 }, this);
             return result;
        },

        [chlk.controllers.AccessForRoles([
            chlk.models.common.RoleEnum.DEVELOPER
        ])],

        [[chlk.models.id.AppId]],
        function generalDeveloperAction(appId_){
            var app = this.appsService
                .getInfo(appId_)
                .then(function(data){
                    if (!data.getId()){
                        return this.forward_('apps', 'add', []);
                    }
                    else{
                        return this.PushView(chlk.activities.apps.AppGeneralInfoPage,
                            new ria.async.DeferredData(new chlk.models.apps.AppGeneralInfoViewData(data))
                        );
                    }
                }, this)
        }
    ])
});
