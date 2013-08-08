REQUIRE('chlk.controllers.BaseController');

REQUIRE('chlk.services.ApplicationService');
REQUIRE('chlk.services.AppCategoryService');
REQUIRE('chlk.services.GradeLevelService');
REQUIRE('chlk.activities.apps.AppsListPage');
REQUIRE('chlk.activities.apps.AppInfoPage');
REQUIRE('chlk.models.apps.Application');
REQUIRE('chlk.models.apps.AppAccess');
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


        chlk.models.apps.Application, function getCurrentApp(){
            var result = null;
            if (this.userInRole(this.getCurrentRole().getRoleId()))
                result = this.getContext().getSession().get('currentApp');
            return result;
        },


        [chlk.controllers.AccessForRoles([
            chlk.models.common.RoleEnum.DEVELOPER
        ])],
        function detailsDeveloperAction() {

            var result = this.categoryService
                .getCategories()
                .then(function(data){
                    var cats = data.getItems();
                    var app = this.getCurrentApp();
                    app.setName("Test APP");
                    app.setUrl("www.no.com");
                    app.setShortDescription("Short description");
                    app.setDescription("Long description");
                    app.setVideoModeUrl("Video Url");




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

                    var appAccess = new chlk.models.apps.AppAccess();

                    app.setAppAccess(appAccess);

                    var gradeLevels = this.gradeLevelService.getGradeLevels();

                    app.setPermissions([new chlk.models.apps.AppPermission(new chlk.models.id.AppPermissionId(6), "Discipline", 6)]);


                    var appState = new chlk.models.apps.AppState();
                    appState.deserialize(1);

                    app.setState(appState);
                    var g1 = new chlk.models.common.NameId;
                    g1.setId(1);
                    g1.setName('1st');
                    var g2 = new chlk.models.common.NameId;
                    g2.setId(5);
                    g2.setName('5th');
                    app.setGradeLevels([
                        g1, g2
                    ]);


                    var cat = new chlk.models.apps.AppCategory();
                    cat.setId(new chlk.models.id.AppCategoryId('54e48fd5-cc7c-4726-867a-666ff876b317'));
                    cat.setName('Math');

                    app.setCategories([cat]);
                    return new ria.async.DeferredData(new chlk.models.apps.AppInfoViewData(app, false, cats, gradeLevels, permissions));

                }, this);
            return this.PushView(chlk.activities.apps.AppInfoPage, result);
        },
        [chlk.controllers.AccessForRoles([
            chlk.models.common.RoleEnum.DEVELOPER
        ])],
        [[chlk.models.apps.Application]],
        function updateDeveloperAction(){
            //update app
        }
    ])
});
