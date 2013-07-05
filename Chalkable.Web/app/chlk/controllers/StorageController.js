REQUIRE('chlk.controllers.BaseController');
REQUIRE('chlk.services.StorageService');
REQUIRE('chlk.activities.storage.StorageListPage');

NAMESPACE('chlk.controllers', function (){

    /** @class chlk.controllers.StorageController */
    CLASS(
        'StorageController', EXTENDS(chlk.controllers.BaseController), [

            [ria.mvc.Inject],
            chlk.services.StorageService, 'storageService',

            [chlk.controllers.SidebarButton('settings')],
            [[Number]],
            function listAction(pageIndex_) {
                var result = this.storageService
                    .getStorages(pageIndex_ | 0)
                    .attach(this.validateResponse_());
                /* Put activity in stack and render when result is ready */
                return this.PushView(chlk.activities.storage.StorageListPage, result);
            },
            [[String]],
            function detailsAction(pageIndex_) {
            }
        ])
});
