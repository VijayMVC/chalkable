REQUIRE('chlk.controllers.BaseController');

REQUIRE('chlk.services.AppMarketService');

REQUIRE('chlk.activities.apps.AppMarketPage');
REQUIRE('chlk.activities.apps.AppMarketDetailsPage');
REQUIRE('chlk.activities.apps.AttachAppDialog');
REQUIRE('chlk.models.id.AppId');

NAMESPACE('chlk.controllers', function (){

    /** @class chlk.controllers.AppMarketController */
    CLASS(
        'AppMarketController', EXTENDS(chlk.controllers.BaseController), [

        [ria.mvc.Inject],
        chlk.services.AppMarketService, 'appMarketService',

        [chlk.controllers.SidebarButton('apps')],
        [[Number]],
        function listAction(pageIndex_) {
            var result = this.appMarketService
                .getApps(pageIndex_ | 0)
                .attach(this.validateResponse_());
            return this.PushView(chlk.activities.apps.AppMarketPage, result);
        },

        [chlk.controllers.SidebarButton('apps')],
        [[chlk.models.id.AppId]],
        function detailsAction(id) {
            var result = this.appMarketService
                .getDetails(id)
                .attach(this.validateResponse_());
            return this.PushView(chlk.activities.apps.AppMarketDetailsPage, result);
        },


        [[Number]],
        function pageAction(pageIndex_) {
            var result = this.appMarketService
                .getApps(pageIndex_ | 0)
                .attach(this.validateResponse_());
            return this.UpdateView(chlk.activities.apps.AppMarketPage, result);
        },

        [[chlk.models.id.SchoolPersonId, Number]],
        function listAvailableForAttachTeacherPageAction(teacherId, pageIndex_) {
            var result = this.appMarketService
                .getInstalledApps(teacherId, pageIndex_)
                .attach(this.validateResponse_());
            return this.UpdateView(chlk.activities.apps.AttachAppDialog, result);
        }
    ])
});
