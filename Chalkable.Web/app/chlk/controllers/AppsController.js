REQUIRE('chlk.controllers.BaseController');

REQUIRE('chlk.services.ApplicationService');
REQUIRE('chlk.services.AppCategoryService');
REQUIRE('chlk.services.GradeLevelService');
REQUIRE('chlk.activities.apps.AppsListPage');
REQUIRE('chlk.activities.apps.AppInfoPage');
REQUIRE('chlk.activities.apps.AddAppDialog');
REQUIRE('chlk.models.apps.Application');
REQUIRE('chlk.models.apps.AppPostData');
REQUIRE('chlk.models.apps.ShortAppInfo');
REQUIRE('chlk.models.apps.AppAccess');
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

        [chlk.controllers.SidebarButton('apps')],
        [[Number]],
        function listAction(pageIndex_) {
            var result = this.appsService
                .getApps(pageIndex_ | 0)
                .attach(this.validateResponse_());
            return this.PushView(chlk.activities.apps.AppsListPage, result);
        },

        [chlk.controllers.AccessForRoles([
            chlk.models.common.RoleEnum.DEVELOPER
        ])],
        [[chlk.models.apps.Application]],
        ria.async.Future, function prepareAppInfo(app_) {
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
                    return new chlk.models.apps.AppInfoViewData(app_, false, cats, gradeLevels, permissions, true);

                }, this);

            return result;
        },

        [chlk.controllers.AccessForRoles([
            chlk.models.common.RoleEnum.DEVELOPER
        ])],
        [[chlk.models.id.AppId]],
        function detailsDeveloperAction(appId_) {
            var app = this.appsService
                    .getInfo(appId_)
                    .then(function(data){
                        if (!data.getId()){
                            return this.forward_('apps', 'add', []);
                        }
                        else
                            return this.PushView(chlk.activities.apps.AppInfoPage, this.prepareAppInfo(data));
                    }, this)
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


        [chlk.controllers.AccessForRoles([
            chlk.models.common.RoleEnum.DEVELOPER
        ])],
        [[chlk.models.apps.AppPostData]],
        function updateDeveloperAction(model){
             var shortAppData = new chlk.models.apps.ShortAppInfo(
                model.getName(),
                model.getUrl(),
                model.getVideoModeUrl(),
                model.getShortDescription(),
                model.getLongDescription()
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


             var appPriceInfo = new chlk.models.apps.AppPrice(
                 model.getPrice(),
                 model.getPricePerClass(),
                 model.getPricePerSchool()
             );

             var pictures = [];

             var cats = model.getCategories() ? model.getCategories().split(',').map(function(item){
                 return new chlk.models.id.AppCategoryId(item)
             }) : [];


             var gradeLevels = model.getGradeLevels() ? model.getGradeLevels().split(',').map(function(item){
                 return new chlk.models.id.GradeLevelId(item)
             }) : [];

             var appPermissions = model.getPermissions() ? model.getPermissions().split(',').map(function(item){
                 return new chlk.models.id.AppPermissionId(item)
             }) : [];

             var result = this.appsService
                 .updateApp(
                     model.getId(),
                     shortAppData,
                     appPermissions,
                     appPriceInfo,
                     this.getCurrentPerson().getId(),
                     appAccess,
                     cats,
                     pictures,
                     gradeLevels,
                     !model.isDraft()
                 )
                 .then(function(updatedApp){
                     this.switchApp(updatedApp);
                 }, this)
                     .then(function(){
                         return this.forward_('apps', 'details', []);
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
                        var appGeneralInfo = new chlk.models.apps.AppGeneralInfoViewData(app.getId(), app.getName(), app.getStatus());
                        return this.PushView(chlk.activities.apps.AppGeneralInfoPage, new ria.async.DeferredData(appGeneralInfo));
                    }
                }, this)
        }
    ])
});
