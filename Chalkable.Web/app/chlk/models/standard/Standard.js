REQUIRE('chlk.models.id.StandardId');

NAMESPACE('chlk.models.standard', function () {
    "use strict";
    /** @class chlk.models.standard.Standard*/
    CLASS(
        'Standard', [
             String, 'name',

             String, 'description',

             [ria.serialize.SerializeProperty('standardid')],
             chlk.models.id.StandardId, 'standardId'
        ]);
});
