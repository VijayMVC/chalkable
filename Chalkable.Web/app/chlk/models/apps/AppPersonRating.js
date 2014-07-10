REQUIRE('chlk.models.people.ShortUserInfo');
REQUIRE('chlk.models.people.Role');

NAMESPACE('chlk.models.apps', function () {
    "use strict";
    /** @class chlk.models.apps.AppPersonRating*/
    CLASS(
        'AppPersonRating', [
            chlk.models.people.ShortUserInfo, 'person',
            Number, 'rating',
            String, 'review'
        ]);
});
