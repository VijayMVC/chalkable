REQUIRE('chlk.models.people.Address');

NAMESPACE('chlk.models.people', function () {
    "use strict";

    /** @class chlk.models.people.Addresses*/
    CLASS(
        'Addresses', [
            ArrayOf(chlk.models.people.Address), 'items'
        ]);
});
