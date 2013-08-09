REQUIRE('chlk.controllers.BaseController');

REQUIRE('chlk.services.ApplicationService');
REQUIRE('chlk.services.AppCategoryService');
REQUIRE('chlk.services.GradeLevelService');
REQUIRE('chlk.activities.apps.AppsListPage');
REQUIRE('chlk.activities.apps.AppInfoPage');
REQUIRE('chlk.activities.apps.AddAppDialog');
REQUIRE('chlk.models.apps.Application');
REQUIRE('chlk.models.apps.AppPostData');
REQUIRE('chlk.models.apps.AppAccess');
REQUIRE('chlk.models.apps.AppState');
REQUIRE('chlk.models.id.AppId');
REQUIRE('chlk.models.id.AppPermissionId');

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
        [[chlk.models.id.AppId]],
        function deleteAction(id) {
        },
        [[chlk.models.id.AppId]],
        function detailsAction(id) {

        },

        [chlk.controllers.AccessForRoles([
            chlk.models.common.RoleEnum.DEVELOPER
        ])],
        chlk.models.apps.Application, function getCurrentApp(){
            var result = null;
            if (this.userInRole(this.getCurrentRole().getRoleId()))
                result = this.getContext().getSession().get('currentApp');
            return result;
        },


        [chlk.controllers.AccessForRoles([
            chlk.models.common.RoleEnum.DEVELOPER
        ])],
        [[chlk.models.id.AppId]],
        chlk.models.apps.Application, function getAppById(id){
            var result = null;
            if (this.userInRole(this.getCurrentRole().getRoleId())){
                var apps = this.getContext().getSession().get('dev-apps');
                result = apps
                        .filter(function(item){
                            return item.getId() == id;
                        });
            }

            if (result && result.length > 0) result = result[0];
            return result;
        },

        [chlk.controllers.AccessForRoles([
            chlk.models.common.RoleEnum.DEVELOPER
        ])],
        [[chlk.models.apps.Application]],
        function switchApp(app) {
            this.getContext().getSession().set('currentApp', app);
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
                    var permissions = [
                        new chlk.models.apps.AppPermission(new chlk.models.id.AppPermissionId(1), "User", 1),
                        new chlk.models.apps.AppPermission(new chlk.models.id.AppPermissionId(2), "Class", 2),
                        new chlk.models.apps.AppPermission(new chlk.models.id.AppPermissionId(3), "Grade", 3),
                        new chlk.models.apps.AppPermission(new chlk.models.id.AppPermissionId(4), "Announcement", 4),
                        new chlk.models.apps.AppPermission(new chlk.models.id.AppPermissionId(5), "Attendance", 5),
                        new chlk.models.apps.AppPermission(new chlk.models.id.AppPermissionId(6), "Discipline", 6),
                        new chlk.models.apps.AppPermission(new chlk.models.id.AppPermissionId(7), "Message", 7),
                        new chlk.models.apps.AppPermission(new chlk.models.id.AppPermissionId(8), "Schedule", 8)
                    ];



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
            var app = null;
            if (appId_)
                app = this.getAppById(appId_);
            if (!app)
                app = this.getCurrentApp();
            if (!app){
                return this.forward_('apps', 'add', []);
            }


            return this.PushView(chlk.activities.apps.AppInfoPage, this.prepareAppInfo(app));

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
                .then(function(data){
                    this.switchApp(data);
                }, this);
            return this.forward_('apps', 'details');
        },

        [chlk.controllers.AccessForRoles([
            chlk.models.common.RoleEnum.DEVELOPER
        ])],
        [[chlk.models.apps.AppPostData]],
        function updateDeveloperAction(model){
             //update all apps list

        }
    ])
});
