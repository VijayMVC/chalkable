REQUIRE('chlk.models.storage.Storage');
NAMESPACE('chlk.models.storage', function () {
    "use strict";
    /** @class chlk.models.storage.StorageList*/
    CLASS(
        'StorageList', [
            ArrayOf(chlk.models.storage.Storage), 'items'
        ]);
});
