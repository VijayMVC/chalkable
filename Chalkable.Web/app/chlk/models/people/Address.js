REQUIRE('chlk.models.id.AddressId');

NAMESPACE('chlk.models.people', function () {
    "use strict";

    /** @class chlk.models.people.Address*/
    CLASS(
        'Address', [
            chlk.models.id.AddressId, 'id',

            Number, 'type',

            String, "value"
        ]);
});
