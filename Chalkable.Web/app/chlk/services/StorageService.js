REQUIRE('chlk.services.BaseService');
REQUIRE('ria.async.Future');
REQUIRE('chlk.models.storage.Storage');
REQUIRE('chlk.models.storage.Blob');

NAMESPACE('chlk.services', function () {
    "use strict";
    /** @class chlk.services.StorageService*/
    CLASS(
        'StorageService', EXTENDS(chlk.services.BaseService), [
            [[Number]],
            ria.async.Future, function getStorages(pageIndex_) {
                return this.getPaginatedList('app/data/storage.json', chlk.models.storage.Storage, {
                    start: pageIndex_
                });
            },

            [[String, Number]],
            ria.async.Future, function getBlobs(uri, pageIndex_) {
                return this.getPaginatedList('app/data/blobs.json', chlk.models.storage.Blob, {
                    uri: uri,
                    start: pageIndex_
                });
            }
        ])
});