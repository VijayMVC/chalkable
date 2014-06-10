REQUIRE('chlk.models.id.SchoolPersonId');
REQUIRE('chlk.models.apps.AppPersonRating');
REQUIRE('chlk.models.apps.AppRoleRating');

NAMESPACE('chlk.models.apps', function () {
    "use strict";

    /** @class chlk.models.apps.AppRating*/
    CLASS(
        'AppRating', [
            [ria.serialize.SerializeProperty('avg')],
            Number, 'average',

            [ria.serialize.SerializeProperty('ratingbyperson')],
            ArrayOf(chlk.models.apps.AppPersonRating), 'personRatings',

            [ria.serialize.SerializeProperty('ratingbyroles')],
            ArrayOf(chlk.models.apps.AppRoleRating), 'roleRatings'
        ]);
});
