REQUIRE('chlk.models.people.Role');

NAMESPACE('chlk.models.apps', function () {
    "use strict";
    /** @class chlk.models.apps.AppRoleRating*/
    CLASS(
        'AppRoleRating', [

            [ria.serialize.SerializeProperty('avgrating')],
            Number, 'rating',

            [ria.serialize.SerializeProperty('personcount')],
            Number, 'personCount',

            chlk.models.people.Role, 'role'

        ]);
});
