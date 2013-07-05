REQUIRE('chlk.models.common.ChlkDate');
NAMESPACE('chlk.models.storage', function () {
    "use strict";
    /** @class chlk.models.storage.Storage*/
    CLASS(
        'Storage', [
           String, 'name',
           String, 'uri'
        ]);
});
