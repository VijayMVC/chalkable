REQUIRE('chlk.models.id.PhoneId');

NAMESPACE('chlk.models.people', function () {
    "use strict";

    /** @class chlk.models.people.Phone*/
    CLASS(
        'Phone', [
            chlk.models.id.PhoneId, 'id',

            [ria.serialize.SerializeProperty('isprimary')],
            Boolean, 'isPrimary',

            Number, 'type',

            String, "value"
        ]);
});
