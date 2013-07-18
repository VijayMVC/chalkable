REQUIRE('chlk.controllers.BaseController');
REQUIRE('chlk.services.StorageService');
REQUIRE('chlk.activities.storage.StorageListPage');
REQUIRE('chlk.activities.storage.StorageBlobsPage');

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
            [[String, Number]],
            function blobsAction(uri, pageIndex_) {
                var result = this.storageService
                    .getBlobs(uri, pageIndex_ | 0)
                    .attach(this.validateResponse_());
                return this.PushView(chlk.activities.storage.StorageBlobsPage, result);
            },
            [[String]],
            function deleteAction(uri) {

            }
        ])
});
