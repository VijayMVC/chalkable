REQUIRE('chlk.controllers.BaseController');
REQUIRE('chlk.services.StorageService');
REQUIRE('chlk.models.storage.BlobsListViewData');

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
                return this.PushView(chlk.activities.storage.StorageListPage, result);
            },

            [[Number]],
            function pageAction(pageIndex_) {
                var result = this.storageService
                    .getStorages(pageIndex_ | 0)
                    .attach(this.validateResponse_());
                return this.UpdateView(chlk.activities.storage.StorageListPage, result);
            },

            [[String, Number]],
            function blobsAction(uri, pageIndex_) {
                var result = this.storageService
                    .getBlobs(uri, pageIndex_ | 0)
                    .attach(this.validateResponse_())
                    .then(function(data){
                        var blobsListViewData = new chlk.models.storage.BlobsListViewData();
                        blobsListViewData.setItems(data);
                        blobsListViewData.setContainerAddress(uri);
                        return new ria.async.DeferredData(blobsListViewData);
                });
                return this.PushView(chlk.activities.storage.StorageBlobsPage, result);
            },

            [[String, Number]],
            function blobsPageAction(uri, pageIndex_) {
                var result = this.storageService
                    .getBlobs(uri, pageIndex_ | 0)
                    .attach(this.validateResponse_())
                    .then(function(data){
                        var blobsListViewData = new chlk.models.storage.BlobsListViewData();
                        blobsListViewData.setItems(data);
                        blobsListViewData.setContainerAddress(uri);
                        return new ria.async.DeferredData(blobsListViewData);
                });
                return this.UpdateView(chlk.activities.storage.StorageBlobsPage, result);
            }
        ])
});
