REQUIRE('chlk.models.storage.Blob');
NAMESPACE('chlk.models.storage', function () {
    "use strict";
    /** @class chlk.models.storage.BlobsList*/
    CLASS(
        'BlobsList', [
            ArrayOf(chlk.models.storage.Blob), 'items'
        ]);
});
