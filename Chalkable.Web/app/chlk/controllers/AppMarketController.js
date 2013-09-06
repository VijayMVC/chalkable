REQUIRE('chlk.controllers.BaseController');

REQUIRE('chlk.services.AppMarketService');
REQUIRE('chlk.services.AppCategoryService');
REQUIRE('chlk.services.GradeLevelService');

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

        [ria.mvc.Inject],
        chlk.services.AppCategoryService, 'appCategoryService',

        [ria.mvc.Inject],
        chlk.services.GradeLevelService, 'gradeLevelService',

        [chlk.controllers.SidebarButton('apps')],

        [[Number]],
        function listAction(pageIndex_) {
            var result = this.appCategoryService
                .getCategories()
                .then(function(data){
                    return data.getItems();
                })
                .then(function(categories){
                    return this.appMarketService
                        .getApps(
                            categories,
                            this.gradeLevelService.getGradeLevels(),
                            ""
                        );
                }, this)
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
        }
    ])
});
