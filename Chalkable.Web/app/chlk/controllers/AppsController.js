REQUIRE('chlk.controllers.BaseController');

REQUIRE('chlk.services.ApplicationService');
REQUIRE('chlk.activities.apps.AppsListPage');
REQUIRE('chlk.models.id.AppId');

NAMESPACE('chlk.controllers', function (){

    /** @class chlk.controllers.AppsController */
    CLASS(
        'AppsController', EXTENDS(chlk.controllers.BaseController), [

        [ria.mvc.Inject],
        chlk.services.ApplicationService, 'appsService',

        [chlk.controllers.SidebarButton('apps')],
        [[chlk.models.id.AppId]],
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
        }

    ])
});
