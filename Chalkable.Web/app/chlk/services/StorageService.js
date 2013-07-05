REQUIRE('chlk.services.BaseService');
REQUIRE('ria.async.Future');
REQUIRE('chlk.models.storage.Storage');

NAMESPACE('chlk.services', function () {
    "use strict";
    /** @class chlk.services.StorageService*/
    CLASS(
        'StorageService', EXTENDS(chlk.services.BaseService), [
            [[Number]],
            ria.async.Future, function getStorages(pageIndex_) {
                return this.getPaginatedList('/app/data/storage.json', chlk.models.storage.Storage, pageIndex_);
            }
        ])
});