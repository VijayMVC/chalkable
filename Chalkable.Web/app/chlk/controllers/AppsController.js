REQUIRE('chlk.controllers.BaseController');

REQUIRE('chlk.services.ApplicationService');
REQUIRE('chlk.activities.apps.AppsListPage');

NAMESPACE('chlk.controllers', function (){

    /** @class chlk.controllers.AppsController */
    CLASS(
        'AppsController', EXTENDS(chlk.controllers.BaseController), [

        [ria.mvc.Inject],
        chlk.services.ApplicationService, 'appsService',

        [chlk.controllers.SidebarButton('apps')],
        [[Number]],
        function listAction(pageIndex_) {
            var result = this.appsService
                .getApps(pageIndex_ | 0)
                .attach(this.validateResponse_());
            /* Put activity in stack and render when result is ready */
            return this.PushView(chlk.activities.apps.AppsListPage, result);
        }
    ])
});
