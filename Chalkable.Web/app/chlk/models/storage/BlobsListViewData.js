REQUIRE('chlk.models.storage.Blob');
NAMESPACE('chlk.models.storage', function () {
    "use strict";
    /** @class chlk.models.storage.BlobsListViewData*/
    CLASS(
        'BlobsListViewData', [
            chlk.models.common.PaginatedList, 'items',
            String, 'containerAddress'
        ]);
});
